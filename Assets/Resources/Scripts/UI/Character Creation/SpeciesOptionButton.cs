using TMPro;
using UnityEngine;

public class SpeciesOptionButton : MonoBehaviour
{
    public void DisplaySpeciesDescription()
    {
        var optionClicked =
            EntityTemplateLoader.GetEntityTemplate(transform.GetComponentInChildren<TextMeshProUGUI>().text);

        CharacterCreation.Instance.SelectSpeciesOption(optionClicked);
    }
}
