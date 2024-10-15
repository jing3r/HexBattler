using UnityEngine;
using System.Collections.Generic;

public class GameEndManager : MonoBehaviour
{
    public TurnManager turnManager;

    private int team1CapturePoints = 0;
    private int team2CapturePoints = 0;
    private int requiredCapturePoints = 3;

    void Start()
    {
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }
    }

    public void CheckWinConditions()
    {
        if (CheckTeamWipedOut())
        {
            return; 
        }
        CheckFlagCapture();
    }

    private bool CheckTeamWipedOut()
    {
        int team1Count = 0;
        int team2Count = 0;

        foreach (var unit in turnManager.units)
        {
            if (unit.team == 1)
            {
                team1Count++;
            }
            else if (unit.team == 2)
            {
                team2Count++;
            }
        }

        if (team1Count == 0)
        {
            Debug.Log("Команда 2 побеждает!");
            EndGame(2);
            return true;
        }
        else if (team2Count == 0)
        {
            Debug.Log("Команда 1 побеждает!");
            EndGame(1);
            return true;
        }

        return false;
    }

    private void CheckFlagCapture()
    {
        int team1OnFlagTiles = GetUnitsOnFlagTiles(1);
        int team2OnFlagTiles = GetUnitsOnFlagTiles(2);

        if (team1OnFlagTiles > 0 && team2OnFlagTiles == 0)
        {
            UpdateCapturePoints(1);
            Debug.Log($"Команда 1 удерживает флаг. Очки: {team1CapturePoints}");
        }
        else if (team2OnFlagTiles > 0 && team1OnFlagTiles == 0)
        {
            UpdateCapturePoints(2);
            Debug.Log($"Команда 2 удерживает флаг. Очки: {team2CapturePoints}");
        }
        else if (team1OnFlagTiles > 0 && team2OnFlagTiles > 0)
        {
            Debug.Log("Захват приостановлен: на тайлах с флагом находятся юниты обеих команд.");
        }
        else
        {
            Debug.Log("Тайлы с флагом пусты. Очки захвата не изменены.");
        }

        // Проверка на достижение требуемых очков захвата
        if (team1CapturePoints >= requiredCapturePoints)
        {
            Debug.Log("Команда 1 побеждает, захватив флаг!");
            EndGame(1);
        }
        else if (team2CapturePoints >= requiredCapturePoints)
        {
            Debug.Log("Команда 2 побеждает, захватив флаг!");
            EndGame(2);
        }
    }

    private void UpdateCapturePoints(int capturingTeam)
    {
        if (capturingTeam == 1)
        {
            if (team2CapturePoints > 0)
            {
                team2CapturePoints--;
            }
            else
            {
                team1CapturePoints++;
            }
        }
        else if (capturingTeam == 2)
        {
            if (team1CapturePoints > 0)
            {
                team1CapturePoints--;
            }
            else
            {
                team2CapturePoints++;
            }
        }

        Debug.Log($"Очки команды 1: {team1CapturePoints}, Очки команды 2: {team2CapturePoints}");
    }

    private int GetUnitsOnFlagTiles(int teamId)
    {
        int count = 0;

        foreach (var unit in turnManager.units)
        {
            if (unit.team == teamId && unit.currentTile != null && unit.currentTile.isFlagTile)
            {
                count++;
            }
        }

        return count;
    }

    private void EndGame(int winningTeam)
    {
        Debug.Log($"Игра завершена! Победила команда {winningTeam}");
        Time.timeScale = 0;
    }

    public void ResetPoints()
    {
        team1CapturePoints = 0;
        team2CapturePoints = 0;
        Debug.Log("Очки команд сброшены.");
    }
}
