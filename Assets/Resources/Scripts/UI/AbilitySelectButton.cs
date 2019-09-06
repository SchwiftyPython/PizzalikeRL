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

        CharacterCreation.Instance.AbilitySelected(abilityName);
    }
}
