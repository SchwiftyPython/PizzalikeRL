using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionTemplateLoader : MonoBehaviour {

    public const string Path = "factions";
    private static List<string> _factionNames;
    private static FactionTemplateContainer _fc;

    public static void Initialize()
    {
        _fc = FactionTemplateContainer.Load(Path);

        _factionNames = new List<string>();

        foreach (var s in _fc.FactionTemplates)
        {
            _factionNames.Add(s.Name.ToLower());
        }
    }

    public static List<string> GetFactionNames()
    {
        return _factionNames;
    }

    public static string GetFactionNameAt(int index)
    {
        return _factionNames[index];
    }

    public static int GetFcLength()
    {
        return _factionNames.Count;
    }

    public static FactionTemplate GetFactionByName(string factionName)
    {
        try
        {
            return (from f in _fc.FactionTemplates
                where f.Name.Trim().Equals(factionName.ToLower().Trim())
                select f).SingleOrDefault();
        }
        catch (Exception e)
        {
            Debug.Log("Error Getting Faction By Name: " + e.Message);
            return null;
        }
    }
}
