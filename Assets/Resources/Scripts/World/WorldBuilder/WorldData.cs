using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldData : MonoBehaviour {
    private int _height;
    private int _width;
    private Cell[,] _map;

    public string Seed { get; set; }

    public Cell[,] Map {
        get { return _map; }
        set {
            _map = value;
            Height = value.GetLength(0);
            Width = value.GetLength(1);
        }
    }

    public int Height {
        get {
            return _height;
        }

        set {
            _height = value;
        }
    }

    public int Width {
        get {
            return _width;
        }

        set {
            _width = value;
        }
    }

    public GameObject WorldGrassLandTile;
    public GameObject WorldForestTile;
    public GameObject WorldDesertTile;
    public GameObject WorldMountainTile;
    public GameObject WorldWasteLandTile;
    public GameObject WorldSwampTile;
    public GameObject WorldIceTile;
    public GameObject WorldWaterTile;

    public static WorldData Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }
}
