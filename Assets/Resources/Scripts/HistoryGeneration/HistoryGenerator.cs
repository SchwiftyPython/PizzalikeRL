using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HistoryGenerator : MonoBehaviour
{
    private const int TurnsPerDay = 6;
    private const int DaysPerWeek = 7;
    private const int DaysPerMonth = 28;
    private const int DaysPerYear = 112;
    private const int MinTurns = 18 * TurnsPerDay * DaysPerYear;
    private const int MaxTurns = 21 * TurnsPerDay * DaysPerYear;

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

    public static Dictionary<Guid, Situation> ActiveSituations { get; set; }

    private string _currentDayOfTheWeek;
    private int _currentNumericalDay;
    private string _currentMonth;
    private int _currentYear;

    public static int CurrentTurn;

    public static HistoryGenerator Instance;

    private void Start()
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

        _situationStore = new SituationStore();
        _situationStore.Initialize();

        //FactionTemplateLoader.Initialize();

        ActiveSituations = new Dictionary<Guid, Situation>();

        _startSituations = _situationStore.GetSituationsOfType(SituationTypes.Start.ToString());
//        _middleSituations = _situationStore.GetSituationsOfType(SituationTypes.Middle.ToString());
//        _endSituations = _situationStore.GetSituationsOfType(SituationTypes.End.ToString());

        _currentDayOfTheWeek = _daysOfTheWeek[0];
        _currentNumericalDay = 1;
        _currentMonth = _months[0];
        _currentYear = 0;

        CurrentTurn = 0;

        //Generate();

        //Debug.Log($"Done Generating on {_currentMonth} {_currentDayOfTheWeek}, {_currentYear}");

//        foreach (var faction in WorldData.Instance.Factions.Values)
//        {
//            Debug.Log($"Faction Leader for {faction.Type} at end: {faction.Leader.Fluff.Name}");
//        }
    }

    public void Generate()
    {
        var turnsLeftInDay = TurnsPerTime["day"];
        var turnsLeftInMonth = TurnsPerTime["month"];
        var turnsLeftInYear = TurnsPerTime["year"];
        var turnsLeftInHistoryGeneration = Random.Range(MinTurns, MaxTurns);
        var turnsTilNextSituation = Random.Range(TurnsPerDay, TurnsPerDay * DaysPerMonth);
        var turnsTilNextLifeEvent = Random.Range(TurnsPerDay * DaysPerMonth, TurnsPerDay * DaysPerYear);

        try
        {
            while (turnsLeftInHistoryGeneration > 0)
            {
                if (turnsLeftInYear <= 0)
                {
                    _currentYear++;
                    turnsLeftInYear = TurnsPerTime["year"];
                }
                if (turnsLeftInMonth <= 0)
                {
                    AdvanceToNextMonth();
                    turnsLeftInMonth = TurnsPerTime["month"];
                }
                if (turnsLeftInDay <= 0)
                {
                    AdvanceToNextDay();
                    turnsLeftInDay = TurnsPerTime["day"];
                }
               
                // Could add player age to current year to find point to start generating
                // Also need to consider that at least one parent is spoken for from previous player
                // so will have to watch out with the parent stories. Maybe let it run like normal and
                // overwrite with information that was already available.
                if (CurrentTurn >= turnsTilNextLifeEvent)
                {
                    GameManager.Instance.Player.Fluff.AddToBackground(
                        BackgroundGenerator.Instance.GenerateLifeEvent());

                    turnsTilNextLifeEvent = Random.Range(TurnsPerDay * DaysPerMonth, TurnsPerDay * DaysPerYear);
                }

                if (ActiveSituations.Any())
                {
                    var activeSituations = ActiveSituations.Values.ToList();
                    foreach (var situation in activeSituations)
                    {
                        if (situation.GetTurnsTilNextSituation() <= 0)
                        {
                            var nextSituation = PickNextSituation(situation.GetNextSituations());
                            _situationStore.RunSituation(nextSituation, situation.GetSituationContainer());

                            //                                    Debug.Log($"Ran {nextSituation} on {_currentDayOfTheWeek} {_currentMonth} {_currentNumericalDay}, {_currentYear}\n " +
                            //                                              $"Faction: {situation.GetFactions().First().Name}: {situation.GetFactions().First().Population}");
                        }
                        else
                        {
                            situation.DecrementTurnsTilNextSituation();
                        }
                    }
                }

                if (turnsTilNextSituation <= 0)
                {
                    var startSituation = PickStartSituation();
                    _situationStore.RunSituation(startSituation);

                    turnsTilNextSituation = Random.Range(TurnsPerDay, TurnsPerDay * DaysPerMonth);

                    //                            Debug.Log(
                    //                                $"Ran {startSituation} on {_currentDayOfTheWeek} {_currentMonth} {_currentNumericalDay}, {_currentYear}");
                }

                CurrentTurn++;

                turnsLeftInDay--;
                turnsLeftInMonth--;
                turnsLeftInYear--;
                turnsLeftInHistoryGeneration--;
                turnsTilNextSituation--;
                turnsTilNextLifeEvent--;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error generating history: " + e.Message);
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

    public static bool SituationIdExists(Guid id)
    {
        return ActiveSituations.ContainsKey(id);
    }
}
