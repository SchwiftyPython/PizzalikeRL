using System.Collections.Generic;
using UnityEngine;

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
    WasteLand,
    Ice,
    Water
}

public class Cell {
    private const int CellWidth = 3;
    private const int CellHeight = 3;

    private Area _currentArea;

    public bool HasNPCs;

    public HeightType HeightType;
    public HeatType HeatType;
    public MoistureType MoistureType;
    public BiomeType BiomeType;

    public float HeightValue { get; set; }
    public float HeatValue { get; set; }
    public float MoistureValue { get; set; }
    public int X, Y;

    public Cell Left;
    public Cell Right;
    public Cell Top;
    public Cell Bottom;

    public int Bitmask;
    public int BiomeBitmask;

    public bool Collidable;
    public bool FloodFilled;

    public Color Color = Color.black;

    public List<River> Rivers = new List<River>();
    public Area[,] Areas = new Area[CellHeight, CellWidth];

    public int RiverSize { get; set; }

    public GameObject WorldMapSprite { get; set; }

    public List<Faction> PresentFaction;

    public Settlement Settlement;

    public BiomeType biomeType
    {
        get { return BiomeType; }
        set
        {
            BiomeType = value;
            SetCellSprite(value);
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
                            Y = j
                        };
                    }
                }
            }
        }
    }

    public Area CurrentArea {
        get {
            return _currentArea;
        }

        set {
            _currentArea = value;
        }
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

    private void SetCellSprite(BiomeType biome) {
        switch (biome) {
            case BiomeType.Mountain:
                WorldMapSprite = WorldData.Instance.WorldMountainTile;
                break;
            case BiomeType.Desert:
                WorldMapSprite = WorldData.Instance.WorldDesertTile;
                break;
            case BiomeType.Grassland:
                WorldMapSprite = WorldData.Instance.WorldGrassLandTile;
                break;
            case BiomeType.Ice:
                WorldMapSprite = WorldData.Instance.WorldIceTile;
                break;
            case BiomeType.Swamp:
                WorldMapSprite = WorldData.Instance.WorldSwampTile;
                break;
            case BiomeType.WasteLand:
                WorldMapSprite = WorldData.Instance.WorldWasteLandTile;
                break;
            case BiomeType.Water:
                WorldMapSprite = WorldData.Instance.WorldWaterTile;
                break;
            case BiomeType.SeasonalForest:
            case BiomeType.TropicalRainforest:
            case BiomeType.Woodland:
                WorldMapSprite = WorldData.Instance.WorldForestTile;
                break;
//            default:
//                throw new ArgumentOutOfRangeException("biome", biome, null);
        }
    }

}


