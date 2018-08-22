using UnityEngine;
using UnityEngine.UI;

public class SpeciesOptionButton : MonoBehaviour
{
    public void DisplaySpeciesDescription()
    {
        var optionClicked = EntityTemplateLoader.GetEntityTemplate(transform.GetComponentsInChildren<Text>()[0].text);

        CharacterCreation.Instance.SelectSpeciesOption(optionClicked);
    }
}
