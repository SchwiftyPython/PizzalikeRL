using TMPro;
using UnityEngine;

public class CharacterBackgroundOptionButton : MonoBehaviour
{
    public void DisplayCharacterBackgroundDescription()
    {
        var optionClicked = CharacterBackgroundLoader.GetCharacterBackground(transform.GetComponentsInChildren<TextMeshProUGUI>()[0].text);

        CharacterCreation.Instance.SelectButton(gameObject);

        CharacterCreation.Instance.SelectCharacterBackgroundOption(optionClicked);
    }
}
