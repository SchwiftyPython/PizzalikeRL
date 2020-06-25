using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SituationStore
{
    private readonly Dictionary<string, Action<SituationContainer>> _allSituations = new Dictionary<string, Action<SituationContainer>>
    {
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

    public void Initialize()
    {
        _startSituations = GetSituationsFromFile(SituationLoader.Instance.StartSituationFile);
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
        var situations = new List<string>();
        try
        {
            using (var reader = new StreamReader(fullPath))
            {
                string line;
                while (null != (line = reader.ReadLine()?.Trim()))
                {
                    var processedLine = line.Split(',');

                    situations.Add(processedLine.FirstOrDefault()); 
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error processing file: " + fullPath + " " + e.Message);
        }
        return situations;
    }

    private static List<string> GetSituationsFromFile(TextAsset file)
    {
        var situations = new List<string>();
        try
        {
            situations.AddRange(file.text.Split("\r\n"[0]).ToList());
        }
        catch (Exception e)
        {
            Debug.Log("Error processing file: " + file.name + " " + e.Message);
        }

        return situations;
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

        var id = Guid.NewGuid();

        while (HistoryGenerator.SituationIdExists(id))
        {
            id = Guid.NewGuid();
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
