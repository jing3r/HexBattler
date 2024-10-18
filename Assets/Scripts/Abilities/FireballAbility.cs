using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Fireball Ability", menuName = "Abilities/Fireball")]
public class FireballAbility : TileTargetedAbility
{
    public int centralDamage = 36; // TODO исправить на 6d6 по центру
    public int peripheralDamage = 18; // исправить на 3d6 по краям
    public int radius = 1; // Радиус действия в тайлах, при 0 - только центральный.

    public override void Activate(Unit unit)
    {
        FindObjectOfType<TileSelector>().selectedAbility = this;
    }

    public override void ActivateOnTile(Unit unit, Tile targetTile)
    {
        List<Tile> affectedTiles = TileSelector.GetTilesInCircle(targetTile, radius);
        DealDamageToUnits(unit, targetTile, affectedTiles);
        unit.currentAP -= costAP;

        if (unit.currentAP == 0)
        {
            unit.EndTurn();
        }
    }

    private void DealDamageToUnits(Unit unit, Tile centralTile, List<Tile> affectedTiles)
    {
        foreach (Tile tile in affectedTiles)
        {
            if (tile.occupiedBy != null)
            {
                int damage = (tile == centralTile) ? centralDamage : peripheralDamage;
                tile.occupiedBy.TakeDamage(damage);
                Debug.Log($"{unit.unitName} наносит {damage} урона по {tile.occupiedBy.unitName}");
            }
        }
    }
}
