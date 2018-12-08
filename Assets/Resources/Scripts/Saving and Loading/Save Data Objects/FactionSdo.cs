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

    public List<EntitySdo> CitizenSdos;

    public Entity Leader;

    public List<Entity> EntitiesWithFluff;

    public static List<FactionSdo> ConvertToFactionSdos(List<Faction> factions)
    {
        return factions.Select(ConvertToFactionSdo).ToList();
    }

    public static FactionSdo ConvertToFactionSdo(Faction faction)
    {
        return new FactionSdo
        {
            PopType = faction.PopType,
            Relationships = faction.Relationships,
            CitizenSdos = EntitySdo.ConvertToEntitySdos(faction.Citizens),
            EntitiesWithFluff = faction.EntitiesWithFluff,
            Leader = faction.Leader,
            Name = faction.Name,
            Population = faction.Population
        };
    }
}
