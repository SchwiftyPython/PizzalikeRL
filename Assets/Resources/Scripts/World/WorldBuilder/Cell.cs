﻿using System.Collections.Generic;
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
    Savanna,
    TropicalRainforest,
    Grassland,
    Woodland,
    SeasonalForest,
    TemperateRainforest,
    BorealForest,
    Tundra,
    Ice
}

public class Cell {
    private const int CellWidth = 3;
    private const int CellHeight = 3;

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
    public Area[,] Areas = new Area[CellWidth, CellHeight];

    public int RiverSize { get; set; }

    /*
    public Cell() {
        for (var i = 0; i < CellWidth; i++) {
            for (var j = 0; j < CellHeight; j++) {
                Areas[i,j] = new Area();
                Debug.Log(i + " " + j);
            }
        }
    }
    */

    public BiomeType biomeType {
        get { return BiomeType; }
        set {
            BiomeType = value;
            for(var i = 0; i < CellWidth; i++) {
                for(var j = 0; j < CellHeight; j++){
                    if (Areas[i, j] != null) {
                        Areas[i, j].biomeType = value;
                    }
                    else {
                        Areas[i, j] = new Area {biomeType = value};
                    }
                }
            }
        }
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

}


