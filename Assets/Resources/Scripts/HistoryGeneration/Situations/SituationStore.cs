using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SituationStore
{
    //todo throw this into a file as well 
    private readonly Dictionary<string, Action> _allSituations = new Dictionary<string, Action>
    {
        {"heretic nation", HereticNation},
        {"plague outbreak", PlagueOutbreak},
        {"plague continues", PlagueContinues},
        {"plague ends naturally", PlagueEndsNaturally},
        {"plague cured", PlagueCured},
        {"faction leader assassination", FactionLeaderAssassination },
        {"faction leader dies naturally", FactionLeaderDiesNaturally },
        {"faction leader disappears", FactionLeaderDisappears}
    };

    private List<string> _startSituations;
    private List<string> _middleSituations;
    private List<string> _endSituations;

    private const string ResourcesPath = "\\Assets\\Resources\\";
    private const string StartSituationFile = "start_situations.csv";
    private const string MiddleSituationFile = @"middle_situations.csv";
    private const string EndSituationFile = @"end_situations.csv";

    public void Initialize()
    {
        var basePath = Environment.CurrentDirectory;

        var fullPath = Path.Combine(basePath, ResourcesPath.TrimStart('\\', '/'), StartSituationFile);

        _startSituations = GetSituationsFromFile(fullPath);
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
        if (!_allSituations.ContainsKey(situation))
        {
            return;
        }

        try
        {
            _allSituations[situation]();
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(
                $"Error running situation: {situation}\n {e.Message}");
            throw;
        }
    }

    private static List<string> GetSituationsFromFile(string file)
    {
        var fullPath = Path.Combine(ResourcesPath, file);
        var situatons = new List<string>();
        try
        {
            using (var reader = new StreamReader(fullPath))
            {
                string line;
                while (null != (line = reader.ReadLine()?.Trim()))
                {
                    situatons.AddRange(line.Split(',')); 
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error processing file: " + fullPath + " " + e.Message);
        }
        return situatons;
    }

    private static Faction PickFaction()
    {
        if (WorldData.Instance.Factions.Count < 1)
        {
            return null;
        }

        var unselectedFactions = WorldData.Instance.Factions.Values.ToList();

        var index = Random.Range(0, unselectedFactions.Count);

        return unselectedFactions[index];
    }

    #region Situations

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
            if (!factionA.IsFanaticOfReligion(religion.Key))
            {
                continue;
            }

            if (factionB.IsHereticOfReligion(religion.Key))
            {
                factionA.ChangeRelationshipValue(factionB, -100);
            }
        }
    }

    private static void PlagueOutbreak()
    {
        var plagueFaction = PickFaction();

        var infected = Random.Range(1, (int)(plagueFaction.Population * .05)) * -1;
        plagueFaction.ChangePopulation(infected);

        var nextSituations = new List<string>
        {
            "plague continues",
            "plague ends naturally",
            "plague cured"
        };

        var id = GUID.Generate();

        while (HistoryGenerator.SituationIdExists(id))
        {
            id = GUID.Generate();
        }

        var situationContainer = new SituationContainer
        {
            SituationId = id,
            NextSituations = nextSituations,
            Factions = new List<Faction>{plagueFaction},
            TurnsTilNextSituation = HistoryGenerator.TurnsPerTime["week"]
        };

        HistoryGenerator.AddToActiveSituations(situationContainer);
    }

    private static void PlagueContinues()
    {
        var activeSituationContainers = (from s in HistoryGenerator.ActiveSituations
            where s.Value.GetTurnsTilNextSituation() <= 0 
            && s.Value.GetNextSituations().Contains("plague continues")
            select s.Value.GetSituationContainer()).ToList();
        
        foreach (var sc in activeSituationContainers)
        {
            var plagueFaction = sc.Factions.SingleOrDefault();

            if (plagueFaction == null)
            {
                continue;
            }

            var infected = Random.Range(1, (int)(plagueFaction.Population * .1)) * -1;
            plagueFaction.ChangePopulation(infected);

            const int chanceOfLeaderDeath = 5;
            var roll = Random.Range(0, 100);

            if (roll < chanceOfLeaderDeath)
            {
                FactionLeaderDiesFromPlague(plagueFaction);
            }

            sc.TurnsTilNextSituation = HistoryGenerator.TurnsPerTime["week"];
        }
    }

    private static void PlagueEndsNaturally()
    {
        var activeSituationContainers = (from s in HistoryGenerator.ActiveSituations
            where s.Value.GetTurnsTilNextSituation() <= 0
                  && s.Value.GetNextSituations().Contains("plague end naturally")
            select s.Value.GetSituationContainer()).ToList();

        foreach (var sc in activeSituationContainers)
        {
            var plagueFaction = sc.Factions.SingleOrDefault();
            if (plagueFaction == null)
            {
                continue;
            }

            //TODO: Write fluff

            HistoryGenerator.RemoveFromActiveSituations(sc);
        }
    }

    private static void PlagueCured()
    {
        var activeSituationContainers = (from s in HistoryGenerator.ActiveSituations
            where s.Value.GetTurnsTilNextSituation() <= 0
                  && s.Value.GetNextSituations().Contains("plague cured")
            select s.Value.GetSituationContainer()).ToList();

        foreach (var sc in activeSituationContainers)
        {
            var plagueFaction = sc.Factions.SingleOrDefault();
            if (plagueFaction == null)
            {
                continue;
            }

            //TODO: Write fluff

            HistoryGenerator.RemoveFromActiveSituations(sc);
        }
    }

    private static void FactionLeaderAssassination()
    {
        var faction = PickFaction();

        faction.CreateLeader();

        //TODO: Write fluff

    }

    private static void FactionLeaderDiesFromPlague(Faction faction)
    {
        faction.CreateLeader();

        //TODO: Write fluff
    }

    private static void FactionLeaderDiesNaturally()
    {
        var faction = PickFaction();

        faction.CreateLeader();

        //TODO: Write fluff
    }

    private static void FactionLeaderDisappears()
    {
        var faction = PickFaction();

        faction.CreateLeader();

        //TODO: Write fluff
    }

    #endregion Situations

}
