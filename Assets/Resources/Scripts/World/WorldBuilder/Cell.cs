using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum HeightType {
    DeepWater = 1,
    ShallowWater = 2,
    Shore = 3,
    Sand = 4,
    Grass = 5,
    Forest = 6,
    Rock = 7,
    Snow = 8,
    River = 9
}

public enum HeatType {
    Coldest = 0,
    Colder = 1,
    Cold = 2,
    Warm = 3,
    Warmer = 4,
    Warmest = 5
}

public enum MoistureType {
    Wettest = 5,
    Wetter = 4,
    Wet = 3,
    Dry = 2,
    Dryer = 1,
    Dryest = 0
}

public enum BiomeType {
    Desert,
    TropicalRainforest,
    Grassland,
    Woodland,
    Swamp,
    Mountain,
    SeasonalForest,
    Wasteland,
    Ice,
    Water
}

public class Cell
{
    private const int CellWidth = 3;
    private const int CellHeight = 3;

    public bool HasNPCs;

    public HeightType HeightType;
    public HeatType HeatType;
    public MoistureType MoistureType;
    public BiomeType BiomeType;

    public float HeightValue { get; set; }
    public float HeatValue { get; set; }
    public float MoistureValue { get; set; }
    public int X, Y;
    public string Id;

    public Cell Left;
    public Cell Right;
    public Cell Top;
    public Cell Bottom;

    public int Bitmask;
    public int BiomeBitmask;

    public bool Collidable;
    public bool FloodFilled;

    public List<River> Rivers = new List<River>();
    public Area[,] Areas = new Area[CellHeight, CellWidth];

    public int RiverSize { get; set; }

    public WorldTile WorldMapSprite { get; private set; }

    public List<Faction> PresentFactions;

    public Settlement Settlement;

    public BiomeType biomeType
    {
        get { return BiomeType; }
        set
        {
            BiomeType = value;
            for (var i = 0; i < CellHeight; i++)
            {
                for (var j = 0; j < CellWidth; j++)
                {
                    if (Areas[i, j] != null)
                    {
                        Areas[i, j].BiomeType = value;
                    }
                    else
                    {
                        Areas[i, j] = new Area
                        {
                            ParentCell = this,
                            BiomeType = value,
                            X = i,
                            Y = j,
                            Id = i + " " + j
                        };
                    }
                }
            }
        }
    }

    public CellSdo ConvertToCellSdo()
    {
        return CellSdo.ConvertToCellSdo(this);
    }

    public int GetCellWidth() {
        return CellWidth;
    }

    public int GetCellHeight()
    {
        return CellHeight;
    }

    public void UpdateBitmask() {
        var count = 0;

        if (Top.HeightType == HeightType)
            count += 1;
        if (Right.HeightType == HeightType)
            count += 2;
        if (Bottom.HeightType == HeightType)
            count += 4;
        if (Left.HeightType == HeightType)
            count += 8;

        Bitmask = count;
    }

    public void UpdateBiomeBitmask() {
        var count = 0;

        if (Collidable && Top != null && Top.BiomeType == BiomeType)
            count += 1;
        if (Collidable && Bottom != null && Bottom.BiomeType == BiomeType)
            count += 4;
        if (Collidable && Left != null && Left.BiomeType == BiomeType)
            count += 8;
        if (Collidable && Right != null && Right.BiomeType == BiomeType)
            count += 2;

        BiomeBitmask = count;
    }

    public int GetRiverNeighborCount(River river) {
        var count = 0;
        if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
            count++;
        if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
            count++;
        if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
            count++;
        if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
            count++;
        return count;
    }

    public Direction GetLowestNeighbor() {
        if (Left.HeightValue < Right.HeightValue && Left.HeightValue < Top.HeightValue && Left.HeightValue < Bottom.HeightValue)
            return Direction.Left;
        else if (Right.HeightValue < Left.HeightValue && Right.HeightValue < Top.HeightValue && Right.HeightValue < Bottom.HeightValue)
            return Direction.Right;
        else if (Top.HeightValue < Left.HeightValue && Top.HeightValue < Right.HeightValue && Top.HeightValue < Bottom.HeightValue)
            return Direction.Right;
        else if (Bottom.HeightValue < Left.HeightValue && Bottom.HeightValue < Top.HeightValue && Bottom.HeightValue < Right.HeightValue)
            return Direction.Right;
        else
            return Direction.Bottom;
    }

    public void SetRiverPath(River river) {
        if (!Collidable)
            return;

        if (!Rivers.Contains(river)) {
            Rivers.Add(river);
        }
    }

    private void SetRiverCell(River river) {
        SetRiverPath(river);
        HeightType = HeightType.River;
        HeightValue = 0;
        Collidable = false;

        //Debug.Log($"River exists in Cell {X}, {Y}");
    }

    public void DigRiver(River river, int size) {
        SetRiverCell(river);
        RiverSize = size;

        switch (size)
        {
            case 1:
                Bottom.SetRiverCell(river);
                Right.SetRiverCell(river);
                Bottom.Right.SetRiverCell(river);
                break;
            case 2:
                Bottom.SetRiverCell(river);
                Right.SetRiverCell(river);
                Bottom.Right.SetRiverCell(river);
                Top.SetRiverCell(river);
                Top.Left.SetRiverCell(river);
                Top.Right.SetRiverCell(river);
                Left.SetRiverCell(river);
                Left.Bottom.SetRiverCell(river);
                break;
            case 3:
                Bottom.SetRiverCell(river);
                Right.SetRiverCell(river);
                Bottom.Right.SetRiverCell(river);
                Top.SetRiverCell(river);
                Top.Left.SetRiverCell(river);
                Top.Right.SetRiverCell(river);
                Left.SetRiverCell(river);
                Left.Bottom.SetRiverCell(river);
                Right.Right.SetRiverCell(river);
                Right.Right.Bottom.SetRiverCell(river);
                Bottom.Bottom.SetRiverCell(river);
                Bottom.Bottom.Right.SetRiverCell(river);
                break;
            case 4:
                Bottom.SetRiverCell(river);
                Right.SetRiverCell(river);
                Bottom.Right.SetRiverCell(river);
                Top.SetRiverCell(river);
                Top.Right.SetRiverCell(river);
                Left.SetRiverCell(river);
                Left.Bottom.SetRiverCell(river);
                Right.Right.SetRiverCell(river);
                Right.Right.Bottom.SetRiverCell(river);
                Bottom.Bottom.SetRiverCell(river);
                Bottom.Bottom.Right.SetRiverCell(river);
                Left.Bottom.Bottom.SetRiverCell(river);
                Left.Left.Bottom.SetRiverCell(river);
                Left.Left.SetRiverCell(river);
                Left.Left.Top.SetRiverCell(river);
                Left.Top.SetRiverCell(river);
                Left.Top.Top.SetRiverCell(river);
                Top.Top.SetRiverCell(river);
                Top.Top.Right.SetRiverCell(river);
                Top.Right.Right.SetRiverCell(river);
                break;
        }
    }

    public void LoadCellSprite(WorldTile.LayerPrefabIndexDictionary layerPrefabIndexes)
    {
        if (WorldMapSprite == null)
        {
            WorldMapSprite = new WorldTile();
        }
        WorldMapSprite.LayerPrefabIndexes = layerPrefabIndexes;

        PickBaseLayer();
        LoadDetailLayer();
        LoadSettlementMarkerLayers();
    }

    public void SetCellSprite()
    {
        if (WorldMapSprite == null)
        {
            WorldMapSprite = new WorldTile();
        }

        PickBaseLayer();

        if (biomeType != BiomeType.Mountain && biomeType != BiomeType.Water)
        {
            var detailData = PickDetailLayer(biomeType);

            if (detailData == null)
            {
                return;
            }

            WorldMapSprite.LayerPrefabIndexes[WorldSpriteLayer.Detail] = detailData.First().Key;

            WorldMapSprite.Layers[WorldSpriteLayer.Detail] = detailData.First().Value;
        }
    }

    private void PickBaseLayer()
    {
        switch (biomeType)
        {
            case BiomeType.Mountain:
                PickMountainTile();
                break;
            case BiomeType.Desert:
                WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldDesertTile;
                //assign river layer
                //If connecting cell between settlements or just have road set road layer
                break;
            case BiomeType.Grassland:
                WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldGrassLandTile;
                break;
            case BiomeType.Ice:
                WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldIceTile;
                break;
            case BiomeType.Swamp:
                WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldSwampTile;
                break;
            case BiomeType.Wasteland:
                WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldWasteLandTile;
                break;
            case BiomeType.Water:
                WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldWaterTile;
                break;
            case BiomeType.SeasonalForest:
            case BiomeType.TropicalRainforest:
            case BiomeType.Woodland:
                WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldGrassLandTile;
                break;
        }
    }

    private static Dictionary<int, GameObject> PickDetailLayer(BiomeType biome)
    {
        var detailChance = new Dictionary<BiomeType, int>
        {
            {BiomeType.Desert, 25 },
            {BiomeType.Grassland, 28 },
            {BiomeType.Ice, 40 },
            {BiomeType.Swamp, 63 },
            {BiomeType.Wasteland, 24 },
            {BiomeType.Woodland, 100 }
        };

        var roll = Random.Range(1, 101);

        if (roll <= detailChance[biome])
        {
            int index;
            GameObject detail;
            switch (biome)
            {
                case BiomeType.Desert:
                    index = roll % WorldData.Instance.WorldDesertDetailTiles.Length;
                    detail = WorldData.Instance.WorldDesertDetailTiles[index];
                    return new Dictionary<int, GameObject>{{index, detail}};
                case BiomeType.Grassland:
                    index = roll % WorldData.Instance.WorldGrassLandDetailTiles.Length;
                    detail = WorldData.Instance.WorldGrassLandDetailTiles[index];
                    return new Dictionary<int, GameObject> { { index, detail } };
                case BiomeType.Ice:
                    index = roll % WorldData.Instance.WorldIceDetailTiles.Length;
                    detail = WorldData.Instance.WorldIceDetailTiles[index];
                    return new Dictionary<int, GameObject> { { index, detail } };
                case BiomeType.Swamp:
                    index = roll % WorldData.Instance.WorldSwampDetailTiles.Length;
                    detail = WorldData.Instance.WorldSwampDetailTiles[index];
                    return new Dictionary<int, GameObject> { { index, detail } };
                case BiomeType.Wasteland:
                    index = roll % WorldData.Instance.WorldWasteLandDetailTiles.Length;
                    detail = WorldData.Instance.WorldWasteLandDetailTiles[index];
                    return new Dictionary<int, GameObject> { { index, detail } };
                case BiomeType.SeasonalForest:
                case BiomeType.TropicalRainforest:
                case BiomeType.Woodland:
                    index = roll % WorldData.Instance.WorldWoodLandDetailTiles.Length;
                    detail = WorldData.Instance.WorldWoodLandDetailTiles[index];
                    return new Dictionary<int, GameObject> { { index, detail } };
            }
        }
        return null;
    }

    private void LoadDetailLayer()
    {
        var index = WorldMapSprite.LayerPrefabIndexes[WorldSpriteLayer.Detail];

        GameObject detail = null;
        switch (biomeType)
        {
            case BiomeType.Desert:
                detail = WorldData.Instance.WorldDesertDetailTiles[index];
                break;
            case BiomeType.Grassland:
                detail = WorldData.Instance.WorldGrassLandDetailTiles[index];
                break;
            case BiomeType.Ice:
                detail = WorldData.Instance.WorldIceDetailTiles[index];
                break;
            case BiomeType.Swamp:
                detail = WorldData.Instance.WorldSwampDetailTiles[index];
                break;
            case BiomeType.Wasteland:
                detail = WorldData.Instance.WorldWasteLandDetailTiles[index];
                break;
            case BiomeType.SeasonalForest:
            case BiomeType.TropicalRainforest:
            case BiomeType.Woodland:
                detail = WorldData.Instance.WorldWoodLandDetailTiles[index];
                break;
        }

        WorldMapSprite.Layers[WorldSpriteLayer.Detail] = detail;
    }

    private void LoadSettlementMarkerLayers()
    {
        if (Settlement == null)
        {
            return;
        }

        var floorIndex = WorldMapSprite.LayerPrefabIndexes[WorldSpriteLayer.SettlementFloor];

        WorldMapSprite.Layers[WorldSpriteLayer.SettlementFloor] =
            WorldData.Instance.SettlementFloorTiles[floorIndex];

        var wallIndex = WorldMapSprite.LayerPrefabIndexes[WorldSpriteLayer.SettlementWall];

        WorldMapSprite.Layers[WorldSpriteLayer.SettlementWall] =
            WorldData.Instance.SettlementWallTiles[wallIndex];
    }

    private void PickMountainTile()
    {
        var mountainTiles = new Dictionary<BiomeType, GameObject>
        {
            {BiomeType.Grassland, WorldData.Instance.GrassMountainTile},
            {BiomeType.SeasonalForest, WorldData.Instance.GrassMountainTile},
            {BiomeType.Woodland, WorldData.Instance.GrassMountainTile},
            {BiomeType.TropicalRainforest, WorldData.Instance.GrassMountainTile},
            {BiomeType.Desert, WorldData.Instance.DesertMountainTile},
            {BiomeType.Ice, WorldData.Instance.SnowMountainTile},
            {BiomeType.Wasteland, WorldData.Instance.WastelandMountainTile},
            {BiomeType.Swamp, WorldData.Instance.SwampMountainTile}
        };
        
        Cell neighbor = null;
        var maxTries = 4;
        while (maxTries > 0 && (neighbor == null || !mountainTiles.ContainsKey(neighbor.biomeType)))
        {
            var cellNumber = Random.Range(0, 3);

            switch (cellNumber)
            {
                case 0:
                    neighbor = Bottom;
                    break;
                case 1:
                    neighbor = Left;
                    break;
                case 2:
                    neighbor = Top;
                    break;
                case 3:
                    neighbor = Right;
                    break;
                default:
                    neighbor = Bottom;
                    break;
            }
            maxTries--;
        }

        if (maxTries <= 0 && neighbor == null)
        {
            WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldGrassLandTile;
            WorldMapSprite.Layers[WorldSpriteLayer.Mountain] = mountainTiles[BiomeType.Grassland];
        }
        else
        {
            if (neighbor == null)
            {
                return;
            }

            switch (neighbor.biomeType)
            {
                case BiomeType.Mountain:
                    WorldMapSprite.Layers[WorldSpriteLayer.Base] = neighbor.WorldMapSprite.Layers[WorldSpriteLayer.Base];
                    WorldMapSprite.Layers[WorldSpriteLayer.Mountain] = neighbor.WorldMapSprite.Layers[WorldSpriteLayer.Mountain];
                    break;
                case BiomeType.Desert:
                    WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldDesertTile;
                    break;
                case BiomeType.Ice:
                    WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldIceTile;
                    break;
                case BiomeType.Swamp:
                    WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldSwampTile;
                    break;
                case BiomeType.Wasteland:
                    WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldWasteLandTile;
                    break;
                case BiomeType.Grassland:
                case BiomeType.SeasonalForest:
                case BiomeType.TropicalRainforest:
                case BiomeType.Woodland:
                    WorldMapSprite.Layers[WorldSpriteLayer.Base] = WorldData.Instance.WorldGrassLandTile;
                    break;
            }
            if (neighbor.biomeType != BiomeType.Mountain)
            {
                WorldMapSprite.Layers[WorldSpriteLayer.Mountain] = mountainTiles[neighbor.biomeType];
            }
        }
    }
}


