using UnityEngine;

[CreateAssetMenu(fileName = "New Movement Ability", menuName = "Abilities/Movement")]
public class MovementAbility : TileTargetedAbility
{
    public int defaultMovementSpeed;

    public override void Activate(Unit unit)
    {
        FindObjectOfType<TileSelector>().selectedAbility = this;
    }


public override void ActivateOnTile(Unit unit, Tile targetTile)
{
    int speed = unit.movementSpeed > 0 ? unit.movementSpeed : defaultMovementSpeed;

    if (unit.currentTile == targetTile)
    {
        Debug.Log($"{unit.unitName} уже находится на этом тайле. Перемещение невозможно.");
        return;
    }

    if (targetTile.IsPassable() && targetTile.occupiedBy == null)
    {
        float distance = Vector3.Distance(unit.transform.position, targetTile.transform.position);
        if (distance <= speed + 0.5f) // допуск в половину тайла
        {
            if (unit.currentAP >= costAP)
            {
                unit.currentAP -= costAP;
                Debug.Log($"{unit.unitName} переместился на тайл {targetTile.name}");
                unit.MoveToTile(targetTile);

                if (unit.currentAP == 0)
                {
                    unit.EndTurn();
                }
            }
            else
            {
                Debug.Log($"{unit.unitName} не хватает очков действия для движения");
            }
        }
        else
        {
            Debug.Log("Тайл вне радиуса движения " + unit.unitName);
        }
    }
    else
    {
        Debug.Log("Тайл либо занят, либо непроходим. Перемещение невозможно.");
    }
}
}
