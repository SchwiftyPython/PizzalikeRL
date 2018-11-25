using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FactionTemplateLoader : MonoBehaviour
{
    public const string Path = "factions";
    
    private static List<string> _factionTypes;
    private static FactionTemplateContainer _fc;
    private static List<string> _rawFactionNames;

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
        //todo create actual generation instead of picking from premade list
        var index = Random.Range(0, _rawFactionNames.Count);
        return _rawFactionNames[index].Trim('\n');
    }

    private static void LoadRawFactionNamesFromFile()
    {
        _rawFactionNames = WorldData.Instance.RawFactionNamesFile.text.Split("\r\n"[0]).ToList();
    }
}
