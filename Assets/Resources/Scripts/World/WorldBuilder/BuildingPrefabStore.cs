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

    private static readonly IDictionary<string, char[,]> _buildingPrefabs = new Dictionary<string, char[,]>();

    public static IDictionary<char, string> TileKeys = new Dictionary<char, string>
    {
        {'0', "wall_vertical_straight_right" },
        {'1', "wall_vertical_straight_left" },
        {'2', "wall_upper_left_corner" },
        {'3', "wall_upper_right_corner" },
        {'4', "wall_lower_right_corner" },
        {'5', "wall_lower_left_corner" },
        {'6', "wall_horizontal_straight" },
        {'a', "floor_center" },
        {'b', "floor_left_side" },
        {'c', "floor_right_side" },
        {'d', "floor_upper_center" },
        {'e', "floor_lower_center" },
        {'f', "floor_upper_left_corner" },
        {'g', "floor_upper_right_corner" },
        {'h', "floor_lower_right_corner" },
        {'i', "floor_lower_left_corner" }
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

        var height = 0;
        
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
                if (_buildingPrefabs.ContainsKey(trimmedLine))
                {
                    Debug.Log("Building template already exists in _buildingPrefabs!");
                    return;
                }
                _buildingPrefabs.Add(trimmedLine, null);
                currentPreFab = trimmedLine;
                currentStep = LoadingSteps.Dimensions;
                x = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Dimensions)
            {
                var dimensions = trimmedLine.Split(' ');
                var width = int.Parse(dimensions[0]);
                height = int.Parse(dimensions[1]);
                _buildingPrefabs[currentPreFab] = new char[width, height];
                currentStep = LoadingSteps.Template;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                for (var y = 0; y < height; y++)
                {
                    var row = _buildingPrefabs[currentPreFab];
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
            { "wall_horizontal_straight", null}
        };
        
        WoodenFloorTiles = new Dictionary<string, GameObject>
        {
            {"floor_center", null },
            {"floor_left_side", null },
            {"floor_right_side", null },
            {"floor_upper_center", null },
            {"floor_lower_center", null },
            {"floor_upper_left_corner", null },
            {"floor_upper_right_corner", null },
            {"floor_lower_right_corner", null },
            {"floor_lower_left_corner", null }
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
        return _buildingPrefabs[buildingType];
    }
	
}
