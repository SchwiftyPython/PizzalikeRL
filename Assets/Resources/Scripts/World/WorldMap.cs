using UnityEngine;

public class WorldMap : MonoBehaviour {

    private Transform _mapHolder;
    private Cell[,] _map;
    private int _mapHeight;
    private int _mapWidth;

    public static WorldMap Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
        }
        DrawMap();
    }

    private void DrawMap() {
        _mapHolder = transform;
        _map = WorldData.Instance.Map;
        _mapHeight = WorldData.Instance.Height;
        _mapWidth = WorldData.Instance.Width;

        for (var x = 0; x < _mapWidth; x++) {
            for (var y = 0; y < _mapHeight; y++) {
                var instance = Instantiate(_map[x,y].WorldMapSprite, new Vector2(x,y), Quaternion.identity);
                instance.transform.SetParent(_mapHolder);
            }
        }
    }
}
