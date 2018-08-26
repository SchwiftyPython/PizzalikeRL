using UnityEngine;
using UnityEngine.UI;

public class CharacterBackgroundOptionButton : MonoBehaviour
{
    public void DisplayCharacterBackgroundDescription()
    {
        var optionClicked = CharacterBackgroundLoader.GetCharacterBackground(transform.GetComponentsInChildren<Text>()[0].text);

        CharacterCreation.Instance.SelectCharacterBackgroundOption(optionClicked);
    }
}
