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

    private IDictionary<char, string> _tileKeys = new Dictionary<char, string>
    {
        {'0', "wall_vertical_straight" },
        {'1', "wall_horizontal_straight" },
        {'2', "wall_upper_left_corner" },
        {'3', "wall_upper_right_corner" },
        {'4', "wall_lower_right_corner" },
        {'5', "wall_lower_left_corner" },
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

    private readonly IDictionary<string, char[,]> _buildingPrefabs = new Dictionary<string, char[,]>();

    public const string Path = "\\Scripts\\World\\WorldBuilder\\building_prefabs";

    public TextAsset BuildingPrefabFile;

	private void Start ()
    {
		LoadPrefabsFromFile();
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
	
}
