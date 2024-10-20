using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public Unit currentUnit;
    public Ability selectedAbility;
    public TileSelector tileSelector;
    public HighlightController highlightController;
    void Update()
    {
        if (selectedAbility != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Tile targetTile = hit.collider.GetComponent<Tile>();

                if (targetTile != null && selectedAbility is TileTargetedAbility tileAbility)
                {
                    Debug.Log($"Выбрана цель: тайл {targetTile.name}");
                    tileAbility.ActivateOnTile(currentUnit, targetTile);
                    selectedAbility = null;
                    tileSelector.ClearHighlights();
                    return;
                }

                Unit targetUnit = hit.collider.GetComponent<Unit>();

                if (targetUnit != null && selectedAbility is MeleeAttackAbility meleeAbility)
                {
                    Debug.Log($"Выбран юнит {targetUnit.unitName} для ближней атаки.");
                    meleeAbility.ActivateOnUnit(currentUnit, targetUnit);
                    selectedAbility = null;
                    tileSelector.ClearHighlights();
                    return;
                }
                else if (targetUnit != null && selectedAbility is RangedAttackAbility rangedAbility)
                {
                    Debug.Log($"Выбран юнит {targetUnit.unitName} для дистанционной атаки.");
                    rangedAbility.ActivateOnUnit(currentUnit, targetUnit);
                    selectedAbility = null;
                    tileSelector.ClearHighlights();
                    return;
                }
                else if (targetUnit != null && selectedAbility is DefenseAbility defenseAbility)
                {
                    Debug.Log($"Выбран юнит {targetUnit.unitName} для защиты.");
                    defenseAbility.ActivateOnUnit(currentUnit, targetUnit);
                    selectedAbility = null;
                    tileSelector.ClearHighlights();
                    return;
                }

                Debug.Log("Некорректная цель.");
            }
        }
    }

    public void SetSelectedAbility(Ability ability)
    {
        selectedAbility = ability;
        Debug.Log($"Активирована способность: {ability.abilityName}. Выберите цель.");

        if (selectedAbility is TileTargetedAbility)
        {
            HighlightAvailableTilesForAbility();
        }
        else if (selectedAbility is MeleeAttackAbility)
        {
            HighlightEnemiesInRange(((MeleeAttackAbility)selectedAbility).RangeWithOffset);
        }
        else if (selectedAbility is RangedAttackAbility)
        {
            HighlightEnemiesInRange(((RangedAttackAbility)selectedAbility).RangeWithOffset);
        }
        else if (selectedAbility is DefenseAbility)
        {
            HighlightAlliesInRange(((DefenseAbility)selectedAbility).RangeWithOffset);
        }
    }
    public void ActivateTileTargetingAbility(TileTargetedAbility ability)
    {
        selectedAbility = ability;
        tileSelector.HighlightTiles(currentUnit, ability.selectionType, ability.selectionRange);
        Debug.Log("Активирована способность для выбора тайлов. Выберите цель.");
    }
    public void CancelTargeting()
    {
        Debug.Log("Отмена выбора способности: " + selectedAbility?.abilityName);
        ClearSelection();
    }

    public void ClearSelection()
    {
        selectedAbility = null;
        RemovePulsatingEffectFromTargets();
    }

    public void HighlightAvailableTilesForAbility()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (var tile in allTiles)
        {
            if (IsTileAvailable(tile))
            {
                tile.HighlightAsTarget();
            }
        }
    }

    bool IsTileAvailable(Tile tile)
    {
        float distance = Vector3.Distance(currentUnit.transform.position, tile.transform.position);
        return distance <= currentUnit.movementSpeed && tile.IsPassable() && tile.occupiedBy == null;
    }

    void HighlightEnemiesInRange(float range)
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (var unit in allUnits)
        {
            if (IsEnemy(unit) && Vector3.Distance(currentUnit.transform.position, unit.transform.position) <= range)
            {
                unit.HighlightAsTarget();
            }
        }
    }

    public void HighlightAlliesInRange(float range)
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (var unit in allUnits)
        {
            if (Vector3.Distance(currentUnit.transform.position, unit.transform.position) <= range)
            {
                unit.HighlightAsTarget();
            }
        }
    }

    bool IsEnemy(Unit targetUnit)
    {
        return currentUnit.team != targetUnit.team;
    }

    bool IsAlly(Unit targetUnit)
    {
        return currentUnit.team == targetUnit.team;
    }

    public void RemovePulsatingEffectFromTargets()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (var tile in allTiles)
        {
            tile.RemoveHighlight();
        }

        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (var unit in allUnits)
        {
            unit.RemoveHighlight();
        }
    }
    private bool IsValidTarget(Unit targetUnit, Ability ability)
    {
        if (targetUnit == null) return false;

        float distance = Vector3.Distance(currentUnit.transform.position, targetUnit.transform.position);
        return distance <= ability.range + 0.5f;
    }
    public void HighlightUnit(Unit targetUnit)
    {
        highlightController.EnableCircularHighlight(targetUnit.transform.position);
    }

    public void ClearUnitHighlight()
    {
        highlightController.DisableCircularHighlight();
    }
    void OnDisable()
    {
        selectedAbility = null;
        currentUnit = null;
    }
}
