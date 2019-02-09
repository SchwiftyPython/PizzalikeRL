using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadWindowPopup : MonoBehaviour
{
    private static string _selectedSaveGameId;
    private Dictionary<string, string> _saveGameFileInfo;

    public GameObject LoadWindow;
    public GameObject TitleBar;
    public GameObject ActionBar;

    public GameObject SaveGameButtonPrefab;
    public GameObject LoadButton;

    public RectTransform SaveGameButtonParent;

    public static LoadWindowPopup Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        Hide();
        _selectedSaveGameId = string.Empty;
    }

    public void Show()
    {
        LoadWindow.SetActive(true);
        TitleBar.SetActive(true);
        ActionBar.SetActive(true);
        LoadButton.GetComponent<Button>().interactable = false;
        _selectedSaveGameId = string.Empty;

        Populate();
    }

    public void Hide()
    {
        LoadWindow.SetActive(false);
        TitleBar.SetActive(false);
        ActionBar.SetActive(false);
    }

    public static void SaveGameSelected(string id)
    {
        _selectedSaveGameId = string.Copy(id);
    }

    public void LoadGame()
    {
        if (string.IsNullOrEmpty(_selectedSaveGameId))
        {
            return;
        }

        SaveGameData.Instance.Load(_selectedSaveGameId);
    }

    private void Populate()
    {
        _saveGameFileInfo = SaveGameData.Instance.SaveFileNames;

        if (_saveGameFileInfo == null || _saveGameFileInfo.Count < 1)
        {
            LoadButton.GetComponent<Button>().interactable = false;
            return;
        }

        foreach (var saveGameId in _saveGameFileInfo.Keys)
        {
            var saveGameButton = Instantiate(SaveGameButtonPrefab, new Vector3(0, 0), Quaternion.identity);

            saveGameButton.transform.SetParent(SaveGameButtonParent);

            var textFields = saveGameButton.GetComponentsInChildren<Text>(true);

            textFields[0].text = _saveGameFileInfo[saveGameId];
            textFields[1].text = saveGameId;
        }

        LoadButton.GetComponent<Button>().interactable = true;
    }
}
