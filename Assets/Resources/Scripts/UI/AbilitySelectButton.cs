using TMPro;
using UnityEngine;

public class AbilitySelectButton : MonoBehaviour
{
    public void OnClick()
    {
        SelectAbility();
    }

    private void SelectAbility()
    {
        var abilityName = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        EventMediator.Instance.Broadcast(GlobalHelper.AbilitySelectedEventName, this, abilityName);

        //CharacterCreation.Instance.AbilitySelected(abilityName);
    }
}
