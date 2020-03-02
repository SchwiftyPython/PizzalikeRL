using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Faction
{
    public enum PopulationType
    {
        Monospecies,
        MixedSpecies
    }

    private const int CitizenPopulationValue = 5;

    public PopulationType PopType;

    public string Type;

    public Reputation FactionReputation; 
    public Dictionary<string, int> Religions;     //<Religion Name, Number of Believers>

    public string Name;
    public int Population;
    public List<Entity> Citizens;
    public List<Entity> RemainingCitizensToPlace;

    public int ScienceLevel;
    public int FaithLevel;

    public Entity Leader;

    public List<Entity> EntitiesWithFluff;

    public EntityTemplate EntityType;

    public Faction()
    {
        Religions = new Dictionary<string, int>();
        EntitiesWithFluff = new List<Entity>();

        Name = FactionTemplateLoader.GenerateFactionName();
        GeneratePopulation();
        CreateLeader();

        FactionReputation = new Reputation(EntityGroupType.Faction, Name);

        WorldData.Instance.EntityGroupRelationships.Add(Name, FactionReputation);
    }

    public Faction(FactionSdo sdo)
    {
        Religions = new Dictionary<string, int>();
        EntitiesWithFluff = new List<Entity>();

        PopType = sdo.PopType;
        FactionReputation = sdo.FactionReputation;
        Citizens = new List<Entity>();
        EntitiesWithFluff = new List<Entity>();
        Leader = WorldData.Instance.Entities[sdo.LeaderId];
        Name = sdo.Name;
        Population = sdo.Population;

        foreach (var id in sdo.CitizenIds)
        {
            if (!WorldData.Instance.Entities.ContainsKey(id))
            {
                continue;
            }

            var citizen = WorldData.Instance.Entities[id];
            citizen.Faction = this;
            Citizens.Add(citizen);
        }

        foreach (var id in sdo.EntitiesWithFluffIds)
        {
            if (!WorldData.Instance.Entities.ContainsKey(id))
            {
                continue;
            }

            var entity = WorldData.Instance.Entities[id];
            EntitiesWithFluff.Add(entity);
        }
    }

    public Faction(FactionTemplate factionTemplate)
    {
        Religions = new Dictionary<string, int>();
        EntitiesWithFluff = new List<Entity>();

        Type = factionTemplate.Type;

        var index = Random.Range(0, factionTemplate.EntityTypes.Count);

        EntityType = EntityTemplateLoader.GetEntityTemplate(factionTemplate.EntityTypes[index]);

        CreateLeader();

        WorldData.Instance.FactionLeaders.Add(Leader);

        FactionReputation = new Reputation(EntityGroupType.Faction, Name);

        WorldData.Instance.EntityGroupRelationships.Add(Name, FactionReputation);
    }

    public bool IsFanaticOfReligion(string religionName)
    {
        return Religions[religionName] / (float) Population >= .85;
    }

    public bool IsHereticOfReligion(string religionName)
    {
        return !Religions.ContainsKey(religionName)
               || Religions[religionName] / (float) Population < .15;
    }

    public void ChangePopulation(int change)
    {
        Population += change;

        //todo figure out where below will fit. Probably need an overload for when this is called after a citizen was killed

        //todo if oldpop - newpop >= CitizenPopulationValue then pick citizen at random and kill
        //if newpop - oldpop >= CitizenPopulationValue then new citizen
        // num to kill or create = difference in pop / CitizenPopulationValue
    }

    public void CreateLeader()
    {
        var index = Random.Range(0, Citizens.Count);

        var chosenOne = Citizens[index];

        Leader = chosenOne;

        EntitiesWithFluff.Add(Leader);
    }

    private void PickPopulationType()
    {
        var roll = Random.Range(0, 100);

        PopType = roll <= 40 ? PopulationType.MixedSpecies : PopulationType.Monospecies;
    }

    private void GeneratePopulation()
    {
        const int humanoidChance = 96;

        PickPopulationType();
        Population = Random.Range(900, 2250);
        
        var availableEntityTypes = new List<string>();
        if (PopType == PopulationType.MixedSpecies)
        {
            availableEntityTypes = new List<string>(EntityTemplateLoader.GetAllEntityTemplateTypes());
        }
        else
        {
            var allTypes = EntityTemplateLoader.GetAllEntityTemplateTypes();
            var index = Random.Range(0, allTypes.Length);

            var validPick = false;
            while (!validPick)
            {
                var typeTemplate = EntityTemplateLoader.GetEntityTemplate(allTypes[index]);

                var roll = Random.Range(0, 101);

                if (roll < humanoidChance)
                {
                    if (typeTemplate.Classification == Entity.EntityClassification.Humanoid)
                    {
                        validPick = true;
                    }
                }
                else
                {
                    if (typeTemplate.Classification == Entity.EntityClassification.NonHumanoid)
                    {
                        validPick = true;
                    }
                }
                index = Random.Range(0, allTypes.Length);
            }

            availableEntityTypes.Add(allTypes[index]);
        }

        Citizens = new List<Entity>();

        EntityTemplate template = null;

        for (var i = 0; i < Population; i += CitizenPopulationValue)
        {
            if (PopType == PopulationType.MixedSpecies)
            {
                var index = Random.Range(0, availableEntityTypes.Count);
                var validPick = false;

                while (!validPick)
                {
                    template = EntityTemplateLoader.GetEntityTemplate(availableEntityTypes[index]);

                    var roll = Random.Range(0, 101);

                    if (roll < humanoidChance)
                    {
                        if (template.Classification == Entity.EntityClassification.Humanoid)
                        {
                            validPick = true;
                        }
                    }
                    else
                    {
                        if (template.Classification == Entity.EntityClassification.NonHumanoid)
                        {
                            validPick = true;
                        }
                    }
                    index = Random.Range(0, availableEntityTypes.Count);
                }
            }
            else
            {
                template = EntityTemplateLoader.GetEntityTemplate(availableEntityTypes[0]);
            }

            var citizen = new Entity(template, this);
            citizen.GenerateStartingEquipment();
            citizen.CreateFluff(template, Name);
            Citizens.Add(citizen);
            WorldData.Instance.Entities.Add(citizen.Id, citizen);
        }

        Debug.Log($"Number of Citizens: {Citizens.Count}");

        RemainingCitizensToPlace = new List<Entity>(Citizens);
    }
}
