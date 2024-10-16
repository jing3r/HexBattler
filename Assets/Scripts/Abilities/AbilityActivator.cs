using UnityEngine;

public class AbilityActivator : MonoBehaviour
{
    public TargetSelector targetSelector;

    void Start()
    {
        if (targetSelector == null)
        {
            targetSelector = FindObjectOfType<TargetSelector>();
        }
    }


    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;
            if (Input.GetKeyDown(key))
            {
                ActivateAbilityAtIndex(i);
            }
        }
        
    void ActivateAbilityAtIndex(int index)
    {
        if (targetSelector == null || targetSelector.currentUnit == null) return;

        if (index >= 0 && index < targetSelector.currentUnit.abilities.Length)
        {
            var selectedAbility = targetSelector.currentUnit.abilities[index];

            if (targetSelector.selectedAbility == selectedAbility)
            {
                targetSelector.CancelTargeting();
            }
            else
            {
                targetSelector.SetSelectedAbility(selectedAbility);
            }
        }
    }   
    }

    void ActivateAbilityAtIndex(int index)
    {
        Unit currentUnit = targetSelector.currentUnit;
        if (currentUnit == null || currentUnit.abilities.Length <= index)
        {
            Debug.LogError("Невозможно активировать способность: нет юнита или неправильный индекс.");
            return;
        }

        Ability ability = currentUnit.abilities[index];

        if (ability != null && ability.currentUses > 0)
        {
            Debug.Log($"{currentUnit.unitName} активирует способность {ability.abilityName}.");
            targetSelector.selectedAbility = ability;
            ability.Activate(currentUnit);
        }
        else
        {
            Debug.Log("Способность недоступна или закончились заряды.");
        }
    }
}