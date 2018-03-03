using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryGenerator : MonoBehaviour {
    private const int NumTurnsPerDay = 6;

    private enum SituationTypes {
        Start,
        Middle,
        End
    }

    private enum Days {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    private enum Months {
        Rooster,
        Dog,
        Wombat,
        Flamingo
    }

                         // unit of time, "turns"   
    private readonly Dictionary<string, int> _time = new Dictionary<string, int> {
        { "day", NumTurnsPerDay },      
        { "week", NumTurnsPerDay * 7 },   // 7 days
        { "month", NumTurnsPerDay * 28 }, // 4 weeks 
        { "year", NumTurnsPerDay * 112 }   // 4 months
    };

    private List<Situation> _startSituations;
    private List<Situation> _middleSituations;
    private List<Situation> _endSituations;

    private Days _currentDay;
    private Months _currentMonth;
    private int _currentYear;

    private void Start()
    {
        SituationLoader.Initialize();

        _startSituations = SituationLoader.GetSituationsOfType(SituationTypes.Start.ToString());
        _middleSituations = SituationLoader.GetSituationsOfType(SituationTypes.Middle.ToString());
        _endSituations = SituationLoader.GetSituationsOfType(SituationTypes.End.ToString());

        _currentDay = Days.Monday;
        _currentMonth = Months.Rooster;
        _currentYear = 0;

        Generate();
    }

    private void Generate() {
        var startSituation = PickStartSituation();

        
    }

    public Situation PickStartSituation()
    {
        return _startSituations[Random.Range(0, _startSituations.Count)];
    }
}
