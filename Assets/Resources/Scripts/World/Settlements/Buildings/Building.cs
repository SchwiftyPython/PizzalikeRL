using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Building
{
    private const int MaxTriesToPlaceObject = 3;

    //todo make list of keys for specific room types

    public GameObject[,] FloorTiles;
    public GameObject[,] WallTiles;
    public GameObject[,] Props;

    public int WallTypeIndex;
    public IDictionary<string, GameObject> WallTilePrefabs;

    public int FloorTypeIndex;
    public List<GameObject> FloorTilePrefabs;

    public int Width;
    public int Height;

    public char[,] Blueprint;

    public Building(BuildingPrefab prefab)
    {
        Height = prefab.Height;
        Width = prefab.Width;

        FloorTiles = new GameObject[Height, Width];
        WallTiles = new GameObject[Height, Width];
        Props = new GameObject[Height, Width];

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
        Furnish();
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

    private void Furnish()
    {
        var maxFurniture = Height * Width / 20;

        var minFurniture = Height * Width / 60;

        if (minFurniture < 1)
        {
            minFurniture = 1;
        }

        var numFurnitureToPlace = Random.Range(minFurniture, maxFurniture);

        var tilesAdjacentToWall = FindAllTilesAdjacentToWall();

        for (var i = 0; i < numFurnitureToPlace; i++)
        {
            var row = Random.Range(0, Height);
            var column = Random.Range(0, Width);

            if (tilesAdjacentToWall[row, column] && Props[row, column] == null)
            {
                //todo pick furniture based on room type

                var furniturePrefab = BuildingPrefabStore.GetRandomFurniturePrefab();

                Props[row, column] = furniturePrefab;
            }
        }
        
        //todo else increment failure, try again

        
    }

    private bool[,] FindAllTilesAdjacentToWall()
    {
        var tilesAdjacentToWall = new bool[Height, Width];

        for (var currentRow = 0; currentRow < Height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < Width; currentColumn++)
            {
                tilesAdjacentToWall[currentRow, currentColumn] = IsFloorTileAdjacentToWall(currentRow, currentColumn);
            }
        }

        return tilesAdjacentToWall;
    }

    private bool IsFloorTileAdjacentToWall(int currentRow, int currentColumn)
    {
        if (IsWall(currentRow, currentColumn))
        {
            return false;
        }

        if (WallNorth(currentRow, currentColumn))
        {
            return true;
        }

        if (WallEast(currentRow, currentColumn))
        {
            return true;
        }

        if (WallSouth(currentRow, currentColumn))
        {
            return true;
        }

        if (WallWest(currentRow, currentColumn))
        {
            return true;
        }

        return false;
    }

    private bool IsDoor(int row, int column)
    {
        var doorComponent = WallTiles[row, column].GetComponent<Door>();

        return doorComponent != null;
    }

    private bool IsWall(int currentRow, int currentColumn)
    {
        return WallTiles[currentRow, currentColumn] != null;
    }

    private bool WallNorth(int currentRow, int currentColumn)
    {
        if (currentRow - 1 < 0)
        {
            return false;
        }

        return IsWall(currentRow - 1, currentColumn) && !IsDoor(currentRow - 1, currentColumn);
    }

    private bool WallEast(int currentRow, int currentColumn)
    {
        if (currentColumn + 1 >= Width)
        {
            return false;
        }

        return IsWall(currentRow, currentColumn + 1) && !IsDoor(currentRow, currentColumn + 1);
    }

    private bool WallSouth(int currentRow, int currentColumn)
    {
        if (currentRow + 1 >= Height)
        {
            return false;
        }

        return IsWall(currentRow + 1, currentColumn) && !IsDoor(currentRow + 1, currentColumn);
    }

    private bool WallWest(int currentRow, int currentColumn)
    {
        if (currentColumn - 1 < 0)
        {
            return false;
        }

        return IsWall(currentRow, currentColumn - 1) && !IsDoor(currentRow, currentColumn - 1);
    }
}
