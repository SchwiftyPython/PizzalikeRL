using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    private const string StartSymbol = "#origin#";

    //private const string Path = "\\Scripts\\World\\Entities\\Backgrounds";

    private const string ChildhoodFileName = "\\Assets\\Resources\\Scripts\\World\\Entities\\Backgrounds\\childhood.json";
    private const string ParentStatusFileName = "\\Assets\\Resources\\Scripts\\World\\Entities\\Backgrounds\\parentstatus.json";
    private const string LifeEventsFileName = "\\Assets\\Resources\\Scripts\\World\\Entities\\Backgrounds\\life_events.json";

    public TextAsset ChildhoodFile;
    public TextAsset ParentStatusFile;
    public TextAsset LifeEventsFile;

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

//        var basePath = Environment.CurrentDirectory;
//
//        var fullPath = Path.Combine(basePath, ChildhoodFileName.TrimStart('\\', '/'));
//
//        ChildhoodFile = Resources.Load<TextAsset>(fullPath);
//
//        fullPath = Path.Combine(basePath, ParentStatusFileName.TrimStart('\\', '/'));
//        ParentStatusFile = Resources.Load<TextAsset>(fullPath);
//
//        fullPath = Path.Combine(basePath, LifeEventsFileName.TrimStart('\\', '/'));
//        LifeEventsFile = Resources.Load<TextAsset>(fullPath);
    }


    public List<string> GenerateBackground()
    {
        return new List<string> {GenerateChildhood(), GenerateParentStatus()};
    }

    public string GenerateLifeEvent()
    {
        var basePath = Environment.CurrentDirectory;
        var fullPath = Path.Combine(basePath, LifeEventsFileName.TrimStart('\\', '/'));
        return GenerateText(LifeEventsFile);
    }

    private string GenerateChildhood()
    {
        var basePath = Environment.CurrentDirectory;
        var fullPath = Path.Combine(basePath, ChildhoodFileName.TrimStart('\\', '/'));
        return GenerateText(ChildhoodFile);
    }

    private string GenerateParentStatus()
    {
        var basePath = Environment.CurrentDirectory;
        var fullPath = Path.Combine(basePath, ParentStatusFileName.TrimStart('\\', '/'));
        return GenerateText(ParentStatusFile);
    }

    private static string GenerateText(TextAsset file)
    {
        try
        {
            var grammar = new TraceryNet.Grammar(file.ToString());
            return grammar.Flatten(StartSymbol);
        }
        catch (Exception e)
        {
            Debug.Log("Error opening " + file + "! " + e);
            throw;
        }
    }
}
