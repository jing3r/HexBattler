using UnityEngine;

public abstract class TileTargetedAbility : Ability
{
    public TileSelector.SelectionType selectionType;
    public float selectionRange = 1f;

    public abstract void ActivateOnTile(Unit unit, Tile targetTile);
}