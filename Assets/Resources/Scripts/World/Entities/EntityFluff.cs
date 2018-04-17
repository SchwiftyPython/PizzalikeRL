﻿using System.Collections;
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

    private readonly int _turnBorn;  
    private readonly string _entityType;
    private List<string> background;
    private readonly string _sex;

    public EntityFluff(string entityType, string factionType)
    {
        _sex = _sexes[Random.Range(0, _sexes.Count)];
        _entityType = entityType;
        Name = GenerateName();
        _turnBorn = HistoryGenerator.CurrentTurn;
        background = new List<string>();
    }

    public void AddToBackground(string story)
    {
        background.Add(story);
    }

    public int GetAgeInTurns()
    {
        return (HistoryGenerator.CurrentTurn - _turnBorn) / HistoryGenerator.TurnsPerTime["year"];
    }

    private string GenerateName()
    {
        return new NameStore(_entityType, _sex).GenerateName();
    }
}
