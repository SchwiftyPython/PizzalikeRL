using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettlementPrefabStore : MonoBehaviour
{
    private enum LoadingSteps
    {
        NewPrefab,
        Template
    }

    private static readonly IDictionary<SettlementSize, List<SettlementPrefab>> SettlementPrefabs = new Dictionary<SettlementSize, List<SettlementPrefab>>();

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

        var currentStep = LoadingSteps.NewPrefab;

        var currentPreFab = SettlementSize.Outpost;

        const int numColumns = 80;
        const int numRows = 25;

        var x = 0;

        foreach (var line in rawPrefabInfo)
        {
            var trimmedLine = line.Trim('\n');
            if (string.IsNullOrEmpty(trimmedLine))
            {
                currentStep = LoadingSteps.NewPrefab;
                continue;
            }

            if (currentStep == LoadingSteps.NewPrefab)
            {
                currentPreFab = GetSettlementSize(trimmedLine);
                if (!SettlementPrefabs.ContainsKey(currentPreFab))
                {
                    SettlementPrefabs.Add(currentPreFab, new List<SettlementPrefab>());
                }
                SettlementPrefabs[currentPreFab].Add(new SettlementPrefab(new char[numColumns, numRows]));

                currentStep = LoadingSteps.Template;
                x = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                for (var y = 0; y < numRows; y++)
                {
                    var row = SettlementPrefabs[currentPreFab].Last().Blueprint;

                    Debug.Log($"x: {x}  y: {y}");

                    row[x, y] = trimmedLine[y];
                }
                x++;
            }
        }
    }

    private static SettlementSize GetSettlementSize(string size)
    {
        switch (size)
        {
            case "outpost":
                return SettlementSize.Outpost;
                default:
                    return SettlementSize.Outpost; 
        }
    }


    public static SettlementPrefab GetSettlementPrefab(SettlementSize size)
    {
        return SettlementPrefabs[size][Random.Range(0, SettlementPrefabs[size].Count)];
    }
}
