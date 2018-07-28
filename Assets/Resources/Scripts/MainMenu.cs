using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const string WorldGenerationSetupScene = "WorldGenerationSetup";
    private const string CharacterCreationScene = "CharacterCreation";

    public static MainMenu Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadWorldGenerationScene()
    {
        SceneManager.LoadScene(WorldGenerationSetupScene);
    }

    public void LoadCharacterCreationScene()
    {
        SceneManager.LoadScene(CharacterCreationScene);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
    
}
