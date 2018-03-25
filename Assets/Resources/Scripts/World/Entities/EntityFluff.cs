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

    private int age;
    private readonly string _entityType;
    private string background;
    private readonly string _sex;

    public EntityFluff(string entityType, string factionType)
    {
        _sex = _sexes[Random.Range(0, _sexes.Count)];
        _entityType = entityType;
        Name = GenerateName();
        //TODO: Set age somehow. Likely some kind of range depending on type of notable entity.
        //TODO: Gen story of coming into power
    }

    private string GenerateName()
    {
        return new NameStore(_entityType, _sex).GenerateName();
    }
}
