using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
public abstract class Ability : ScriptableObject
{
    public string abilityName = "New Ability";
    public string description;
    public int costAP;
    public bool usableWhileEngaged = true;
    public bool isInstant = true;
    public float range = 1f;
    public bool alwaysHits = true;

    public virtual bool CheckHit(Unit attacker, Unit target)
    {
        if (alwaysHits) return true;
        float chanceToHit = attacker.accuracy - target.evasion;
        float roll = Random.Range(0f, 1f);

        return roll <= chanceToHit;
    }
    protected bool IsInRange(Vector3 unitPosition, Vector3 targetPosition, float additionalRange = 0.5f)
    {
        float distance = Vector3.Distance(unitPosition, targetPosition);
        return distance <= (range + additionalRange); // увеличение радиуса применения на полтайла, т.к. точка отсчёта - центр тайла
    }

    public abstract void Activate(Unit unit);
}