using TMPro;
using UnityEngine;

public class AbilitySelectButton : MonoBehaviour
{
    public void OnClick()
    {
        DisplayAbilityDescription();
    }

    private void DisplayAbilityDescription()
    {
        var abilityName = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

        CharacterCreation.Instance.DisplaySelectedAbilityDescription(abilityName);
    }
}
