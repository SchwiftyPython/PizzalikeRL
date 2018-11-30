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
    public string FactionName { get; }

    public EntityFluff(string entityType, List<string> possibleNameFiles)
    {
        _sex = _sexes[Random.Range(0, _sexes.Count)];
        _entityType = entityType;
        Name = GenerateName(possibleNameFiles);
        Background = new List<string>();
        _turnBorn = HistoryGenerator.CurrentTurn;
    }

    public EntityFluff(string entityType, string factionName, List<string> possibleNameFiles)
    {
        _sex = _sexes[Random.Range(0, _sexes.Count)];
        _entityType = entityType;
        Name = GenerateName(possibleNameFiles);
        _turnBorn = HistoryGenerator.CurrentTurn;
        Background = new List<string>();
        FactionName = factionName;
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

    private string GenerateName(List<string> possibleNameFiles)
    {
        return NameStore.Instance.GenerateFullName(possibleNameFiles, _sex);
    }
}
