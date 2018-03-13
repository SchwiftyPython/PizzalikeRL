using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction {

    public Dictionary<string, int> Relationships; //<Faction Name, Affection Level>
    public Dictionary<string, int> Religions;     //<Religion Name, Number of Believers>

    public string Name;
    public int Population;

    public int ScienceLevel;
    public int FaithLevel;

    public Entity Leader;

    public Faction(FactionTemplate factionTemplate)
    {
        Relationships = new Dictionary<string, int>();
        Religions = new Dictionary<string, int>();

        Name = factionTemplate.Name;
    }
}
