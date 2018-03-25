using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFluff
{
    private List<string> Sex = new List<string>
    {
        "Male",
       "Female"
    };

    private string _name;
    private int age;
    private string background;
    private string _sex;

    public EntityFluff(string entityType, string factionType)
    {
        _sex = Sex[Random.Range(0, Sex.Count)];
        //TODO: Gen name
        //TODO: Set age somehow. Likely some kind of range depending on type of notable entity.
        //Might not need to do anything with background yet
    }

    private string GenerateName(string entityType, string factionType)
    {
        
    }
}
