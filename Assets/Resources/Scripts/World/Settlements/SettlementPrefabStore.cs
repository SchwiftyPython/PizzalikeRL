﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        PopulateTileDictionaries();
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
                SettlementPrefabs[currentPreFab].Add(new SettlementPrefab(new char[numRows, numColumns]));

                currentStep = LoadingSteps.Template;
                x = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                var charArray = trimmedLine.ToCharArray();
                Array.Reverse(charArray);
                trimmedLine = new string(charArray);
                for (var y = 0; y < numColumns; y++)
                {
                    var row = SettlementPrefabs[currentPreFab].Last().Blueprint;

                    //Debug.Log($"x: {x}  y: {y}");

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

    private void PopulateTileDictionaries()
    {
        GrassDirtPathTiles = new Dictionary<string, GameObject>
        {
            { "path_vertical_straight_left", null },
            { "path_vertical_straight_right", null },
            { "path_upper_left_corner_outside", null },
            { "path_upper_right_corner_outside", null },
            { "path_lower_right_corner_outside", null },
            { "path_lower_left_corner_outside", null },
            { "path_upper_left_corner_inside", null },
            { "path_upper_right_corner_inside", null },
            { "path_lower_right_corner_inside", null },
            { "path_lower_left_corner_inside", null },
            { "path_horizontal_straight_top", null },
            { "path_horizontal_straight_bottom", null }
        };

        var i = 0;
        foreach (var tile in GrassDirtPathTiles.Keys.ToArray())
        {
            GrassDirtPathTiles[tile] = WorldData.Instance.GrassDirtPathTiles[i];
            i++;
        }
    }

    public static char[,] Rotate180(char[,] blueprint){
        
        var height = blueprint.GetLength(0);
        var width = blueprint.GetLength(1);
        var answer = new char[height, width];

        for (var y = 0; y < height / 2; y++)
        {
            var topY = y;
            var bottomY = height - 1 - y;
            for (var topX = 0; topX < width; topX++)
            {
                var bottomX = width - topX - 1;
                answer[topY, topX] = blueprint[bottomY, bottomX];
                answer[bottomY, bottomX] = blueprint[topY, topX];
            }
        }

        if (height % 2 == 0)
        {
            return answer;
        }

        var centerY = height / 2;
        for (var leftX = 0; leftX < Mathf.CeilToInt(width / 2f); leftX++)
        {
            var rightX = width - 1 - leftX;
            answer[centerY, leftX] = blueprint[centerY, rightX];
            answer[centerY, rightX] = blueprint[centerY, leftX];
        }
        
        return answer;
    }


    public static SettlementPrefab GetSettlementPrefab(SettlementSize size)
    {
        return SettlementPrefabs[size][Random.Range(0, SettlementPrefabs[size].Count)];
    }
}
