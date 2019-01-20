using System.Collections.Generic;
using UnityEngine;

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

    public Building(BuildingPrefab prefab)
    {
        Width = prefab.Width;
        Height = prefab.Height;

        FloorTiles = new GameObject[prefab.Height, prefab.Width];
        WallTiles = new GameObject[prefab.Height, prefab.Width];

        PickTilePrefabs();

        for (var currentRow = 0; currentRow < prefab.Height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < prefab.Width; currentColumn++)
            {
                var tileCode = prefab.Blueprint[currentRow, currentColumn];

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
}
