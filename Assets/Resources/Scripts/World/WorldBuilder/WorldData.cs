using System;
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
    public List<Entity> FactionLeaders { get; set; }
    public List<Entity> OtherNamedNpcs { get; set; }
    public Dictionary<int, River> Rivers { get; set; }

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

    public int Height { get; set; }

    public int Width { get; set; }

    public Guid SaveGameId { get; set; }

    public TextAsset RawFactionNamesFile;

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

    public GameObject[] AreaGrassLandTilesCommon = new GameObject[1];
    public GameObject[] AreaDesertTilesCommon = new GameObject[1];
    public GameObject[] AreaWastelandTilesCommon = new GameObject[1];
    public GameObject[] AreaSwampTilesCommon = new GameObject[1];
    public GameObject[] AreaIceTilesCommon = new GameObject[1];

    public GameObject[] AreaGrassLandTilesUnCommon = new GameObject[1];
    public GameObject[] AreaDesertTilesUnCommon = new GameObject[1];
    public GameObject[] AreaWastelandTilesUnCommon = new GameObject[1];
    public GameObject[] AreaSwampTilesUnCommon = new GameObject[1];
    public GameObject[] AreaIceTilesUnCommon = new GameObject[1];

    public GameObject[] AreaGrassLandTilesRare = new GameObject[1];
    public GameObject[] AreaDesertTilesRare = new GameObject[1];
    public GameObject[] AreaWastelandTilesRare = new GameObject[1];
    public GameObject[] AreaSwampTilesRare = new GameObject[1];
    public GameObject[] AreaIceTilesRare = new GameObject[1];

    public GameObject[] WoodenFloorTiles = new GameObject[1];
    public GameObject[] StoneFloorTiles = new GameObject[1];

    public GameObject[] WoodenWallTiles = new GameObject[1];
    public GameObject[] StoneWallTiles = new GameObject[1];
    public GameObject[] BrickWallTiles = new GameObject[1];
    public GameObject[] SingleTileBrickWallTiles = new GameObject[1];

    public GameObject[] GrassDirtPathTiles = new GameObject[1];
    public GameObject[] DesertAsphaltRoadTiles = new GameObject[1];
    public GameObject[] SwampDirtPathTiles = new GameObject[1];
    public GameObject[] IceAsphaltRoadTiles = new GameObject[1];
    public GameObject[] WastelandDirtPathTiles = new GameObject[1];

    public GameObject[] GrassWaterTiles = new GameObject[1];
    public GameObject[] DesertWaterTiles = new GameObject[1];
    public GameObject[] SwampWaterTiles = new GameObject[1];
    public GameObject[] IceWaterTiles = new GameObject[1];
    public GameObject[] WastelandWaterTiles = new GameObject[1];

    //Todo: This is replaced by deck system
    public Dictionary<BiomeType, string[]> BiomePossibleEntities = new Dictionary<BiomeType, string[]>
    {
        {BiomeType.Desert, new[] {"pepperoni worm"}},
        {BiomeType.Grassland, new[] {"pepperoni worm"}},
        {BiomeType.SeasonalForest, new[] {"pepperoni worm"}},
        {BiomeType.TropicalRainforest, new[] {"pepperoni worm"}},
        {BiomeType.Woodland, new[] {"pepperoni worm"}},
        {BiomeType.Ice, new[] {"pepperoni worm"}},
        {BiomeType.Swamp, new[] {"pepperoni worm"}},
        {BiomeType.Wasteland, new[] {"pepperoni worm"}}
    };
    
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
        FactionLeaders = new List<Entity>();
        OtherNamedNpcs = new List<Entity>();
    }

    public void CreateMapDictionary()
    {
        MapDictionary = new Dictionary<string, Cell>();

        for (var row = 0; row < Height; row++)
        {
            for (var column = 0; column < Width; column++)
            {
                var currentCell = _map[column, row];

                MapDictionary.Add(currentCell.Id, currentCell);
            }
        }
    }

    public Dictionary<GameObject, Rarities> GetBiomeTiles(BiomeType biomeType)
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
