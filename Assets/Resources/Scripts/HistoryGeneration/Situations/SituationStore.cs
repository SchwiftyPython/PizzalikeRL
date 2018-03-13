using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class SituationStore
{

    private Dictionary<string, Action> _startSituations = new Dictionary<string, Action>
    {
        {"heretic nation", () => { HereticNation(WorldData.Instance.Factions); }}
    };
    
    private Dictionary<string, Action> _middleSituations;
    private Dictionary<string, Action> _endSituations;

    public List<string> GetSituationsOfType(string situationType)
    {
        switch (situationType.ToLower())
        {
            case "start":
                return _startSituations.Keys.ToList();
            case "middle":
                return _middleSituations.Keys.ToList();
            case "end":
                return _endSituations.Keys.ToList();
            default:
                return null;
        }
    }

    public void RunStartSituation(string situation)
    {
        _startSituations[situation]();
    }

    private static void HereticNation(Dictionary <string, Faction> factions)
    {
        if (factions.Count < 2)
        {
            return;
        }

        var unselectedFactions = factions.Values.ToList();

        var index = Random.Range(0, unselectedFactions.Count);
        var factionA = unselectedFactions[index];
        unselectedFactions.Remove(unselectedFactions[index]);

        index = Random.Range(0, unselectedFactions.Count);
        var factionB = unselectedFactions[index];

        foreach (var religion in factionA.Religions)
        {
            if((float) religion.Value / (float) factionA.Population >= .85)
            {
                if (!factionB.Religions.ContainsKey(religion.Key)
                    || (float)factionB.Religions[religion.Key] / (float)factionB.Population < .15)
                {
                    factionA.Relationships[factionB.Name] -= 100;
                }
            }
        }
    }				    
}
