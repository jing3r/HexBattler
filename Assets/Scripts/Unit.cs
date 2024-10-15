using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int health;
    public int maxAP;
    public int currentAP;
    public int movementSpeed = 0;
    public int minInitiative;
    public int maxInitiative;
    public int initiative;
    public float baseAccuracy = 0.5f;
    public float baseEvasion = 0.1f;
    public float accuracy;
    public float evasion;

    public bool isTurnOver = false;
    public Ability[] abilities;
    public TurnManager turnManager;
    public Tile currentTile;
    public int team;
    private MetallicPulsation metallicPulsation;
    private PulsatingEffect pulsatingEffect;

    void Start()
    {
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }

        metallicPulsation = GetComponent<MetallicPulsation>();
        pulsatingEffect = GetComponent<PulsatingEffect>();
        accuracy = baseAccuracy;
        evasion = baseEvasion;
    }

    public void MoveToTile(Tile newTile)
    {
        // if (currentTile != null)
        // {
        //     currentTile.ClearOccupiedBy();
        // }
        currentTile = newTile;
        // currentTile.SetOccupiedBy(this);
        transform.position = newTile.transform.position + Vector3.up;
        ApplyHeightModifiers();
        Debug.Log($"{unitName} переместился на тайл {newTile.name}");
    }
    private void ApplyHeightModifiers()
    {
        accuracy = baseAccuracy;
        evasion = baseEvasion;
        accuracy += currentTile.GetAccuracyModifier();
        evasion += currentTile.GetEvasionModifier();
        Debug.Log($"{unitName}: Текущая точность {accuracy}, Текущее уклонение {evasion}");
    }

    public void StartTurn()
    {
        isTurnOver = false;
        currentAP = maxAP;
        Debug.Log($"{unitName} начинает ход с {currentAP} очками действия.");
        StartCoroutine(ActivatePulsationNextFrame());
    }

    IEnumerator ActivatePulsationNextFrame()
    {
        yield return null;
        if (metallicPulsation != null)
        {
            metallicPulsation.enabled = true;
            metallicPulsation.StartPulsating();
        }
    }

    public void EndTurn()
    {
        if (isTurnOver)
        {
            return;
        }

        isTurnOver = true;
        if (metallicPulsation != null)
        {
            metallicPulsation.enabled = false;
            metallicPulsation.StopPulsating();
            Debug.Log($"{unitName} завершил ход.");
        }

        if (turnManager != null)
        {
            turnManager.EndCurrentTurn();
        }
    }

    public void ResetAP()
    {
        currentAP = maxAP;
        Debug.Log($"{unitName} восстановил AP до {maxAP}.");
    }

    public void ActivateAbility(int abilityIndex)
    {
        if (abilityIndex >= 0 && abilityIndex < abilities.Length)
        {
            Debug.Log($"{unitName} использует способность {abilities[abilityIndex].abilityName}.");
            abilities[abilityIndex].Activate(this);
        }
        else
        {
            Debug.LogError("Неверный индекс способности!");
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{unitName} получил {damage} урона, оставшееся здоровье: {health}.");

        if (health <= 0)
        {
            Die();
        }
    }

    public void ModifyAccuracy(float value)
    {
        accuracy = Mathf.Clamp(accuracy + value, 0f, 1f);
    }

    public void ModifyEvasion(float value)
    {
        evasion = Mathf.Clamp(evasion + value, 0f, 1f);
    }

    public void CheckForEndTurn()
    {
        if (currentAP == 0)
        {
            Debug.Log($"У {unitName} не осталось очков действия. Завершаем ход.");
            EndTurn();
        }
    }

    public void HighlightAsTarget()
    {
        if (pulsatingEffect != null)
        {
            pulsatingEffect.StartPulsating();
        }
    }

    public void RemoveHighlight()
    {
        if (pulsatingEffect != null)
        {
            pulsatingEffect.StopPulsating();
        }
    }
    void Die()
    {
        Debug.Log($"{unitName} погибает.");

        if (turnManager != null)
        {
            turnManager.RemoveUnit(this);
        }

        Destroy(gameObject);
    }

    public void Attack(Unit targetUnit)
    {

        float heightModifier = currentTile.GetHeightModifierForUnit(this, targetUnit);
        float heightEvasionModifier = targetUnit.currentTile.GetHeightModifierForUnit(targetUnit, this);
        float finalAccuracy = accuracy + heightModifier - (targetUnit.evasion + heightEvasionModifier);

        if (Random.value <= finalAccuracy)
        {
            Debug.Log($"{unitName} попадает по {targetUnit.unitName}.");
            targetUnit.TakeDamage(10);
        }
        else
        {
            Debug.Log($"{unitName} промахивается по {targetUnit.unitName}.");
        }
    }
}
