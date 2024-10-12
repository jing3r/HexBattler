using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Ability", menuName = "Abilities/Defense")]
public class DefenseAbility : Ability
{
    public float evasionBonus = 0.2f;

    public override void Activate(Unit unit)
    {
        Debug.Log(unit.unitName + " активирует защиту. Выберите юнита для активации способности.");
        FindObjectOfType<TargetSelector>().selectedAbility = this;
    }

    public void ActivateOnUnit(Unit unit, Unit targetUnit)
    {
        if (unit == targetUnit)
        {
            Debug.Log($"{targetUnit.unitName} активировал защиту: повышение уклонения на {evasionBonus * 100}%.");

            targetUnit.ModifyEvasion(evasionBonus);

            unit.currentAP = 0;
            unit.EndTurn();
        }
        else
        {
            Debug.Log("Нужно выбрать этого же юнита для активации защиты.");
        }
    }
}
