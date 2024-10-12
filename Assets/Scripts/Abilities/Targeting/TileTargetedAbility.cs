using UnityEngine;

public abstract class TileTargetedAbility : Ability
{
    public abstract void ActivateOnTile(Unit unit, Tile targetTile);
}