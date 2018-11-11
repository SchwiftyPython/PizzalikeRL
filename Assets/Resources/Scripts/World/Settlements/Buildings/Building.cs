using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public GameObject[,] FloorTiles;
    public GameObject[,] WallTiles;

    public IDictionary<string, GameObject> WallTilePrefabs;
    public List<GameObject> FloorTilePrefabs;

    public int Width;
    public int Height;

    public Building(BuildingPrefab prefab)
    {
        Width = prefab.Width;
        Height = prefab.Height;

        FloorTiles = new GameObject[prefab.Height, prefab.Width];
        WallTiles = new GameObject[prefab.Height, prefab.Width];

        PickTilePrefabs();

        for (var x = 0; x < prefab.Height; x++)
        {
            for (var y = 0; y < prefab.Width; y++)
            {
                var tileCode = prefab.Blueprint[x, y];

                if (BuildingPrefabStore.WallTileKeys.ContainsKey(tileCode))
                {
                    var tile = GetRandomFloorTilePrefab();

                    FloorTiles[x, y] = tile;

                    var tileType = BuildingPrefabStore.WallTileKeys[tileCode];
                    tile = WallTilePrefabs[tileType];

                    WallTiles[x, y] = tile;
                }
                else
                {
                    if (tileCode != 'a')
                    {
                        continue;
                    }

                    var tile = GetRandomFloorTilePrefab();

                    FloorTiles[x, y] = tile;
                }
            }
        }
    }

    private void PickTilePrefabs()
    {
        WallTilePrefabs = BuildingPrefabStore.GetRandomWallTileType();
        FloorTilePrefabs = BuildingPrefabStore.GetRandomFloorTileType();
    }

    private GameObject GetRandomFloorTilePrefab()
    {
        return FloorTilePrefabs[Random.Range(0, FloorTilePrefabs.Count)];
    }
}
