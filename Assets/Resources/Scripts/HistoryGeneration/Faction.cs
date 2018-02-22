using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction {

    private Dictionary<string, int> _relationships; //<Faction Name, Affection Level>
    private Dictionary<string, int> _religions;     //<Religion Name, Number of Believers>

    public string Name;

    public int ScienceLevel;
    public int FaithLevel;

    public Entity Leader;
}
