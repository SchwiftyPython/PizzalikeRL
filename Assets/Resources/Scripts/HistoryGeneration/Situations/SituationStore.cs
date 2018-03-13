using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SituationStore
{
    private readonly Dictionary<string, Action> _allSituations = new Dictionary<string, Action>
    {
        {"heretic nation", HereticNation}
    };

    private List<string> _startSituations;
    private List<string> _middleSituations;
    private List<string> _endSituations;

    private const string StartSituationFile = @"start_situations.csv";
    private const string MiddleSituationFile = @"middle_situations.csv";
    private const string EndSituationFile = @"end_situations.csv";

    public void Initialize()
    {
        _startSituations = GetSituationsFromFile(StartSituationFile);
        //_middleSituations = GetSituationsFromFile(MiddleSituationFile);
        //_endSituations = GetSituationsFromFile(EndSituationFile);
    }

    public List<string> GetSituationsOfType(string situationType)
    {
        switch (situationType.ToLower())
        {
            case "start":
                return _startSituations;
            case "middle":
                return _middleSituations;
            case "end":
                return _endSituations;
            default:
                return null;
        }
    }

    public void RunSituation(string situation)
    {
        _allSituations[situation]();
    }

    private static List<string> GetSituationsFromFile(string file)
    {
        var situatons = new List<string>();
        try
        {
            using (var reader = new StreamReader(file))
            {
                string line;
                while (null != (line = reader.ReadLine()))
                {
                    situatons.AddRange(line.Split(',')); 
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error processing file: " + file + " " + e.Message);
        }
        return situatons;
    }

    private static void HereticNation()
    {
        if (WorldData.Instance.Factions.Count < 2)
        {
            return;
        }

        var unselectedFactions = WorldData.Instance.Factions.Values.ToList();

        var index = Random.Range(0, unselectedFactions.Count);
        var factionA = unselectedFactions[index];
        unselectedFactions.Remove(unselectedFactions[index]);

        index = Random.Range(0, unselectedFactions.Count);
        var factionB = unselectedFactions[index];

        foreach (var religion in factionA.Religions)
        {
            if((float) religion.Value / (float) factionA.Population >= .85)
            {
                if (!factionB.Religions.ContainsKey(religion.Key)
                    || (float)factionB.Religions[religion.Key] / (float)factionB.Population < .15)
                {
                    factionA.Relationships[factionB.Name] -= 100;
                }
            }
        }
    }				    
}
