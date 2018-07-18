using System.Collections.Generic;
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

    public static IDictionary<char, string> FloorTileKeys = new Dictionary<char, string>
    {
        {'a', "floor_center" },
        {'0', "floor_right_side" },
        {'1', "floor_left_side" },
        {'2', "floor_upper_left_corner" },
        {'3', "floor_upper_right_corner" },
        {'4', "floor_lower_right_corner" },
        {'5', "floor_lower_left_corner" },
        {'6', "floor_upper_center" },
        {'7', "floor_lower_center" }
    };

    public static IDictionary<string, GameObject> BrownStoneWallTiles;

    public static IDictionary<string, GameObject> WoodenFloorTiles;

    public TextAsset BuildingPrefabFile;

	private void Start ()
    {
		LoadPrefabsFromFile();
        PopulateTileDictionaries();
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
                numColumns = int.Parse(dimensions[0]);
                var numRows = int.Parse(dimensions[1]);
                BuildingPrefabs[currentPreFab] = new BuildingPrefab(new char[numRows, numColumns]);
                currentStep = LoadingSteps.Template;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                for (var y = 0; y < numColumns; y++)
                {
                    var row = BuildingPrefabs[currentPreFab].Blueprint;

                    row[x, y] = trimmedLine[y];
                }
                x++;
            }
        }
    }

    private static void PopulateTileDictionaries()
    {
        BrownStoneWallTiles = new Dictionary<string, GameObject>
        {
            { "wall_vertical_straight_right", null},
            { "wall_vertical_straight_left", null},
            { "wall_upper_left_corner", null},
            { "wall_upper_right_corner", null},
            { "wall_lower_right_corner", null},
            { "wall_lower_left_corner", null},
            { "wall_horizontal_straight_top", null},
            { "wall_horizontal_straight_bottom", null}
        };
        
        WoodenFloorTiles = new Dictionary<string, GameObject>
        {
            {"floor_center", null },
            {"floor_right_side", null },
            {"floor_left_side", null },
            {"floor_upper_left_corner", null },
            {"floor_upper_right_corner", null },
            {"floor_lower_right_corner", null },
            {"floor_lower_left_corner", null },
            {"floor_upper_center", null },
            {"floor_lower_center", null }
        };

        var i = 0;
        foreach (var tile in BrownStoneWallTiles.Keys.ToArray())
        {
            BrownStoneWallTiles[tile] = WorldData.Instance.BrownStoneWallTiles[i];
            i++;
        }

        i = 0;
        foreach (var tile in WoodenFloorTiles.Keys.ToArray())
        {
            WoodenFloorTiles[tile] = WorldData.Instance.WoodenFloorTiles[i];
            i++;
        }

    }

    public static char[,] GetBuildingPrefab(string buildingType)
    {
        return BuildingPrefabs[buildingType].Blueprint;
    }
	
}
