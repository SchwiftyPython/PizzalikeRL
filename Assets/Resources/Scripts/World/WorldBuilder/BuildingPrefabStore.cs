using System.Collections;
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

    public const string Path = "\\Scripts\\World\\WorldBuilder\\building_prefabs";

    public TextAsset BuildingPrefabFile;

    private IDictionary<string, char[,]> _buildingPrefabs;

	private void Start ()
    {
		LoadPrefabsFromFile();
	}

    private void LoadPrefabsFromFile()
    {
        var rawPrefabInfo = BuildingPrefabFile.text.Split("\n"[0]).ToList();

        var currentStep = LoadingSteps.AddKey;

        var currentPreFab = string.Empty;

        var width = 0;
        
        var y = 0;

        foreach (var line in rawPrefabInfo)
        {
            if (string.IsNullOrEmpty(line))
            {
                currentStep = LoadingSteps.AddKey;
                continue;
            }

            if (currentStep == LoadingSteps.AddKey)
            {
                if (_buildingPrefabs.ContainsKey(line))
                {
                    Debug.Log("Building template already exists in _buildingPrefabs!");
                    return;
                }
                _buildingPrefabs.Add(line, null);
                currentPreFab = line;
                currentStep = LoadingSteps.Dimensions;
                y = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Dimensions)
            {
                var dimensions = line.Split(' ');
                width = int.Parse(dimensions[0]);
                var height = int.Parse(dimensions[1]);
                _buildingPrefabs[currentPreFab] = new char[width, height];
                currentStep = LoadingSteps.Template;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                for (var x = 0; x < width; x++)
                {
                    var row = _buildingPrefabs[currentPreFab];
                    row[x, y] = line[x];
                }
                y++;
            }
        }
    }
	
}
