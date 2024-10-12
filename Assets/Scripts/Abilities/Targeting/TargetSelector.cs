using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public Unit currentUnit;
    public Ability selectedAbility;
    private bool isTargeting = false;

    void Update()
    {
        if (selectedAbility != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Tile targetTile = hit.collider.GetComponent<Tile>();

                if (targetTile != null && selectedAbility is TileTargetedAbility)
                {
                    if (!IsTileAvailable(targetTile))
                    {
                        Debug.Log("Тайл занят, непроходим или находится слишком далеко.");
                        return;
                    }

                    Debug.Log($"Выбрана цель: тайл {targetTile.name}");
                    ((TileTargetedAbility)selectedAbility).ActivateOnTile(currentUnit, targetTile);
                    selectedAbility = null;
                    RemovePulsatingEffectFromTargets();
                    isTargeting = false;
                    return;
                }

                Unit targetUnit = hit.collider.GetComponent<Unit>();

                // Логика для атакующих способностей
                if (targetUnit != null && selectedAbility is MeleeAttackAbility)
                {
                    Debug.Log($"Выбран юнит {targetUnit.unitName} для ближней атаки.");
                    ((MeleeAttackAbility)selectedAbility).ActivateOnUnit(currentUnit, targetUnit);
                    selectedAbility = null;
                    RemovePulsatingEffectFromTargets();
                    isTargeting = false;
                    return;
                }
                else if (targetUnit != null && selectedAbility is RangedAttackAbility)
                {
                    Debug.Log($"Выбран юнит {targetUnit.unitName} для дистанционной атаки.");
                    ((RangedAttackAbility)selectedAbility).ActivateOnUnit(currentUnit, targetUnit);
                    selectedAbility = null;
                    RemovePulsatingEffectFromTargets();
                    isTargeting = false;
                    return;
                }

                // Логика для защитной способности
                if (targetUnit != null && selectedAbility is DefenseAbility)
                {
                    Debug.Log($"Выбран юнит {targetUnit.unitName} для защиты.");
                    ((DefenseAbility)selectedAbility).ActivateOnUnit(currentUnit, targetUnit);
                    selectedAbility = null;
                    RemovePulsatingEffectFromTargets();
                    isTargeting = false;
                    return;
                }

                Debug.Log("Некорректная цель.");
            }
        }

        // Обработка горячих клавиш для активации способностей
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isTargeting)
            {
                CancelTargeting();
            }
            else
            {
                ActivateMovementAbility();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (isTargeting)
            {
                CancelTargeting();
            }
            else
            {
                ActivateMeleeAttackAbility();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (isTargeting)
            {
                CancelTargeting();
            }
            else
            {
                ActivateRangedAttackAbility();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (isTargeting)
            {
                CancelTargeting();
            }
            else
            {
                ActivateDefenseAbility();
            }
        }
    }


    void ActivateMeleeAttackAbility()
    {
        selectedAbility = currentUnit.abilities[1];
        Debug.Log("Активирована способность ближней атаки. Выберите цель.");
        var meleeAttack = (MeleeAttackAbility)selectedAbility;
        HighlightEnemiesInRange(meleeAttack.RangeWithOffset); // Используем корректное свойство с добавкой
        isTargeting = true;
    }


    void ActivateRangedAttackAbility()
    {
        selectedAbility = currentUnit.abilities[2];
        Debug.Log("Активирована способность дистанционной атаки. Выберите цель.");
        var rangedAttack = (RangedAttackAbility)selectedAbility;
        HighlightEnemiesInRange(rangedAttack.RangeWithOffset); // Используем корректное свойство с добавкой
        isTargeting = true;
    }

    bool IsEnemy(Unit targetUnit)
    {
        return currentUnit.team != targetUnit.team;
    }

    bool IsAlly(Unit targetUnit)
    {
        return currentUnit.team == targetUnit.team;
    }

    void ActivateMovementAbility()
    {
        selectedAbility = currentUnit.abilities[0];
        Debug.Log("Активирована способность перемещения. Выберите тайл.");
        HighlightAvailableTilesForMovement();
        isTargeting = true;
    }

    void ActivateDefenseAbility()
    {
        selectedAbility = currentUnit.abilities[3];
        Debug.Log("Активирована способность защиты. Выберите юнита.");
        HighlightCurrentUnitForEndTurn();
        isTargeting = true;
    }

    void CancelTargeting()
    {
        Debug.Log("Отмена выбора способности: " + selectedAbility?.abilityName);
        RemovePulsatingEffectFromTargets();
        selectedAbility = null;
        isTargeting = false;
    }

    void HighlightAvailableTilesForMovement()
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
            if (unit != currentUnit && IsEnemy(unit) && Vector3.Distance(currentUnit.transform.position, unit.transform.position) <= range)
            {
                unit.HighlightAsTarget();
            }
        }
    }
    void HighlightAlliesInRange(float range)
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (var unit in allUnits)
        {
            if (unit != currentUnit && IsAlly(unit) && Vector3.Distance(currentUnit.transform.position, unit.transform.position) <= range)
            {
                unit.HighlightAsTarget();
            }
        }
    }
    void HighlightCurrentUnitForEndTurn()
    {
        currentUnit.HighlightAsTarget();
    }

    void RemovePulsatingEffectFromTargets()
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

    void OnDisable()
    {
        if (selectedAbility != null)
        {
            selectedAbility = null;
        }

        if (currentUnit != null)
        {
            currentUnit = null;
        }
    }
}