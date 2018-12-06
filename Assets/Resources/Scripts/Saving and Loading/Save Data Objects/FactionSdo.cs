using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class FactionSdo 
{
    public Faction.PopulationType PopType;

    public Dictionary<string, int> Relationships; //<Faction Name, Affection Level>

    public string Name;

    public int Population;

    public List<Entity> Citizens;

    public Entity Leader;

    public List<Entity> EntitiesWithFluff;

    public static List<FactionSdo> ConvertToFactionSdos(List<Faction> factions)
    {
        List<FactionSdo> list = new List<FactionSdo>();
        foreach (var faction in factions)
        {
            var sdo = new FactionSdo();
            sdo.PopType = faction.PopType;
            sdo.Relationships = faction.Relationships;
            sdo.Citizens = faction.Citizens;
            sdo.EntitiesWithFluff = faction.EntitiesWithFluff;
            sdo.Leader = faction.Leader;
            sdo.Name = faction.Name;
            sdo.Population = faction.Population;
            list.Add(sdo);
        }

        return list;
    }

    public static FactionSdo ConvertToFactionSdo(Faction faction)
    {
        var sdo = new FactionSdo();
        sdo.PopType = faction.PopType;
        sdo.Relationships = faction.Relationships;
        sdo.Citizens = faction.Citizens;
        sdo.EntitiesWithFluff = faction.EntitiesWithFluff;
        sdo.Leader = faction.Leader;
        sdo.Name = faction.Name;
        sdo.Population = faction.Population;

        return sdo;
    }
}
