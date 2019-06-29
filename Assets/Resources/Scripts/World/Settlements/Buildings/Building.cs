﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Building
{
    public GameObject[,] FloorTiles;
    public GameObject[,] WallTiles;

    public int WallTypeIndex;
    public IDictionary<string, GameObject> WallTilePrefabs;

    public int FloorTypeIndex;
    public List<GameObject> FloorTilePrefabs;

    public int Width;
    public int Height;

    public char[,] Blueprint;

    public Building(BuildingPrefab prefab)
    {
        Width = prefab.Width;
        Height = prefab.Height;

        FloorTiles = new GameObject[Height, Width];
        WallTiles = new GameObject[Height, Width];

        PickTilePrefabs();

        Blueprint = prefab.Blueprint;

        Build();
    }

    public Building(BuildingSdo sdo)
    {
        Width = sdo.Width;
        Height = sdo.Height;
        
        FloorTiles = new GameObject[Height, Width];
        WallTiles = new GameObject[Height, Width];
        WallTypeIndex = sdo.WallTypeIndex;
        FloorTypeIndex = sdo.FloorTypeIndex;

        WallTilePrefabs = BuildingPrefabStore.GetWallTileTypeAt(WallTypeIndex);
        FloorTilePrefabs = BuildingPrefabStore.GetFloorTileTypeAt(FloorTypeIndex);

        Blueprint = BuildingSdo.ConvertBlueprintForLoading(sdo.Blueprint);

        Build();
    }

    private void Build()
    {
        for (var currentRow = 0; currentRow < Height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < Width; currentColumn++)
            {
                var tileCode = Blueprint[currentRow, currentColumn];

                if (BuildingPrefabStore.WallTileKeys.ContainsKey(tileCode))
                {
                    var tile = GetRandomFloorTilePrefab();

                    FloorTiles[currentRow, currentColumn] = tile;

                    var tileType = BuildingPrefabStore.WallTileKeys[tileCode];
                    tile = WallTilePrefabs[tileType];

                    WallTiles[currentRow, currentColumn] = tile;
                }
                else
                {
                    if (tileCode != 'a')
                    {
                        continue;
                    }

                    var tile = GetRandomFloorTilePrefab();

                    FloorTiles[currentRow, currentColumn] = tile;
                }
            }
        }

        PlaceExteriorDoorOnRandomSide();
    }

    private void PickTilePrefabs()
    {
        WallTypeIndex = BuildingPrefabStore.GetRandomWallTypeIndex();
        WallTilePrefabs = BuildingPrefabStore.GetWallTileTypeAt(WallTypeIndex);

        FloorTypeIndex = BuildingPrefabStore.GetRandomFloorTypeIndex();
        FloorTilePrefabs = BuildingPrefabStore.GetFloorTileTypeAt(FloorTypeIndex);
    }

    private GameObject GetRandomFloorTilePrefab()
    {
        return FloorTilePrefabs[Random.Range(0, FloorTilePrefabs.Count)];
    }

    private void PlaceExteriorDoorOnRandomSide()
    {
        var sides = new List<string>
        {
            "north",
            "south",
            "east",
            "west"
        };

        var side = sides[Random.Range(0, sides.Count)];

        var targetRow = 0;
        var targetColumn = 0;
        GameObject doorPrefab = null;

        switch (side)
        {
            case "north":
                targetRow = 0;
                targetColumn = Random.Range(1, Width - 2);
                doorPrefab = WallTilePrefabs["wall_horizontal_door_closed"];
                break;
            case "south":
                targetRow = Height - 1;
                targetColumn = Random.Range(1, Width - 2);
                doorPrefab = WallTilePrefabs["wall_horizontal_door_closed"];
                break;
            case "east":
                targetRow = Random.Range(1, Height - 2);
                targetColumn = Width - 1;
                doorPrefab = WallTilePrefabs["wall_vertical_door_closed"];
                break;
            case "west":
                targetRow = Random.Range(1, Height - 2);
                targetColumn = 0;
                doorPrefab = WallTilePrefabs["wall_vertical_door_closed"];
                break;
        }

        WallTiles[targetRow, targetColumn] = doorPrefab;
    }
}
