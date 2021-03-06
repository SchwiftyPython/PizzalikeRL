﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldData : MonoBehaviour
{
    private Cell[,] _map;
    private string _seed;

    public Dictionary<string, Cell> MapDictionary { get; private set; }
    public Dictionary<Guid, Entity> Entities { get; set; }
    public Dictionary<Guid, Item> Items { get; set; }
    public Dictionary<string, Faction> Factions { get; set; }
    public Dictionary<string, Reputation> EntityGroupRelationships { get; set; }
    public List<Entity> FactionLeaders { get; set; }
    public List<Entity> OtherNamedNpcs { get; set; }
    public Dictionary<int, River> Rivers { get; set; }
    public Dictionary<Toppings, GameObject> WorldViewToppingsDictionary { get; set; }
    public Dictionary<Toppings, GameObject> UiViewToppingsDictionary { get; set; }
    public Cell PlayerStartingPlace;

    public Dictionary<Guid, Grave> Graves { get; set; }

    public Dictionary<string, Settlement> Settlements { get; set; }

    public string Seed
    {
        get { return _seed; }
        set
        {
            _seed = value;
            Random.InitState(_seed.GetHashCode());
        }
    }

    public Cell[,] Map
    {
        get { return _map; }
        set
        {
            _map = value;
            Height = value.GetLength(0);
            Width = value.GetLength(1);
            CreateMapDictionary();
        }
    }

    //Set in inspector
    public int Height;
    public int Width;

    public string SaveGameId { get; set; }

    public TextAsset RawFactionNamesFile;
    public TextAsset MiscPropBlueprintsFile;

    public GameObject WorldGrassLandTile;
    public GameObject WorldDesertTile;
    public GameObject WorldWasteLandTile;
    public GameObject WorldSwampTile;
    public GameObject WorldIceTile;
    public GameObject WorldWaterTile;

    public GameObject[] WorldGrassLandDetailTiles;
    public GameObject[] WorldWoodLandDetailTiles;
    public GameObject[] WorldDesertDetailTiles;
    public GameObject[] WorldWasteLandDetailTiles;
    public GameObject[] WorldSwampDetailTiles;
    public GameObject[] WorldIceDetailTiles;

    public GameObject[] SettlementFloorTiles;
    public GameObject[] SettlementWallTiles;

    public GameObject GrassMountainTile;
    public GameObject SwampMountainTile;
    public GameObject WastelandMountainTile;
    public GameObject SnowMountainTile;
    public GameObject DesertMountainTile;
    public GameObject MountainPassTile;

    public GameObject[] AreaGrassLandTilesCommon;
    public GameObject[] AreaDesertTilesCommon;
    public GameObject[] AreaWastelandTilesCommon;
    public GameObject[] AreaSwampTilesCommon;
    public GameObject[] AreaIceTilesCommon;

    public GameObject[] AreaGrassLandTilesUnCommon;
    public GameObject[] AreaDesertTilesUnCommon;
    public GameObject[] AreaWastelandTilesUnCommon;
    public GameObject[] AreaSwampTilesUnCommon;
    public GameObject[] AreaIceTilesUnCommon;

    public GameObject[] AreaGrassLandTilesRare;
    public GameObject[] AreaDesertTilesRare;
    public GameObject[] AreaWastelandTilesRare;
    public GameObject[] AreaSwampTilesRare;
    public GameObject[] AreaIceTilesRare;

    public GameObject[] WoodenFloorTiles;
    public GameObject[] StoneFloorTiles;

    public GameObject[] WoodenWallTiles;
    public GameObject[] StoneWallTiles;
    public GameObject[] BrickWallTiles;
    public GameObject[] SingleTileBrickWallTiles;

    public GameObject[] GrassDirtPathTiles;
    public GameObject[] DesertAsphaltRoadTiles;
    public GameObject[] SwampDirtPathTiles;
    public GameObject[] IceAsphaltRoadTiles;
    public GameObject[] WastelandDirtPathTiles;

    public GameObject[] GrassWaterTiles;
    public GameObject[] DesertWaterTiles;
    public GameObject[] SwampWaterTiles;
    public GameObject[] IceWaterTiles;
    public GameObject[] WastelandWaterTiles;

    public GameObject[] PizzaToppingsWorldView;
    public GameObject[] PizzaToppingsUiView;
    public GameObject[] ItemPrefabsWorldView;

    public GameObject[] Furniture;
    public GameObject[] GraveyardProps;
    public GameObject[] WheatFieldTiles;
    public GameObject[] LegumeFieldTiles;
    public GameObject[] MiscExteriorProps;
    public GameObject[] Containers;

    public GameObject CheeseTreePrefab;
    public GameObject ConventionalTurretPrefab;
    public GameObject SmallChest;
    public GameObject PizzaOven;

    public GameObject SpiderWebPrefab;
    
    public static WorldData Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Factions = new Dictionary<string, Faction>();
        EntityGroupRelationships = new Dictionary<string, Reputation>();
        FactionLeaders = new List<Entity>();
        OtherNamedNpcs = new List<Entity>();
        Entities = new Dictionary<Guid, Entity>();
        Items = new Dictionary<Guid, Item>();
        Rivers = new Dictionary<int, River>();
        Graves = new Dictionary<Guid, Grave>();
        Settlements = new Dictionary<string, Settlement>();
    }

    private void Start()
    {
        PopulateToppingsDictionaries();
    }

    public void PopulateToppingsDictionaries()
    {
        WorldViewToppingsDictionary = new Dictionary<Toppings, GameObject>
        {
            {Toppings.Bacon, null },
            {Toppings.BellPepper, null },
            {Toppings.Cheese, null },
            {Toppings.Jalapeno, null },
            {Toppings.Mushrooms, null },
            {Toppings.Olives, null },
            {Toppings.Onion, null },
            {Toppings.Pepperoni, null },
            {Toppings.Pineapple, null },
            {Toppings.Sausage, null },
            {Toppings.Tomato, null },
            {Toppings.Wheat, null }
        };

        UiViewToppingsDictionary = new Dictionary<Toppings, GameObject>
        {
            {Toppings.Bacon, null },
            {Toppings.BellPepper, null },
            {Toppings.Cheese, null },
            {Toppings.Jalapeno, null },
            {Toppings.Mushrooms, null },
            {Toppings.Olives, null },
            {Toppings.Onion, null },
            {Toppings.Pepperoni, null },
            {Toppings.Pineapple, null },
            {Toppings.Sausage, null },
            {Toppings.Tomato, null },
            {Toppings.Wheat, null }
        };

        var index = 0;
        foreach (var topping in WorldViewToppingsDictionary.Keys.ToArray())
        {
            WorldViewToppingsDictionary[topping] = PizzaToppingsWorldView[index];
            UiViewToppingsDictionary[topping] = PizzaToppingsUiView[index];
            index++;
        }
    }

    public void CreateMapDictionary()
    {
        MapDictionary = new Dictionary<string, Cell>();

        for (var row = 0; row < Height; row++)
        {
            for (var column = 0; column < Width; column++)
            {
                var currentCell = _map[row, column];

                MapDictionary.Add(currentCell.Id, currentCell);
            }
        }
    }

    public GameObject GetToppingWorldViewSpriteByType(Toppings toppingType)
    {
        return WorldViewToppingsDictionary[toppingType];
    }

    public GameObject GetTileTextureByNameRarityAndBiome(string prefabName, BiomeType biomeType)
    {
        var biomeTiles = new Dictionary<string, GameObject>();

        switch (biomeType)
        {
            case BiomeType.Desert:
            {
                foreach (var tile in AreaDesertTilesCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaDesertTilesUnCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaDesertTilesRare)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in DesertAsphaltRoadTiles)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                break;
            }
            case BiomeType.Grassland:
            case BiomeType.Woodland:
            {
                foreach (var tile in AreaGrassLandTilesCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaGrassLandTilesUnCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaGrassLandTilesRare)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in GrassDirtPathTiles)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                break;
            }
            case BiomeType.Ice:
            {
                foreach (var tile in AreaIceTilesCommon)
                {
                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaIceTilesUnCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaIceTilesRare)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in IceAsphaltRoadTiles)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                break;
            }
            case BiomeType.Swamp:
            {
                foreach (var tile in AreaSwampTilesCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaSwampTilesUnCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaSwampTilesRare)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in SwampDirtPathTiles)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                break;
            }
            case BiomeType.Wasteland:
            {
                foreach (var tile in AreaWastelandTilesCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaWastelandTilesUnCommon)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in AreaWastelandTilesRare)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                foreach (var tile in WastelandDirtPathTiles)
                {
                    if (biomeTiles.ContainsKey(tile.name))
                    {
                        continue;
                    }

                    biomeTiles.Add(tile.name, tile);
                }

                break;
            }
        }

        if (!biomeTiles.ContainsKey(prefabName))
        {
            Debug.Log($@"Could not find tile prefab by name {prefabName} for biome type {biomeType}");

            return biomeTiles.Values.ToArray()[Random.Range(0, biomeTiles.Count)];
        }

        var texture = biomeTiles[prefabName];

        return texture;
    }

    public Dictionary<GameObject, Rarities> GetBiomeTilesForAreaTileDeck(BiomeType biomeType)
    {
        var biomeTiles = new Dictionary<GameObject, Rarities>();
        Dictionary<GameObject, Rarities> tilesToAdd;
        switch (biomeType)
        {
            case BiomeType.Desert:
                tilesToAdd = AddCommonTiles(AreaDesertTilesCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddUnCommonTiles(AreaDesertTilesUnCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddRareTiles(AreaDesertTilesRare);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                return biomeTiles;
            case BiomeType.Grassland:
            case BiomeType.SeasonalForest:
            case BiomeType.TropicalRainforest:
            case BiomeType.Woodland:

                tilesToAdd = AddCommonTiles(AreaGrassLandTilesCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddUnCommonTiles(AreaGrassLandTilesUnCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddRareTiles(AreaGrassLandTilesRare);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                return biomeTiles;
            case BiomeType.Ice:
                tilesToAdd = AddCommonTiles(AreaIceTilesCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddUnCommonTiles(AreaIceTilesUnCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddRareTiles(AreaIceTilesRare);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                return biomeTiles;
            case BiomeType.Swamp:
                tilesToAdd = AddCommonTiles(AreaSwampTilesCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddUnCommonTiles(AreaSwampTilesUnCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddRareTiles(AreaSwampTilesRare);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                return biomeTiles;
            case BiomeType.Wasteland:
                tilesToAdd = AddCommonTiles(AreaWastelandTilesCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddUnCommonTiles(AreaWastelandTilesUnCommon);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                tilesToAdd = AddRareTiles(AreaWastelandTilesRare);

                if (tilesToAdd != null && tilesToAdd.Count > 0)
                {
                    biomeTiles = biomeTiles.Concat(tilesToAdd).ToDictionary(k => k.Key, v => v.Value);
                }

                return biomeTiles;
            default:
                throw new ArgumentOutOfRangeException(nameof(biomeType), biomeType, null);
        }
    }

    private static Dictionary<GameObject, Rarities> AddCommonTiles(IEnumerable<GameObject> tiles)
    {
        return tiles.ToDictionary(tile => tile, tile => Rarities.Common);
    }

    private static Dictionary<GameObject, Rarities> AddUnCommonTiles(IEnumerable<GameObject> tiles)
    {
        return tiles.ToDictionary(tile => tile, tile => Rarities.Uncommon);
    }

    private static Dictionary<GameObject, Rarities> AddRareTiles(IEnumerable<GameObject> tiles)
    {
        return tiles.ToDictionary(tile => tile, tile => Rarities.Rare);
    }
}
