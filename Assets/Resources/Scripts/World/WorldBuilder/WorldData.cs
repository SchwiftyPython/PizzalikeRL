using UnityEngine;

public class WorldData : MonoBehaviour {

    public string Seed { get; set; }

    public Cell[,] Cells { get; set; }

    public GameObject worldGrassLandTile;
    public GameObject worldForestTile;
    public GameObject worldDesertTile;
    public GameObject worldMountainTile;
    public GameObject worldWasteLandTile;
    public GameObject worldSwampTile;
    public GameObject worldIceTile;

    public static WorldData Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            // Destroy the current object, so there is just one 
            Destroy(gameObject);
        }

    }
}
