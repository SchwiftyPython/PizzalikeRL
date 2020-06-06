using Pathfinding;
using UnityEngine;

public class WorldMap : MonoBehaviour
{

    private Transform _worldMapHolder;
    private Cell[,] _map;
    private int _mapHeight;
    private int _mapWidth;

    private GameObject _playerSprite;

    public GameObject Camera;
    public GameObject CellInfoWindow;

    public static WorldMap Instance;

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
        
        DrawMap();
        PlacePlayer();
    }

    private void Update()
    {
        if (GameManager.Instance.Player == null || _playerSprite == null)
        {
            InstantiatePlayerSprite();
        }
    }

    private void DrawMap()
    {
        _worldMapHolder = transform;
        _map = WorldData.Instance.Map;
        _mapHeight = WorldData.Instance.Height;
        _mapWidth = WorldData.Instance.Width;

        for (var row = 0; row < _mapHeight; row++)
        {
            for (var column = 0; column < _mapWidth; column++)
            {
                foreach (var layer in _map[row, column].WorldMapSprite.Layers.Keys)
                {
                    var prefab = _map[row, column].WorldMapSprite.Layers[layer];

                    if (prefab == null)
                    {
                        continue;
                    }
                    var instance = Instantiate(prefab, new Vector2(column, row), Quaternion.identity);
                    instance.transform.SetParent(_worldMapHolder);
                    instance.AddComponent<WorldTileInfo>();
                    instance.AddComponent<BoxCollider2D>();
                }
            }
        }
    }

    private void PlacePlayer()
    {
        _playerSprite = GameManager.Instance.Player.GetSprite();

        if (_playerSprite == null)
        {
            InstantiatePlayerSprite();
        }
        else
        {
            GameManager.Instance.Player.CurrentPosition =
                new Vector3(GameManager.Instance.CurrentCell.X, GameManager.Instance.CurrentCell.Y);
        }
    }

    public void InstantiatePlayerSprite()
    {
        var existingPlayerSprite = GameObject.FindWithTag("Player");

        if (existingPlayerSprite != null)
        {
            Destroy(existingPlayerSprite);
        }

        GameManager.Instance.Player.CurrentCell = GameManager.Instance.CurrentCell;
        GameManager.Instance.Player.CurrentArea = GameManager.Instance.CurrentCell.Areas[1, 1];

        _playerSprite = Instantiate(GameManager.Instance.Player.GetSpritePrefab(), GameManager.Instance.Player.CurrentPosition, Quaternion.identity);
        _playerSprite.transform.SetParent(GameManager.Instance.transform);
        _playerSprite.tag = "Player";
        GameManager.Instance.Player.SetSprite(_playerSprite);
        GameManager.Instance.Player.GetSprite().AddComponent<Seeker>();
        GameManager.Instance.Player.GetSprite().AddComponent<AstarAI>();
    }
}
