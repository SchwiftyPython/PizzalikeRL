using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldGenerationSetupMenu : MonoBehaviour {

    public InputField SeedInputField;

    private const string WorldGenerationScene = "WorldGeneration";

    public static WorldGenerationSetupMenu instance = null;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            // Destroy the current object, so there is just one 
            Destroy(gameObject);
        }      

    }


    public void LoadWorldGeneration() {
        if(SeedInputField.text.Equals("") || SeedInputField.text == null) {
            WorldGenData.instance.Seed = (UnityEngine.Random.Range(int.MinValue, int.MaxValue) + 
                                         (int)DateTime.Now.Ticks).ToString();
        }

        SceneManager.LoadScene(WorldGenerationScene);
    }
    
}
