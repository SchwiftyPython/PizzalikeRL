using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SettlementPrefabStore : MonoBehaviour
{
    private const int NumColumns = 80;
    private const int NumRows = 25;

    private enum LoadingSteps
    {
        NewPrefab,
        Template
    }

    private static readonly IDictionary<SettlementSize, List<SettlementPrefab>> SettlementPrefabs = new Dictionary<SettlementSize, List<SettlementPrefab>>();

    private static List<string> _rawNames;

    public static IDictionary<char, string> GrassDirtPathTileKeys = new Dictionary<char, string>
    {
        {'0', "path_dirt_vertical_straight_left" },
        {'1', "path_dirt_vertical_straight_right" },
        {'2', "path_dirt_corner_upper_left" },
        {'3', "path_dirt_corner_upper_right" },
        {'4', "path_dirt_corner_lower_left" },
        {'5', "path_dirt_corner_lower_right" },
        {'6', "path_dirt_straight_horizontal_bottom" },
        {'7', "path_dirt_straight_horizontal_top" },
        {'8', "path_dirt_intersection_3_way_bottom" },
        {'9', "path_dirt_intersection_3_way_left" },
        {'A', "path_dirt_intersection_3_way_right" },
        {'B', "path_dirt_intersection_3_way_top" },
        {'C', "path_dirt_intersection_4_way_center" }
    };

    public static IDictionary<char, string> DesertAsphaltRoadTileKeys = new Dictionary<char, string>
    {
        {'0', "road_desert_straight_vertical_left" },
        {'1', "road_desert_straight_vertical_right" },
        {'2', "road_desert_corner_upper_left" },
        {'3', "road_desert_corner_upper_right" },
        {'4', "road_desert_corner_lower_left" },
        {'5', "road_desert_corner_lower_right" },
        {'6', "road_desert_straight_horizontal_bottom" },
        {'7', "road_desert_straight_horizontal_top" },
        {'8', "road_desert_intersection_3_way_bottom" },
        {'9', "road_desert_intersection_3_way_left" },
        {'A', "road_desert_intersection_3_way_right" },
        {'B', "road_desert_intersection_3_way_top" },
        {'C', "road_desert_intersection_4_way_center" }
    };

    public static IDictionary<char, string> SwampDirtPathTileKeys = new Dictionary<char, string>
    {
        {'0', "path_dirt_vertical_straight_left" },
        {'1', "path_dirt_vertical_straight_right" },
        {'2', "path_dirt_corner_upper_left" },
        {'3', "path_dirt_corner_upper_right" },
        {'4', "path_dirt_corner_lower_left" },
        {'5', "path_dirt_corner_lower_right" },
        {'6', "path_dirt_straight_horizontal_bottom" },
        {'7', "path_dirt_straight_horizontal_top" },
        {'8', "path_dirt_3_way-bottom" },
        {'9', "path_dirt_3_way-left" },
        {'A', "path_dirt_3_way-right" },
        {'B', "path_dirt_3_way-top" },
        {'C', "path_dirt_4_way-center" }
    };

    public static IDictionary<char, string> WastelandDirtPathTileKeys = new Dictionary<char, string>
    {
        {'0', "path_dirt_vertical_straight_left" },
        {'1', "path_dirt_vertical_straight_right" },
        {'2', "path_dirt_corner_upper_left" },
        {'3', "path_dirt_corner_upper_right" },
        {'4', "path_dirt_corner_lower_left" },
        {'5', "path_dirt_corner_lower_right" },
        {'6', "path_dirt_straight_horizontal_bottom" },
        {'7', "path_dirt_straight_horizontal_top" },
        {'8', "path_dirt_3_way-bottom" },
        {'9', "path_dirt_3_way-left" },
        {'A', "path_dirt_3_way-right" },
        {'B', "path_dirt_3_way-top" },
        {'C', "path_dirt_4_way-center" }
    };

    public static IDictionary<char, string> IceAsphaltRoadTileKeys = new Dictionary<char, string>
    {
        {'0', "road_snow_straight_vertical_left" },
        {'1', "road_snow_straight_vertical_right" },
        {'2', "road_snow_corner_upper_left" },
        {'3', "road_snow_corner_upper_right" },
        {'4', "road_snow_corner_lower_left" },
        {'5', "road_snow_corner_lower_right" },
        {'6', "road_snow_straight_horizontal_bottom" },
        {'7', "road_snow_straight_horizontal_top" },
        {'8', "road_snow_intersection_3_way_bottom" },
        {'9', "road_snow_intersection_3_way_left" },
        {'A', "road_snow_intersection_3_way_right" },
        {'B', "road_snow_intersection_3_way_top" },
        {'C', "road_snow_intersection_4_way_center" }
    };

    public static char LotKey = 'L';

    public static IDictionary<string, GameObject> GrassDirtPathTiles;
    public static IDictionary<string, GameObject> DesertAsphaltRoadTiles;
    public static IDictionary<string, GameObject> SwampDirtPathTiles;
    public static IDictionary<string, GameObject> IceAsphaltRoadTiles;
    public static IDictionary<string, GameObject> WastelandDirtPathTiles;

    public TextAsset SettlementPrefabFile;
    public TextAsset SettlementNames;

    public static IDictionary<SettlementSize, int> SettlementSizePopulationCaps { get; } = new Dictionary<SettlementSize, int>
    {
        { SettlementSize.Outpost, 10 },
        { SettlementSize.Hamlet, 20 },
        { SettlementSize.Village, 50 },
        { SettlementSize.SmallCity, 250 },
        { SettlementSize.Fortress, 500 },
        { SettlementSize.LargeCity, 1000 }
    };

    private void Start ()
    {
		LoadPrefabsFromFile();
        FinishPopulatingPrefabs();
        LoadNamesFromFile();
        PopulateTileDictionaries();
	}

    private void LoadNamesFromFile()
    {
        _rawNames = new List<string>();

        var nameFile = SettlementNames.text.Split("\r\n"[0]).ToList();

        foreach (var line in nameFile)
        {
            var trimmedLine = line.Trim('\n');
            _rawNames.Add(trimmedLine);
        }
    }

    private void LoadPrefabsFromFile()
    {
        var rawPrefabInfo = SettlementPrefabFile.text.Split("\r\n"[0]).ToList();

        var currentStep = LoadingSteps.NewPrefab;

        var currentPreFab = SettlementSize.Outpost;

        var currentRow = 0;

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
                SettlementPrefabs[currentPreFab].Add(new SettlementPrefab(new char[NumRows, NumColumns]));

                currentStep = LoadingSteps.Template;
                currentRow = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                var charArray = trimmedLine.ToCharArray();
                Array.Reverse(charArray);
                trimmedLine = new string(charArray);

                for (var currentColumn = 0; currentColumn < NumColumns; currentColumn++)
                {
                    var row = SettlementPrefabs[currentPreFab].Last().Blueprint;

                    //Debug.Log($"x: {x}  y: {y}");

                    row[currentRow, currentColumn] = trimmedLine[currentColumn];
                }
                currentRow++;
            }
        }
        foreach (var prefabType in SettlementPrefabs.Values)
        { 
            foreach(var prefab in prefabType)
            {
                prefab.Blueprint = Rotate180(prefab.Blueprint);
                FindLotsInBlueprint(prefab);
            }
        }
    }

    public static void AssignBuildingToLots(SettlementPrefab prefab)
    {
        foreach (var lot in prefab.Lots)
        {
            var buildingPrefab = BuildingPrefabStore.GetBuildingPrefabForLot(lot);
            lot.AssignedBuilding = new Building(buildingPrefab); 
        }
    }

    private static void FindLotsInBlueprint(SettlementPrefab prefab)
    {
        var blueprint = prefab.Blueprint;

        for (var currentRow = 0; currentRow < NumRows; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < NumColumns; currentColumn++)
            {
                if (blueprint[currentRow, currentColumn] != LotKey)
                {
                    continue;
                }

                if (prefab.Lots.Count == 0)
                {
                    var lot = GetNewLotInfo(new Vector2(currentRow, currentColumn), blueprint);
                    prefab.Lots.Add(lot);
                }
                else
                {
                    if (IsPartOfExistingLot(new Vector2(currentRow, currentColumn), prefab.Lots))
                    {
                        continue;
                    }

                    var lot = GetNewLotInfo(new Vector2(currentRow, currentColumn), blueprint);
                    prefab.Lots.Add(lot);
                }
            }
        }
    }

    private static bool IsPartOfExistingLot(Vector2 point, List<Lot> lots)
    {
        return lots.Any(lot => lot.IsPartOfLot(point));
    }

    private static Lot GetNewLotInfo(Vector2 upperLeftCorner, char[,] blueprint)
    {
        var x = (int)upperLeftCorner.x;
        var y = (int)upperLeftCorner.y;

        var width = 0;
        var height = 0;

        while (blueprint[x,y] == LotKey)
        {
            width++;
            y++;
        }
        y--;
        x = (int)upperLeftCorner.x;

        while (blueprint[x, y] == LotKey)
        {
            height++;
            x++;
        }

        return new Lot(upperLeftCorner, height, width);
    }

    private void FinishPopulatingPrefabs()
    {
        var sizeObjects = Enum.GetValues(typeof(SettlementSize));

        foreach (var sizeObject in sizeObjects)
        {
            var size = (SettlementSize) sizeObject;

            if (!SettlementPrefabs.ContainsKey(size))
            {
                continue;
            }

            foreach (var prefab in SettlementPrefabs.Keys)
            {
                if (prefab > (int) SettlementSize.Outpost && (int) prefab < (int) size)
                {
                    SettlementPrefabs[size].AddRange(SettlementPrefabs[prefab]);
                }
            }
        }
        Debug.Log("Settlement prefabs populated");
    }

    private static SettlementSize GetSettlementSize(string size)
    {
        switch (size)
        {
            case "outpost":
                return SettlementSize.Outpost;
            case "hamlet":
                return SettlementSize.Hamlet;
            case "village":
                return SettlementSize.Village;
                default:
                    return SettlementSize.Outpost; 
        }
    }

    private static void PopulateTileDictionaries()
    {
        GrassDirtPathTiles = new Dictionary<string, GameObject>
        {
            { "path_dirt_vertical_straight_left", null },
            { "path_dirt_vertical_straight_right", null },
            { "path_dirt_corner_upper_left", null },
            { "path_dirt_corner_upper_right", null },
            { "path_dirt_corner_lower_left", null },
            { "path_dirt_corner_lower_right", null },
            { "path_dirt_straight_horizontal_bottom", null },
            { "path_dirt_straight_horizontal_top", null },
            { "path_dirt_intersection_3_way_bottom", null },
            { "path_dirt_intersection_3_way_left", null },
            { "path_dirt_intersection_3_way_right", null },
            { "path_dirt_intersection_3_way_top", null },
            { "path_dirt_intersection_4_way_center", null }
        };

        SwampDirtPathTiles = new Dictionary<string, GameObject>
        {
            { "path_dirt_vertical_straight_left", null },
            { "path_dirt_vertical_straight_right", null },
            { "path_dirt_corner_upper_left", null },
            { "path_dirt_corner_upper_right", null },
            { "path_dirt_corner_lower_left", null },
            { "path_dirt_corner_lower_right", null },
            { "path_dirt_straight_horizontal_bottom", null },
            { "path_dirt_straight_horizontal_top", null },
            { "path_dirt_3_way-bottom", null },
            { "path_dirt_3_way-left", null },
            { "path_dirt_3_way-right", null },
            { "path_dirt_3_way-top", null },
            { "path_dirt_4_way-center", null }
        };

        WastelandDirtPathTiles = new Dictionary<string, GameObject>
        {
            { "path_dirt_vertical_straight_left", null },
            { "path_dirt_vertical_straight_right", null },
            { "path_dirt_corner_upper_left", null },
            { "path_dirt_corner_upper_right", null },
            { "path_dirt_corner_lower_left", null },
            { "path_dirt_corner_lower_right", null },
            { "path_dirt_straight_horizontal_bottom", null },
            { "path_dirt_straight_horizontal_top", null },
            { "path_dirt_3_way-bottom", null },
            { "path_dirt_3_way-left", null },
            { "path_dirt_3_way-right", null },
            { "path_dirt_3_way-top", null },
            { "path_dirt_4_way-center", null }
        };

        DesertAsphaltRoadTiles = new Dictionary<string, GameObject>
        {
            { "road_desert_straight_vertical_left", null },
            { "road_desert_straight_vertical_right", null },
            { "road_desert_corner_upper_left", null },
            { "road_desert_corner_upper_right", null },
            { "road_desert_corner_lower_left", null },
            { "road_desert_corner_lower_right", null },
            { "road_desert_straight_horizontal_bottom", null },
            { "road_desert_straight_horizontal_top", null },
            { "road_desert_intersection_3_way_bottom", null },
            { "road_desert_intersection_3_way_left", null },
            { "road_desert_intersection_3_way_right", null },
            { "road_desert_intersection_3_way_top", null },
            { "road_desert_intersection_4_way_center", null }

        };

        IceAsphaltRoadTiles = new Dictionary<string, GameObject>
        {
            { "road_snow_straight_vertical_left", null },
            { "road_snow_straight_vertical_right", null },
            { "road_snow_corner_upper_left", null },
            { "road_snow_corner_upper_right", null },
            { "road_snow_corner_lower_left", null },
            { "road_snow_corner_lower_right", null },
            { "road_snow_straight_horizontal_bottom", null },
            { "road_snow_straight_horizontal_top", null },
            { "road_snow_intersection_3_way_bottom", null },
            { "road_snow_intersection_3_way_left", null },
            { "road_snow_intersection_3_way_right", null },
            { "road_snow_intersection_3_way_top", null },
            { "road_snow_intersection_4_way_center", null }
        };

        var i = 0;
        foreach (var tile in GrassDirtPathTiles.Keys.ToArray())
        {
            GrassDirtPathTiles[tile] = WorldData.Instance.GrassDirtPathTiles[i];
            i++;
        }

        i = 0;
        foreach (var tile in DesertAsphaltRoadTiles.Keys.ToArray())
        {
            DesertAsphaltRoadTiles[tile] = WorldData.Instance.DesertAsphaltRoadTiles[i];
            i++;
        }

        i = 0;
        foreach (var tile in SwampDirtPathTiles.Keys.ToArray())
        {
            SwampDirtPathTiles[tile] = WorldData.Instance.SwampDirtPathTiles[i];
            i++;
        }

        i = 0;
        foreach (var tile in WastelandDirtPathTiles.Keys.ToArray())
        {
            WastelandDirtPathTiles[tile] = WorldData.Instance.WastelandDirtPathTiles[i];
            i++;
        }

        i = 0;
        foreach (var tile in IceAsphaltRoadTiles.Keys.ToArray())
        {
            IceAsphaltRoadTiles[tile] = WorldData.Instance.IceAsphaltRoadTiles[i];
            i++;
        }
    }

    public static string GenerateName()
    {
        //todo generate name based on location name or landmark
        var index = Random.Range(0, _rawNames.Count);
        return _rawNames[index];
    }

    public static char[,] Rotate180(char[,] blueprint){
        
        var height = blueprint.GetLength(0);
        var width = blueprint.GetLength(1);
        var answer = new char[height, width];

        for (var row = 0; row < height / 2; row++)
        {
            var topX = row;
            var bottomX = height - 1 - row;
            for (var topY = 0; topY < width; topY++)
            {
                var bottomY = width - topY - 1;
                answer[topX, topY] = blueprint[bottomX, bottomY];
                answer[bottomX, bottomY] = blueprint[topX, topY];
            }
        }

        if (height % 2 == 0)
        {
            return answer;
        }

        var centerX = height / 2;
        for (var leftY = 0; leftY < Mathf.CeilToInt(width / 2f); leftY++)
        {
            var rightY = width - 1 - leftY;
            answer[centerX, leftY] = blueprint[centerX, rightY];
            answer[centerX, rightY] = blueprint[centerX, leftY];
        }
        
        return answer;
    }

    public static SettlementPrefab GetSettlementPrefab(SettlementSize size)
    {
        var prefab = SettlementPrefabs[size][Random.Range(0, SettlementPrefabs[size].Count)];

        return new SettlementPrefab(prefab);
    }
}
