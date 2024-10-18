using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public bool isHighGround = false;
    public bool isLowGround = false;
    public bool isDifficultTerrain = false;
    public bool isImpassable = false;
    public bool isFlagTile = false;

    public Unit occupiedBy = null;

    private PulsatingEffect pulsatingEffect;
    public GameObject marker;
    void Start()
    {
        pulsatingEffect = GetComponent<PulsatingEffect>();
    }

    public void HighlightAsTarget()
    {
        if (pulsatingEffect != null)
        {
            pulsatingEffect.StartPulsating();
        }
    }

    public void RemoveHighlight()
    {
        if (pulsatingEffect != null)
        {
            pulsatingEffect.StopPulsating();
        }
    }

    public bool IsPassable()
    {
        return !isImpassable && occupiedBy == null;
    }

    public int GetMovementCost(bool fromHighGround, bool fromLowGround)
    {
        int cost = 1;

        if (isDifficultTerrain)
        {
            cost = 2;
        }

        if ((isHighGround && fromLowGround) || (isLowGround && fromHighGround))
        {
            cost = 2;
        }

        return cost;
    }

    public bool CanMoveTo(Tile other)
    {
        if (!IsPassable() || !other.IsPassable())
        {
            return false;
        }

        if ((isHighGround && other.isLowGround) || (isLowGround && other.isHighGround))
        {
            return false;
        }

        return true;
    }

    public float GetAccuracyModifier()
    {
        if (isHighGround)
        {
            return 0.2f;
        }
        else if (isLowGround)
        {
            return -0.2f;
        }
        return 0f;
    }

    public float GetEvasionModifier()
    {
        if (isHighGround)
        {
            return -0.1f;
        }
        else if (isLowGround)
        {
            return 0.1f;
        }
        return 0f;
    }

    public Tile[] GetAdjacentTiles()
    {
        List<Tile> adjacentTiles = new List<Tile>();

        Vector3[] directions = {
        new Vector3(1, 0, -0.577f),
        new Vector3(0, 0, -1.154f),
        new Vector3(-1, 0, -0.577f),
        new Vector3(-1, 0, 0.577f),
        new Vector3(0, 0, 1.154f),
        new Vector3(1, 0, 0.577f)
    };

        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPosition = transform.position + direction;
            Collider[] colliders = Physics.OverlapSphere(neighborPosition, 0.1f);

            foreach (Collider collider in colliders)
            {
                Tile tile = collider.GetComponent<Tile>();
                if (tile != null)
                {
                    adjacentTiles.Add(tile);
                }
            }
        }

        return adjacentTiles.ToArray();
    }

    private Tile FindTileAtPosition(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, 0.1f);
        foreach (var hit in hits)
        {
            Tile tile = hit.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
        }

        return null;
    }
    public bool HasLineOfSight(Tile targetTile)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = targetTile.transform.position;

        RaycastHit hit;
        Vector3 direction = (targetPos - startPos).normalized;

        if (Physics.Raycast(startPos + Vector3.up, direction, out hit, Vector3.Distance(startPos, targetPos)))
        {
            Tile hitTile = hit.collider.GetComponent<Tile>();

            if (hitTile != null && (hitTile.isImpassable || hitTile.occupiedBy != null))
            {
                return false;
            }
        }
        return true;
    }

    public float GetHeightModifierForUnit(Unit attackingUnit, Unit targetUnit)
    {

        if (isHighGround && targetUnit.currentTile.isLowGround)
        {
            return 0.2f;
        }

        else if (isLowGround && targetUnit.currentTile.isHighGround)
        {
            return -0.2f;
        }
        return 0f;
    }

    public float GetHeightModifierForEvasion(Unit attackingUnit, Unit targetUnit)
    {

        if (targetUnit.currentTile.isHighGround && attackingUnit.currentTile.isLowGround)
        {
            return 0.1f;
        }
        else if (targetUnit.currentTile.isLowGround && attackingUnit.currentTile.isHighGround)
        {
            return -0.1f;
        }
        return 0f;
    }

}