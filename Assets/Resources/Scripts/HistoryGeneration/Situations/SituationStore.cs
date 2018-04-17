using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SituationStore
{
    private readonly Dictionary<string, Action<SituationContainer>> _allSituations = new Dictionary<string, Action<SituationContainer>>
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

    public void RunSituation(string situation, SituationContainer sc = null)
    {
        if (!_allSituations.ContainsKey(situation))
        {
            return;
        }

        try
        {
            _allSituations[situation](sc);
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

    // TODO needs to be redone without prereqs
    private static void HereticNation(SituationContainer sc = null)
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

    private static void PlagueOutbreak(SituationContainer sc = null)
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

    private static void PlagueContinues(SituationContainer sc)
    {
        var plagueFaction = sc.Factions.SingleOrDefault();

        if (plagueFaction == null)
        {
            return;
        }

        var infected = Random.Range(1, (int) (plagueFaction.Population * .1)) * -1;
        plagueFaction.ChangePopulation(infected);

        const int chanceOfLeaderDeath = 5;
        var roll = Random.Range(0, 100);

        if (roll < chanceOfLeaderDeath)
        {
            FactionLeaderDiesFromPlague(plagueFaction);
        }

        sc.TurnsTilNextSituation = HistoryGenerator.TurnsPerTime["week"];
    }

    private static void PlagueEndsNaturally(SituationContainer sc)
    {
        var plagueFaction = sc.Factions.SingleOrDefault();
        if (plagueFaction == null)
        {
            return;
        }

        //TODO: Write fluff

        HistoryGenerator.RemoveFromActiveSituations(sc);
    }

    private static void PlagueCured(SituationContainer sc)
    {
        var plagueFaction = sc.Factions.SingleOrDefault();
        if (plagueFaction == null)
        {
            return;
        }

        //TODO: Write fluff

        HistoryGenerator.RemoveFromActiveSituations(sc);
    }

    private static void FactionLeaderAssassination(SituationContainer sc)
    {
        var faction = PickFaction();

        sc = new SituationContainer
        {
            Factions = new List<Faction> { faction},
            NamedCharacters = new List<Entity> { faction.Leader}
        };

        faction.CreateLeader();

        sc.NamedCharacters.Add(faction.Leader);

        StoryGenerator.Instance.Write("faction leader assassination", sc);

    }

    private static void FactionLeaderDiesFromPlague(Faction faction)
    {
        faction.CreateLeader();

        //TODO: Write fluff
    }

    private static void FactionLeaderDiesNaturally(SituationContainer sc = null)
    {
        var faction = PickFaction();

        faction.CreateLeader();

        //TODO: Write fluff
    }

    private static void FactionLeaderDisappears(SituationContainer sc = null)
    {
        var faction = PickFaction();

        faction.CreateLeader();

        //TODO: Write fluff
    }

    #endregion Situations

}
