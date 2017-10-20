using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    private string WorldGenerationScene = "WorldGeneration";

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
        SceneManager.LoadScene(WorldGenerationScene);
    }

    public void QuitToDesktop() {
        Application.Quit();
    }
    
}
