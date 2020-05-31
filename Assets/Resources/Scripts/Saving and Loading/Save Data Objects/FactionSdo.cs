using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class FactionSdo 
{
    public Faction.PopulationType PopType;

    public Reputation FactionReputation; 

    public string Name;

    public int Population;

    public List<Guid> CitizenIds;

    public EntitySdo Leader;

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
            FactionReputation = faction.FactionReputation,
            CitizenIds = new List<Guid>(),
            EntitiesWithFluffIds = new List<Guid>(),
            Leader = EntitySdo.ConvertToEntitySdo(faction.Leader),
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
