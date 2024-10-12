using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "New Ranged Attack Ability", menuName = "Abilities/RangedAttack")]
public class RangedAttackAbility : Ability
{
    public int minRange = 2;
    public int damage = 8;

    public float RangeWithOffset => range + 0.5f;

    public override void Activate(Unit unit)
    {
        Debug.Log(unit.unitName + " готовится к дальнему бою. Выберите врага.");
        FindObjectOfType<TargetSelector>().selectedAbility = this;
    }

    public void ActivateOnUnit(Unit unit, Unit targetUnit)
    {
        if (unit.team == targetUnit.team)
        {
            Debug.Log("Невозможно атаковать союзника.");
            return;
        }

        float distance = Vector3.Distance(unit.transform.position, targetUnit.transform.position);

        if (distance <= RangeWithOffset && distance >= minRange)
        {

            float finalAccuracy = unit.accuracy;
            float targetEvasion = targetUnit.evasion;

            float heightModifierAccuracy = targetUnit.currentTile.GetHeightModifierForUnit(unit, targetUnit);
            float heightModifierEvasion = targetUnit.currentTile.GetHeightModifierForEvasion(unit, targetUnit);

            finalAccuracy += heightModifierAccuracy;
            targetEvasion += heightModifierEvasion;

            bool isInCover = CoverChecker.IsTargetInCover(unit, targetUnit);
            if (isInCover)
            {
                Debug.Log("Цель находится в укрытии!");
                finalAccuracy -= 0.2f;
            }

            float chanceToHit = Mathf.Clamp(finalAccuracy - targetEvasion, 0f, 1f);
            Debug.Log($"{unit.unitName} атакует {targetUnit.unitName} с шансом попадания {chanceToHit * 100}%.");
            bool hitTarget = Random.value <= chanceToHit;

            if (hitTarget)
            {
                targetUnit.TakeDamage(damage);
                Debug.Log($"{unit.unitName} попадает по {targetUnit.unitName} и наносит {damage} урона.");
            }
            else
            {
                List<Unit> adjacentUnits = GetAdjacentUnits(targetUnit);
                bool hitAnyAdjacent = false;
                foreach (var adjacentUnit in adjacentUnits)
                {
                    if (Random.Range(0f, 1f) <= 0.05f) // 5% шанс попасть по юниту, стоящему рядом с целью
                    {
                        Debug.Log($"{unit.unitName} промахивается по {targetUnit.unitName}, но случайно попадает по {adjacentUnit.unitName} и наносит{damage} урона.");
                        adjacentUnit.TakeDamage(damage);
                        hitAnyAdjacent = true;
                        break;
                    }
                }

                if (!hitAnyAdjacent)
                {
                    Debug.Log($"{unit.unitName} промаивается по {targetUnit.unitName}.");
                }
            }

            unit.currentAP -= costAP;

            if (unit.currentAP == 0)
            {
                unit.EndTurn();
            }
        }
        else if (distance < minRange)
        {
            Debug.Log("Цель слишком близко для дальнего боя.");
        }
        else
        {
            Debug.Log("Цель слишком далеко для дальнего боя.");
        }
    }
    private List<Unit> GetAdjacentUnits(Unit targetUnit)
    {
        Tile[] adjacentTiles = targetUnit.currentTile.GetAdjacentTiles();
        List<Unit> adjacentUnits = new List<Unit>();

        foreach (Tile tile in adjacentTiles)
        {
            if (tile.occupiedBy != null && tile.occupiedBy != targetUnit)
            {
                adjacentUnits.Add(tile.occupiedBy);
            }
        }

        return adjacentUnits;
    }
}