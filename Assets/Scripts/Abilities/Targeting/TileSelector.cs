using UnityEngine;
using System.Collections.Generic;

public class TileSelector : MonoBehaviour
{
    public enum SelectionType
    {
        SingleTile,
        Hexagon,
        Cone,
        Line,
        Ray
    }

    public List<Tile> selectedTiles = new List<Tile>();
    public TargetSelector targetSelector;
    public Ability selectedAbility;
    public HighlightController highlightController;
    public void HandleTileOrUnitSelection(Tile targetTile)
    {
        Unit targetUnit = targetTile.occupiedBy;

        if (selectedAbility is TileTargetedAbility tileAbility)
        {
            if (targetUnit != null)
            {
                Debug.Log($"Цель выбрана: {targetUnit.unitName}");
                tileAbility.ActivateOnTile(targetUnit, targetTile);
            }
            else
            {
                Debug.Log($"Цель выбрана: тайл {targetTile.name}");
                tileAbility.ActivateOnTile(null, targetTile);
            }

            ClearHighlights();
        }
        else
        {
            Debug.LogError("Выбранная способность не поддерживает активацию на тайле.");
        }
    }

    public static List<Tile> GetTilesInCircle(Tile centerTile, int radius = 1)
    {
        List<Tile> result = new List<Tile> { centerTile };
        Queue<Tile> tilesToCheck = new Queue<Tile>();
        tilesToCheck.Enqueue(centerTile);

        int currentRadius = 0;

        while (currentRadius < radius)
        {
            int count = tilesToCheck.Count;

            for (int i = 0; i < count; i++)
            {
                Tile currentTile = tilesToCheck.Dequeue();
                foreach (var neighbor in currentTile.GetAdjacentTiles())
                {
                    if (!result.Contains(neighbor))
                    {
                        result.Add(neighbor);
                        tilesToCheck.Enqueue(neighbor);
                    }
                }
            }

            currentRadius++;
        }

        return result;
    }

    public void HighlightTiles(Unit unit, SelectionType selectionType, float range)
    {
        ClearHighlights();

        switch (selectionType)
        {
            case SelectionType.SingleTile:
                HighlightSingleTile(unit, range);
                break;
            case SelectionType.Hexagon:
                HighlightHexagon(unit, range);
                break;
            case SelectionType.Cone:
                HighlightCone(unit, range);
                break;
            case SelectionType.Line:
                HighlightLine(unit, range);
                break;
            case SelectionType.Ray:
                HighlightRay(unit, range);
                break;
        }
    }

    private void HighlightSingleTile(Unit unit, float range)
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (var tile in allTiles)
        {
            if (Vector3.Distance(unit.transform.position, tile.transform.position) <= range)
            {
                tile.HighlightAsTarget();
                selectedTiles.Add(tile);
            }
        }
    }

    private void HighlightHexagon(Unit unit, float range)
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (var tile in allTiles)
        {
            if (Vector3.Distance(unit.transform.position, tile.transform.position) <= range)
            {
                tile.HighlightAsTarget();
                selectedTiles.Add(tile);
            }
        }
    }

    private void HighlightCone(Unit unit, float range)
    {
        Debug.Log("Подсветка конуса пока не реализована.");
    }

    private void HighlightLine(Unit unit, float range)
    {
        Debug.Log("Подсветка линии пока не реализована.");
    }

    private void HighlightRay(Unit unit, float range)
    {
        Debug.Log("Подсветка луча пока не реализована.");
    }

    public void ClearHighlights()
    {
        if (targetSelector != null)
        {
            targetSelector.ClearSelection();
            selectedAbility = null;
        }
    }
    public void HighlightTile(Tile targetTile)
    {
        highlightController.EnableHexHighlight(targetTile.transform.position);
    }
    public void ClearTileHighlight()
    {
        highlightController.DisableHexHighlight();
    }
}
