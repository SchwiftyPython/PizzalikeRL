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

    public TextAsset FactionNames;

    public static FactionTemplateLoader Instance;

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
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    public void Initialize()
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

    public string GenerateFactionName()
    {
        const string startSymbol = "#origin#";

        try
        {
            //var grammar = new TraceryNet.Grammar(new FileInfo(_fullPath));

            var grammar = new TraceryNet.Grammar(FactionNames.text);

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

    //todo can we skip using file path?
    private static void LoadRawFactionNamesFromFile()
    {
        // const string storyPath = "\\Assets\\Resources\\Scripts\\World\\Factions";
        //
        // const string file = "faction_name_tracery.json";
        //
        // var basePath = Environment.CurrentDirectory;
        //
        // _fullPath = System.IO.Path.Combine(basePath, storyPath.TrimStart('\\', '/'), file);
        //
        // _fullPath = System.IO.Path.Combine(storyPath, _fullPath);
    }
}
