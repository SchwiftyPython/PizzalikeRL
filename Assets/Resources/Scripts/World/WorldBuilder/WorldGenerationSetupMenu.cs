using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldGenerationSetupMenu : MonoBehaviour {

    public InputField SeedInputField;

    private const string WorldGenerationScene = "WorldGeneration";

    public static WorldGenerationSetupMenu Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }      

    }


    public void LoadWorldGeneration() {
        if (SeedInputField.text.Equals("") || SeedInputField.text == null) {
            WorldData.Instance.Seed = (UnityEngine.Random.Range(int.MinValue, int.MaxValue) +
                                       (int) DateTime.Now.Ticks).ToString();
        }
        else {
            WorldData.Instance.Seed = SeedInputField.text;
        }
        SceneManager.LoadScene(WorldGenerationScene);
    }
}
