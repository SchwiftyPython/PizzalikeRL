using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public GameObject[,] FloorTiles;
    public GameObject[,] WallTiles;

    public int Width;
    public int Height;

    public Building(BuildingPrefab prefab)
    {
        Width = prefab.Width;
        Height = prefab.Height;

        FloorTiles = new GameObject[prefab.Height,prefab.Width];
        WallTiles = new GameObject[prefab.Height, prefab.Width];

        for (var x = 0; x < prefab.Height; x++)
        {
            for (var y = 0; y < prefab.Width; y++)
            {
                var tileCode = prefab.Blueprint[x, y];

                if (BuildingPrefabStore.WallTileKeys.ContainsKey(tileCode))
                {
                    var tileType = BuildingPrefabStore.FloorTileKeys[tileCode];

                    var tile = BuildingPrefabStore.WoodenFloorTiles[tileType];

                    FloorTiles[x, y] = tile;

                    tileType = BuildingPrefabStore.WallTileKeys[tileCode];
                    tile = BuildingPrefabStore.BrownStoneWallTiles[tileType];

                    WallTiles[x, y] = tile;
                }
                else
                {
                    if (tileCode != 'a')
                    {
                        continue;
                    }

                    var tileType = BuildingPrefabStore.FloorTileKeys[tileCode];

                    var tile = BuildingPrefabStore.WoodenFloorTiles[tileType];

                    FloorTiles[x, y] = tile;
                }
            }
        }
    }
}
