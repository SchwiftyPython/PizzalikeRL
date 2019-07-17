﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingPrefabStore : MonoBehaviour
{
    private enum LoadingSteps
    {
        AddKey,
        Dimensions,
        Template
    }

    private static readonly IDictionary<string, BuildingPrefab> BuildingPrefabs = new Dictionary<string, BuildingPrefab>();

    private static IDictionary<string, GameObject> _allFurniture;

    public static IDictionary<string, GameObject> StoneWallTiles;
    public static IDictionary<string, GameObject> BrickWallTiles;
    public static IDictionary<string, GameObject> WoodenWallTiles;

    public static IDictionary<string, GameObject> StoneDoorTiles;
    public static IDictionary<string, GameObject> BrickDoorTiles;
    public static IDictionary<string, GameObject> WoodenDoorTiles;

    public static List<GameObject> WoodenFloorTiles;
    public static List<GameObject> StoneFloorTiles;

    public static List<IDictionary<string, GameObject>> WallTileTypes;

    public static List<List<GameObject>> FloorTileTypes;


    public static IDictionary<char, string> WallTileKeys = new Dictionary<char, string>
    {
        {'0', "wall_vertical_straight_right" },
        {'1', "wall_vertical_straight_left" },
        {'2', "wall_upper_left_corner" },
        {'3', "wall_upper_right_corner" },
        {'4', "wall_lower_right_corner" },
        {'5', "wall_lower_left_corner" },
        {'6', "wall_horizontal_straight_top" },
        {'7', "wall_horizontal_straight_bottom" }
    };

    public TextAsset BuildingPrefabFile;

	private void Start ()
    {
		LoadPrefabsFromFile();
        PopulateTileDictionaries();
        PopulateFurnitureDictionary();
    }

    private void LoadPrefabsFromFile()
    {
        var rawPrefabInfo = BuildingPrefabFile.text.Split("\r\n"[0]).ToList();

        var currentStep = LoadingSteps.AddKey;

        var currentPreFab = string.Empty;

        var numColumns = 0;
        
        var x = 0;

        foreach (var line in rawPrefabInfo)
        {
            var trimmedLine = line.Trim('\n');
            if (string.IsNullOrEmpty(trimmedLine))
            {
                currentStep = LoadingSteps.AddKey;
                continue;
            }

            if (currentStep == LoadingSteps.AddKey)
            {
                if (BuildingPrefabs.ContainsKey(trimmedLine))
                {
                    Debug.Log("Building template already exists in _buildingPrefabs!");
                    continue;
                }
                BuildingPrefabs.Add(trimmedLine, null);
                currentPreFab = trimmedLine;
                currentStep = LoadingSteps.Dimensions;
                x = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Dimensions)
            {
                var dimensions = trimmedLine.Split(' ');
                var numRows = int.Parse(dimensions[0]);
                numColumns = int.Parse(dimensions[1]);
                BuildingPrefabs[currentPreFab] = new BuildingPrefab(new char[numRows, numColumns]);
                currentStep = LoadingSteps.Template;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                for (var currentColumn = 0; currentColumn < numColumns; currentColumn++)
                {
                    var row = BuildingPrefabs[currentPreFab].Blueprint;

                    row[x, currentColumn] = trimmedLine[currentColumn];
                }
                x++;
            }
        }
    }

    private static void PopulateTileDictionaries()
    {
        StoneWallTiles = new Dictionary<string, GameObject>
        {
            { "wall_vertical_straight_right", null},
            { "wall_vertical_straight_left", null},
            { "wall_upper_left_corner", null},
            { "wall_upper_right_corner", null},
            { "wall_lower_right_corner", null},
            { "wall_lower_left_corner", null},
            { "wall_horizontal_straight_top", null},
            { "wall_horizontal_straight_bottom", null},
            { "wall_horizontal_door_closed", null},
            { "wall_horizontal_door_open", null},
            { "wall_vertical_door_closed", null},
            { "wall_vertical_door_open", null}
        };

        var i = 0;
        foreach (var tile in StoneWallTiles.Keys.ToArray())
        {
            StoneWallTiles[tile] = WorldData.Instance.StoneWallTiles[i];
            i++;
        }

        BrickWallTiles = new Dictionary<string, GameObject>
        {
            { "wall_vertical_straight_right", null},
            { "wall_vertical_straight_left", null},
            { "wall_upper_left_corner", null},
            { "wall_upper_right_corner", null},
            { "wall_lower_right_corner", null},
            { "wall_lower_left_corner", null},
            { "wall_horizontal_straight_top", null},
            { "wall_horizontal_straight_bottom", null},
            { "wall_horizontal_door_closed", null},
            { "wall_horizontal_door_open", null},
            { "wall_vertical_door_closed", null},
            { "wall_vertical_door_open", null}
        };

        i = 0;
        foreach (var tile in BrickWallTiles.Keys.ToArray())
        {
            BrickWallTiles[tile] = WorldData.Instance.BrickWallTiles[i];
            i++;
        }

        WoodenWallTiles = new Dictionary<string, GameObject>
        {
            { "wall_vertical_straight_right", null},
            { "wall_vertical_straight_left", null},
            { "wall_upper_left_corner", null},
            { "wall_upper_right_corner", null},
            { "wall_lower_right_corner", null},
            { "wall_lower_left_corner", null},
            { "wall_horizontal_straight_top", null},
            { "wall_horizontal_straight_bottom", null},
            { "wall_horizontal_door_closed", null},
            { "wall_horizontal_door_open", null},
            { "wall_vertical_door_closed", null},
            { "wall_vertical_door_open", null}
        };

        i = 0;
        foreach (var tile in WoodenWallTiles.Keys.ToArray())
        {
            WoodenWallTiles[tile] = WorldData.Instance.WoodenWallTiles[i];
            i++;
        }

        WoodenFloorTiles = new List<GameObject>();

        foreach (var tile in WorldData.Instance.WoodenFloorTiles)
        {
            WoodenFloorTiles.Add(tile);
        }

        StoneFloorTiles = new List<GameObject>();

        foreach (var tile in WorldData.Instance.StoneFloorTiles)
        {
            StoneFloorTiles.Add(tile);
        }

        WallTileTypes = new List<IDictionary<string, GameObject>>
        {
            StoneWallTiles,
            BrickWallTiles,
            WoodenWallTiles
        };

        FloorTileTypes = new List<List<GameObject>>
        {
            WoodenFloorTiles,
            StoneFloorTiles
        };
    }

    private static void PopulateFurnitureDictionary()
    {
       _allFurniture = new Dictionary<string, GameObject>
       {
           {"bed", null},
           {"bookcase_empty", null},
           {"bookcase_full", null},
           {"chest_1", null},
           {"chest_2", null},
           {"chest_3", null},
           {"couch_horizontal", null},
           {"couch_diagonal", null},
           {"easy_chair", null},
           {"lantern_off", null},
           {"lantern_on", null},
           {"lamp_off", null},
           {"lamp_on", null},
           {"store_counter_clutter_1", null},
           {"store_counter_clutter_2", null},
           {"store_counter_empty_1", null},
           {"store_counter_empty_2", null},
           {"torture_table", null},
           {"torture_iron_maiden", null},
           {"torture_rack_1", null},
           {"torture_rack_2", null},
           {"weapon_rack_1", null},
           {"weapon_rack_2", null},
           {"weapon_rack_empty", null}
       };

        var i = 0;
        foreach (var furniture in _allFurniture.Keys.ToArray())
        {
            _allFurniture[furniture] = WorldData.Instance.Furniture[i];
            i++;
        }
    }

    public static char[,] GetBuildingPrefab(string buildingType)
    {
        return BuildingPrefabs[buildingType].Blueprint;
    }

    public static BuildingPrefab GetBuildingPrefabForLot(Lot lot)
    {
        var buildingsThatWillFit =
            BuildingPrefabs.Values.Where(buildingPrefab => buildingPrefab.WillFitInLot(lot)).ToList();

        return buildingsThatWillFit[Random.Range(0, buildingsThatWillFit.Count)];
    }

    public static int GetRandomWallTypeIndex()
    {
        return Random.Range(0, WallTileTypes.Count);
    }

    public static IDictionary<string, GameObject> GetWallTileTypeAt(int index)
    {
        return WallTileTypes[index];
    }

    public static int GetRandomFloorTypeIndex()
    {
        return Random.Range(0, FloorTileTypes.Count);
    }

    public static List<GameObject> GetFloorTileTypeAt(int index)
    {
        return FloorTileTypes[index];
    }

    public static GameObject GetRandomFurniturePrefab()
    {
        return _allFurniture.ElementAt(Random.Range(0, _allFurniture.Count)).Value;
    }
}
