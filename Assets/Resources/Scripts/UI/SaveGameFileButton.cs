using UnityEngine;
using UnityEngine.UI;

public class SaveGameFileButton : MonoBehaviour
{
    public void Pressed()
    {
        var saveGameId = transform.GetComponentsInChildren<Text>(true)[1].text;

        LoadWindowPopup.SaveGameSelected(saveGameId);
    }
}
