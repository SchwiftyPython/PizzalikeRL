using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Faction
{
    [Serializable]
    public class RelationshipDictionary : SerializableDictionary<string, int> { }

    public enum PopulationType
    {
        Monospecies,
        MixedSpecies
    }

    private const int MaxRelationshipLevel = 1000;
    private const int MinRelationshipLevel = MaxRelationshipLevel * -1;

    public PopulationType PopType;

    public string Type;

    public RelationshipDictionary Relationships; //<Faction Name, Affection Level>
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
        Relationships = new RelationshipDictionary();
        Religions = new Dictionary<string, int>();
        EntitiesWithFluff = new List<Entity>();

        Name = FactionTemplateLoader.GenerateFactionName();
        GeneratePopulation();
        CreateLeader();
    }

    public Faction(FactionTemplate factionTemplate)
    {
        Relationships = new RelationshipDictionary();
        Religions = new Dictionary<string, int>();
        EntitiesWithFluff = new List<Entity>();

        Type = factionTemplate.Type;

        var index = Random.Range(0, factionTemplate.EntityTypes.Count);

        EntityType = EntityTemplateLoader.GetEntityTemplate(factionTemplate.EntityTypes[index]);

        CreateLeader();

        WorldData.Instance.FactionLeaders.Add(Leader);

        //Debug.Log("Leader name: " + Leader.Fluff.Name);
    }

    public void ChangeRelationshipValue(Faction otherFaction, int relationshipChange)
    {
        Relationships[otherFaction.Name] += relationshipChange;

        if (Relationships[otherFaction.Name] > MaxRelationshipLevel)
        {
            Relationships[otherFaction.Name] = MaxRelationshipLevel;
            return;
        }
        if (Relationships[otherFaction.Name] < MinRelationshipLevel)
        {
            Relationships[otherFaction.Name] = MinRelationshipLevel;
        }
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
        Population = Random.Range(100, 1000);
        
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

        for (var i = 0; i < Population; i += 50)
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
            citizen.CreateFluff(template, Name);
            Citizens.Add(citizen);
        }

        RemainingCitizensToPlace = new List<Entity>(Citizens);
    }
}
