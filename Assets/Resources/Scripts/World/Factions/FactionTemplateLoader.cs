using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FactionTemplateLoader : MonoBehaviour
{
    public const string Path = "factions";
    
    private static List<string> _factionTypes;
    private static FactionTemplateContainer _fc;
    private static List<string> _rawFactionNames;

    private static string _fullPath;

    public static void Initialize()
    {
        LoadRawFactionNamesFromFile();

        _fc = FactionTemplateContainer.Load(Path);

        _factionTypes = new List<string>();

        foreach (var s in _fc.FactionTemplates)
        {
            _factionTypes.Add(s.Type.ToLower());
        }
    }

    public static List<string> GetFactionTypes()
    {
        return _factionTypes;
    }

    public static string GetFactionTypeAt(int index)
    {
        return _factionTypes[index];
    }

    public static int GetFcLength()
    {
        return _factionTypes.Count;
    }

    public static FactionTemplate GetFactionByType(string factionType)
    {
        try
        {
            return (from f in _fc.FactionTemplates
                where f.Type.Trim().Equals(factionType.ToLower().Trim())
                select f).SingleOrDefault();
        }
        catch (Exception e)
        {
            Debug.Log("Error Getting Faction By Type: " + e.Message);
            return null;
        }
    }

    public static string GenerateFactionName()
    {
        const string startSymbol = "#origin#";

        try
        {
            var grammar = new TraceryNet.Grammar(new FileInfo(_fullPath));

            var factionName = grammar.Flatten(startSymbol);

            while (WorldData.Instance.Factions.ContainsKey(factionName))
            {
                factionName = grammar.Flatten(startSymbol);
            }

            return factionName;
        }
        catch (Exception e)
        {
            Debug.Log("Error opening " + _fullPath + "! " + e);
            throw;
        }
    }

    private static void LoadRawFactionNamesFromFile()
    {
        const string storyPath = "\\Assets\\Resources\\Scripts\\World\\Factions";

        const string file = "faction_name_tracery.json";

        var basePath = Environment.CurrentDirectory;

        _fullPath = System.IO.Path.Combine(basePath, storyPath.TrimStart('\\', '/'), file);

        _fullPath = System.IO.Path.Combine(storyPath, _fullPath);
    }
}
