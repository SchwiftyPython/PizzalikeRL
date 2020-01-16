using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectButton : MonoBehaviour
{
    public void OnClick()
    {
        SelectAbility();
    }

    private void SelectAbility()
    {
        var abilityName = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        var button = gameObject.GetComponentInChildren<Button>();

        EventMediator.Instance.Broadcast(GlobalHelper.AbilitySelectedEventName, this, button);
    }
}
