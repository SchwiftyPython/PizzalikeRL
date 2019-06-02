using TMPro;
using UnityEngine;

public class CharacterBackgroundOptionButton : MonoBehaviour
{
    public void DisplayCharacterBackgroundDescription()
    {
        var optionClicked = CharacterBackgroundLoader.GetCharacterBackground(transform.GetComponentsInChildren<TextMeshProUGUI>()[0].text);

        CharacterCreation.Instance.SelectCharacterBackgroundOption(optionClicked);
    }
}
