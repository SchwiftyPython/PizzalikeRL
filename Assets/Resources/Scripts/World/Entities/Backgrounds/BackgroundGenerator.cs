using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    private const string StartSymbol = "#origin#";

    public  TextAsset ChildhoodFile;
    public  TextAsset ParentStatusFile;
    public  TextAsset LifeEventsFile;

    public static BackgroundGenerator Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    public List<string> GenerateBackground()
    {
        return new List<string> {GenerateChildhood(), GenerateParentStatus()};
    }

    public string GenerateLifeEvent()
    {
        return GenerateText(LifeEventsFile);
    }

    private string GenerateChildhood()
    {
        return GenerateText(ChildhoodFile);
    }

    private string GenerateParentStatus()
    {
        return GenerateText(ParentStatusFile);
    }

    private string GenerateText(TextAsset file)
    {
        try
        {
            var grammar = new TraceryNet.Grammar(new FileInfo(AssetDatabase.GetAssetPath(file)));
            return grammar.Flatten(StartSymbol);
        }
        catch (Exception e)
        {
            Debug.Log("Error opening " + file + "! " + e);
            throw;
        }
    }
}
