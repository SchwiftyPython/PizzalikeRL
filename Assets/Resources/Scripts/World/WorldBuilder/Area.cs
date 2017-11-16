using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {
    private const int Width = 80;
    private const int Height = 25;

    private Transform _areaMapHolder;
    private Tile[,] _areaTiles;
    private GameObject[] _biomeTypeTiles;

    public BiomeType BiomeType { get; set; }

    public Tile[,] AreaTiles {
        get {
            return _areaTiles;
        }

        set {
            _areaTiles = value;
        }
    }

    private void BuildArea() {
        if (_areaTiles != null) return;
        _areaMapHolder = GameObject.Find("AreaMapHolder").transform;
        _areaTiles = new Tile[Height, Width];
        _biomeTypeTiles = WorldData.Instance.GetBiomeTiles(BiomeType);
        for (var i = 0; i < Height; i++) {
            for (var j = 0; j < Width; j++) {
                var texture = _biomeTypeTiles[Random.Range(0, _biomeTypeTiles.Length)];
                _areaTiles[i,j] = new Tile(texture, new Vector2(i,j), false, false);
                var instance = Instantiate(texture, new Vector2(i, j), Quaternion.identity);
                instance.transform.SetParent(_areaMapHolder);
            }
        }
    }
    
}
