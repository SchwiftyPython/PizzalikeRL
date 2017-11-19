﻿using System;
using System.Collections.Generic;
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

    public GameObject[] AreaGrassLandTiles = new GameObject[11];
    public GameObject[] AreaDesertTiles = new GameObject[2];
    public GameObject[] AreaWasteLandTiles = new GameObject[4];
    public GameObject[] AreaSwampTiles = new GameObject[9];
    public GameObject[] AreaIceTiles = new GameObject[1];

    //Todo: This is going to need to be an xml file
    public Dictionary<BiomeType, string[]> BiomePossibleEntities = new Dictionary<BiomeType, string[]>() {
        {BiomeType.Desert, new[]{"pepperoni worm"} },
        {BiomeType.Grassland, new[]{"pepperoni worm"} },
        {BiomeType.SeasonalForest, new[]{"pepperoni worm"} },
        {BiomeType.TropicalRainforest, new[]{"pepperoni worm"} },
        {BiomeType.Woodland, new[]{"pepperoni worm"} },
        {BiomeType.Ice, new[]{"pepperoni worm"} },
        {BiomeType.Swamp, new[]{"pepperoni worm"} },
        {BiomeType.WasteLand, new[]{"pepperoni worm"} }
    };

    public static WorldData Instance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    public GameObject[] GetBiomeTiles(BiomeType biomeType){
        switch (biomeType) {
            case BiomeType.Desert:
                return AreaDesertTiles;
            case BiomeType.Grassland:
            case BiomeType.SeasonalForest:
            case BiomeType.TropicalRainforest:
            case BiomeType.Woodland:
                return AreaGrassLandTiles;
            case BiomeType.Ice:
                return AreaIceTiles;
            case BiomeType.Swamp:
                return AreaSwampTiles;
            case BiomeType.WasteLand:
                return AreaWasteLandTiles;
            default:
                throw new ArgumentOutOfRangeException("biomeType", biomeType, null);
        }
    }
}
