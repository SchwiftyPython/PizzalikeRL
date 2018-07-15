using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlementPrefabStore : MonoBehaviour
{
    private enum LoadingSteps
    {
        AddKey,
        Template
    }

    private static readonly IDictionary<string, SettlementPrefab> SettlementPrefabs = new Dictionary<string, SettlementPrefab>();

    public static IDictionary<char, string> PathTileKeys = new Dictionary<char, string>
    {
        {'0', "path_vertical_straight_left" },
        {'1', "path_vertical_straight_right" },
        {'2', "path_upper_left_corner_outside" },
        {'3', "path_upper_right_corner_outside" },
        {'4', "path_lower_right_corner_outside" },
        {'5', "path_lower_left_corner_outside" },
        {'6', "path_upper_left_corner_inside" },
        {'7', "path_upper_right_corner_inside" },
        {'8', "path_lower_right_corner_inside" },
        {'9', "path_lower_left_corner_inside" },
        {'a', "path_horizontal_straight_top" },
        {'b', "path_horizontal_straight_bottom" }
    };

    public static char LotKey = 'L';

    public static IDictionary<string, GameObject> GrassDirtPathTiles;

    public TextAsset SettlementPrefabFile;

    private void Start ()
    {
		LoadPrefabsFromFile();
	}

    private void LoadPrefabsFromFile()
    {
        var rawPrefabInfo = SettlementPrefabFile.text.Split("\r\n"[0]).ToList();

        var currentStep = LoadingSteps.AddKey;

        var currentPreFab = string.Empty;

        const int numColumns = 25;

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
                if (SettlementPrefabs.ContainsKey(trimmedLine))
                {
                    Debug.Log("Settlement template already exists in SettlementPrefabs!");
                    continue;
                }
                SettlementPrefabs.Add(trimmedLine, null);
                currentPreFab = trimmedLine;
                SettlementPrefabs[currentPreFab] = new SettlementPrefab(new char[80, numColumns]);

                currentStep = LoadingSteps.Template;
                x = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                for (var y = 0; y < numColumns; y++)
                {
                    var row = SettlementPrefabs[currentPreFab].Blueprint;

                    Debug.Log($"x: {x}  y: {y}");

                    row[x, y] = trimmedLine[y];
                }
                x++;
            }
        }
    }
}
