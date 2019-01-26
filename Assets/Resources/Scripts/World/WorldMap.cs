﻿using UnityEngine;

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
        //Destroy(AreaMap.Instance?.Camera);
        DrawMap();
        PlacePlayer();
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

        GameManager.Instance.Player.CurrentPosition =
            new Vector3(GameManager.Instance.CurrentCell.X, GameManager.Instance.CurrentCell.Y);

        //_playerSprite.transform.position = GameManager.Instance.Player.CurrentPosition;
    }
}
