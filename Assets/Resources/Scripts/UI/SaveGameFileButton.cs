using TMPro;
using UnityEngine;

public class SaveGameFileButton : MonoBehaviour
{
    public void Pressed()
    {
        var saveGameId = transform.GetComponentsInChildren<TextMeshProUGUI>(true)[1].text;

        LoadWindowPopup.SaveGameSelected(saveGameId);
    }
}
