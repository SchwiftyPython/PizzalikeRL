using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFluff
{
    private readonly List<string> _sexes = new List<string>
    {
        "male",
       "female"
    };

    public string Name;
    public CharacterBackground BackgroundType;
    public List<string> Background;
    public int Age;

    private readonly int _turnBorn;  
    private readonly string _entityType;
    private readonly string _sex;
    private string _factionType;

    public EntityFluff(string entityType)
    {
        _sex = _sexes[Random.Range(0, _sexes.Count)];
        _entityType = entityType;
        Name = GenerateName();
        Background = new List<string>();
        _turnBorn = HistoryGenerator.CurrentTurn;
    }

    public EntityFluff(string entityType, string factionType)
    {
        _sex = _sexes[Random.Range(0, _sexes.Count)];
        _entityType = entityType;
        Name = GenerateName();
        _turnBorn = HistoryGenerator.CurrentTurn;
        Background = new List<string>();
        _factionType = factionType;
    }

    public void AddToBackground(string story)
    {
        Background.Add(story);
    }

    public int GetAgeInTurns()
    {
        return (HistoryGenerator.CurrentTurn - _turnBorn) / HistoryGenerator.TurnsPerTime["year"];
    }

    public List<string> GetBackground()
    {
        return Background;
    }

    private string GenerateName()
    {
        return new NameStore(_entityType, _sex).GenerateName();
    }
}
