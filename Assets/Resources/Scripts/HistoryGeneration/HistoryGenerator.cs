using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryGenerator : MonoBehaviour {
    private const int TurnsPerDay = 6;
    private const int DaysPerWeek = 7;
    private const int DaysPerMonth = 28;
    private const int DaysPerYear = 112;
    private const int MaxTurns = 56000;

    private enum SituationTypes
    {
        Start,
        Middle,
        End
    }

    private readonly List<string> _days = new List<string>
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
    private readonly Dictionary<string, int> _turnsPerTime = new Dictionary<string, int> {
        { "day", TurnsPerDay },      
        { "week", TurnsPerDay * DaysPerWeek },   // 7 days
        { "month", TurnsPerDay * DaysPerMonth }, // 4 weeks 
        { "year", TurnsPerDay * DaysPerYear }   // 4 months
    };

    private List<Situation> _startSituations;
    private List<Situation> _middleSituations;
    private List<Situation> _endSituations;

    private string _currentDay;
    private string _currentMonth;
    private int _currentYear;

    private void Start()
    {
        SituationLoader.Initialize();
        FactionTemplateLoader.Initialize();

        _startSituations = SituationLoader.GetSituationsOfType(SituationTypes.Start.ToString());
        _middleSituations = SituationLoader.GetSituationsOfType(SituationTypes.Middle.ToString());
        _endSituations = SituationLoader.GetSituationsOfType(SituationTypes.End.ToString());

        _currentDay = _days[0];
        _currentMonth = _months[0];
        _currentYear = 0;

        Generate();
    }

    private void Generate()
    {
        var turnsLeftInDay = _turnsPerTime["day"];
        var turnsLeftInMonth = _turnsPerTime["month"];
        var turnsLeftInYear = _turnsPerTime["year"];
        var turnsLeftInHistoryGeneration = MaxTurns;

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

        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        while (turnsLeftInHistoryGeneration > 0)
        {
            while (turnsLeftInYear > 0)
            {
                while (turnsLeftInMonth > 0)
                {
                    while (turnsLeftInDay > 0)
                    {
                        var startSituation = PickStartSituation();

                        var conditionChecker = new ConditionChecker();

                        var method = conditionChecker.CheckCondition(startSituation.ConditionCode);
                        method.Invoke(null, null);
                        Debug.Log($"Ran {method} on {_currentMonth} {_currentDay}, {_currentYear}");

                        turnsLeftInDay--;
                        turnsLeftInMonth--;
                        turnsLeftInYear--;
                        turnsLeftInHistoryGeneration--;
                    }
                    AdvanceToNextDay();
                    turnsLeftInDay = _turnsPerTime["day"];
                }
                AdvanceToNextMonth();
                turnsLeftInMonth = _turnsPerTime["month"];
            }
            _currentYear++;
            turnsLeftInYear = _turnsPerTime["year"];
        }
    }

    private Situation PickStartSituation()
    {
        return _startSituations[Random.Range(0, _startSituations.Count)];
    }

    private void AdvanceToNextDay()
    {
        var curIndex = _days.IndexOf(_currentDay);
        _currentDay = curIndex == _days.Count - 1 ? _days[0] : _days[curIndex + 1];
    }

    private void AdvanceToNextMonth()
    {
        var curIndex = _months.IndexOf(_currentMonth);
        _currentMonth = curIndex == _months.Count - 1 ? _months[0] : _months[curIndex + 1];
    }
}
