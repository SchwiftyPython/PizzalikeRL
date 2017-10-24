using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private const string WorldGenerationSetupScene = "WorldGenerationSetup";

    public static MainMenu instance = null;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            // Destroy the current object, so there is just one 
            Destroy(gameObject);
        }

    }
    
    public void LoadWorldGenerationScene() {
        SceneManager.LoadScene(WorldGenerationSetupScene);
    }

    public void QuitToDesktop() {
        Application.Quit();
    }
    
}
