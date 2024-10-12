using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Attack Ability", menuName = "Abilities/MeleeAttack")]
public class MeleeAttackAbility : Ability
{
    public int damage = 5;

    public float RangeWithOffset => range + 0.5f;
    public MeleeAttackAbility()
    {
        alwaysHits = false;
    }
    public override void Activate(Unit unit)
    {
        Debug.Log(unit.unitName + " готовится к ближней атаке. Выберите врага.");
        FindObjectOfType<TargetSelector>().selectedAbility = this;
    }

    public void ActivateOnUnit(Unit unit, Unit targetUnit)
    {
        if (unit.team == targetUnit.team)
        {
            Debug.Log("Невозможно атаковать союзника.");
            return;
        }

        if (IsInRange(unit.transform.position, targetUnit.transform.position, 0.5f))
        {
            Debug.Log($"{unit.unitName} атакует {targetUnit.unitName} в ближнем бою и наносит {damage} урона.");
            targetUnit.TakeDamage(damage);
            unit.currentAP -= costAP;

            if (unit.currentAP == 0)
            {
                unit.EndTurn();
            }
        }
        else
        {
            Debug.Log("Цель слишком далеко для ближней атаки.");
        }
    }
}