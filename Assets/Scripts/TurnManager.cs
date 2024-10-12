using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    private int currentUnitIndex = 0;
    public TargetSelector targetSelector;

    public void InitializeTurnCycle()
    {
        foreach (var unit in units)
        {
            unit.initiative = Random.Range(unit.minInitiative, unit.maxInitiative + 1);
            Debug.Log($"{unit.unitName} бросил инициативу: {unit.initiative}");
        }

        units.Sort((a, b) => b.initiative.CompareTo(a.initiative));
        Debug.Log("Юниты отсортированы по инициативе. Запускаем первый ход.");
        StartNextTurn();
    }

    public void StartNextTurn()
    {
        if (currentUnitIndex >= units.Count)
        {
            EndRound();
            return;
        }

        Unit currentUnit = units[currentUnitIndex];
        Debug.Log($"Ход юнита {currentUnit.unitName}");
        currentUnit.StartTurn();
        targetSelector.currentUnit = currentUnit;
    }

    public void EndCurrentTurn()
    {
        Unit currentUnit = units[currentUnitIndex];
        currentUnit.EndTurn();

        currentUnitIndex++;
        if (currentUnitIndex >= units.Count)
        {
            EndRound();
        }
        else
        {
            StartNextTurn();
        }
    }

    public void EndRound()
    {
        Debug.Log("Раунд завершён. Начинаем новый раунд.");
        currentUnitIndex = 0;

        foreach (var unit in units)
        {
            unit.ResetAP();
        }

        StartNextTurn();
    }
    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        Debug.Log($"{unit.unitName} удалён из боя.");
    }
}
