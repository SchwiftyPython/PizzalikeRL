using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemWindow : MonoBehaviour
{
    private const string MainMenuScene = "MainMenu";

    public Button OptionsButton;
    public Button HelpButton;
    public Button SaveQuitToMenuButton;
    public Button SaveQuitToOsButton;

    public void ShowOptionsWindow()
    {
        //todo
    }

    public void ShowHelpWindow()
    {
        //todo
    }

    public void SaveAndQuitToMenu()
    {
        SaveGameData.Instance.Save();

        GameManager.Instance.WorldMapGenComplete = false;

        GameManager.Instance.CurrentState = GameManager.GameState.Start;

        AreaMap.Instance.Deconstruct();
        
        foreach (Transform child in GameManager.Instance.transform)
        {
            Destroy(child.gameObject);
        }

        SceneManager.LoadScene(MainMenuScene);
    }

    public void SaveAndQuitToOs()
    {
        SaveGameData.Instance.Save();
        Application.Quit();
    }
}
