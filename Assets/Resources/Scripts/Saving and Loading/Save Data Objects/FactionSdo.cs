using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class FactionSdo 
{
    public Faction.PopulationType PopType;

    public Faction.RelationshipDictionary Relationships; 

    public string Name;

    public int Population;

    public List<Guid> CitizenIds;

    public Guid LeaderId;

    public List<Guid> EntitiesWithFluffIds;

    public static List<FactionSdo> ConvertToFactionSdos(List<Faction> factions)
    {
        return factions.Select(ConvertToFactionSdo).ToList();
    }

    public static FactionSdo ConvertToFactionSdo(Faction faction)
    {
        var sdo = new FactionSdo
        {
            PopType = faction.PopType,
            Relationships = faction.Relationships,
            CitizenIds = new List<Guid>(),
            EntitiesWithFluffIds = new List<Guid>(),
            LeaderId = faction.Leader.Id,
            Name = faction.Name,
            Population = faction.Population
        };

        foreach (var citizen in faction.Citizens)
        {
            sdo.CitizenIds.Add(citizen.Id);
        }

        foreach (var entity in faction.EntitiesWithFluff)
        {
            sdo.EntitiesWithFluffIds.Add(entity.Id);
        }

        return sdo;
    }
}
