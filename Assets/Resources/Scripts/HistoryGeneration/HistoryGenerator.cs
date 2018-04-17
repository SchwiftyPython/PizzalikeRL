using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class HistoryGenerator : MonoBehaviour {
    private const int TurnsPerDay = 6;
    private const int DaysPerWeek = 7;
    private const int DaysPerMonth = 28;
    private const int DaysPerYear = 112;
    private const int MinTurns = 20 * TurnsPerDay * DaysPerYear;
    private const int MaxTurns = 30 * TurnsPerDay * DaysPerYear;

    private enum SituationTypes
    {
        Start,
        Middle,
        End
    }

    private readonly List<string> _daysOfTheWeek = new List<string>
    {
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday"
    };

    private readonly List<string> _months = new List<string>
    {
        "Rooster",
        "Dog",
        "Wombat",
        "Flamingo"
    };

                         // unit of time, "turns"   
    public static readonly Dictionary<string, int> TurnsPerTime = new Dictionary<string, int> {
        { "day", TurnsPerDay },      
        { "week", TurnsPerDay * DaysPerWeek },   // 7 days
        { "month", TurnsPerDay * DaysPerMonth }, // 4 weeks 
        { "year", TurnsPerDay * DaysPerYear }   // 4 months
    };

    private SituationStore _situationStore;

    private List<string> _startSituations;
//    private List<string> _middleSituations;
//    private List<string> _endSituations;

    public static Dictionary<GUID, Situation> ActiveSituations { get; set; }

    private string _currentDayOfTheWeek;
    private int _currentNumericalDay;
    private string _currentMonth;
    private int _currentYear;

    public static int CurrentTurn;

    private void Start()
    {
        _situationStore = new SituationStore();
        _situationStore.Initialize();

        FactionTemplateLoader.Initialize();

        ActiveSituations = new Dictionary<GUID, Situation>();

        _startSituations = _situationStore.GetSituationsOfType(SituationTypes.Start.ToString());
//        _middleSituations = _situationStore.GetSituationsOfType(SituationTypes.Middle.ToString());
//        _endSituations = _situationStore.GetSituationsOfType(SituationTypes.End.ToString());

        _currentDayOfTheWeek = _daysOfTheWeek[0];
        _currentNumericalDay = 1;
        _currentMonth = _months[0];
        _currentYear = 0;

        CurrentTurn = 0;

        Generate();

        Debug.Log($"Done Generating on {_currentMonth} {_currentDayOfTheWeek}, {_currentYear}");

//        foreach (var faction in WorldData.Instance.Factions.Values)
//        {
//            Debug.Log($"Faction Leader for {faction.Type} at end: {faction.Leader.Fluff.Name}");
//        }
    }

    private void Generate()
    {
        var turnsLeftInDay = TurnsPerTime["day"];
        var turnsLeftInMonth = TurnsPerTime["month"];
        var turnsLeftInYear = TurnsPerTime["year"];
        var turnsLeftInHistoryGeneration = Random.Range(MinTurns, MaxTurns);

        //Testing //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (WorldData.Instance.Factions == null)
        {
            WorldData.Instance.Factions = new Dictionary<string, Faction>();
        }

        WorldData.Instance.Factions.Add("biker gang", new Faction(FactionTemplateLoader.GetFactionByName("biker gang")));
        WorldData.Instance.Factions.Add("geriatric", new Faction(FactionTemplateLoader.GetFactionByName("geriatric")));

        WorldData.Instance.Factions["biker gang"].Population = 550;
        WorldData.Instance.Factions["geriatric"].Population = 600;

        WorldData.Instance.Factions["biker gang"].Religions.Add("harley", 500);
        WorldData.Instance.Factions["geriatric"].Religions.Add("harley", 10);

        WorldData.Instance.Factions["biker gang"].Relationships.Add("geriatric", 0);
        WorldData.Instance.Factions["geriatric"].Relationships.Add("biker gang", 0);

//        foreach (var faction in WorldData.Instance.Factions.Values)
//        {
//            Debug.Log($"Faction Leader for {faction.Type} at start: {faction.Leader.Fluff.Name}");
//        }

        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        while (turnsLeftInHistoryGeneration > 0)
        { 
            while (turnsLeftInYear > 0)
            {
                while (turnsLeftInMonth > 0)
                {
                    while (turnsLeftInDay > 0)
                    {
                        if (ActiveSituations.Any())
                        {
                            var activeSituations = ActiveSituations.Values.ToList();
                            foreach (var situation in activeSituations)
                            {
                                if (situation.GetTurnsTilNextSituation() <= 0)
                                {
                                    var nextSituation = PickNextSituation(situation.GetNextSituations());
                                    _situationStore.RunSituation(nextSituation);

//                                    Debug.Log($"Ran {nextSituation} on {_currentDayOfTheWeek} {_currentMonth} {_currentNumericalDay}, {_currentYear}\n " +
//                                              $"Faction: {situation.GetFactions().First().Name}: {situation.GetFactions().First().Population}");
                                }
                                else
                                {
                                    situation.DecrementTurnsTilNextSituation();
                                }
                            }
                        }

                        var startSituation = PickStartSituation();
                        _situationStore.RunSituation(startSituation);

                        //Debug.Log($"Ran {startSituation} on {_currentDayOfTheWeek} {_currentMonth} {_currentNumericalDay}, {_currentYear}");

                        CurrentTurn++;

                        turnsLeftInDay--;
                        turnsLeftInMonth--;
                        turnsLeftInYear--;
                        turnsLeftInHistoryGeneration--;
                    }
                    AdvanceToNextDay();
                    turnsLeftInDay = TurnsPerTime["day"];
                }
                AdvanceToNextMonth();
                turnsLeftInMonth = TurnsPerTime["month"];
            }
            _currentYear++;
            turnsLeftInYear = TurnsPerTime["year"];
        }
    }

    private string PickStartSituation()
    {
        return _startSituations[Random.Range(0, _startSituations.Count)];
    }

    private static string PickNextSituation(IReadOnlyList<string> nextSituations)
    {
        return nextSituations[Random.Range(0, nextSituations.Count)];
    }

    private void AdvanceToNextDay()
    {
        var curIndex = _daysOfTheWeek.IndexOf(_currentDayOfTheWeek);
        _currentDayOfTheWeek = curIndex == _daysOfTheWeek.Count - 1 ? _daysOfTheWeek[0] : _daysOfTheWeek[curIndex + 1];

        if (_currentNumericalDay >= DaysPerMonth)
        {
            _currentNumericalDay = 1;
        }
        else
        {
            _currentNumericalDay++;
        }
    }

    private void AdvanceToNextMonth()
    {
        var curIndex = _months.IndexOf(_currentMonth);
        _currentMonth = curIndex == _months.Count - 1 ? _months[0] : _months[curIndex + 1];
    }

    public static void AddToActiveSituations(SituationContainer sc)
    {
        ActiveSituations.Add(sc.SituationId, new Situation(sc));
    }

    public static void RemoveFromActiveSituations(SituationContainer sc)
    {
        ActiveSituations.Remove(sc.SituationId);
    }

    public static bool SituationIdExists(GUID id)
    {
        return ActiveSituations.ContainsKey(id);
    }
}
