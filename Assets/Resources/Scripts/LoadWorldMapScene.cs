using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWorldMapScene : MonoBehaviour {
    private const string WorldMapScene = "WorldMap";

    public void LoadWorldMap()
    {
        SceneManager.LoadScene(WorldMapScene);
    }
}
