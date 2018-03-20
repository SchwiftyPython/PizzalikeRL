using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story {
    private Dictionary<string, Entity> actors;
    private Dictionary<string, Situation> situations;

    private int _timeTilNextSituation;

    public Story() {
        
    }
}
