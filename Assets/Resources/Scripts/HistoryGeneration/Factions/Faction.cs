using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Faction
{
    private const int MaxRelationshipLevel = 1000;
    private const int MinRelationshipLevel = MaxRelationshipLevel * -1;

    public string Type;

    public Dictionary<string, int> Relationships; //<Faction Name, Affection Level>
    public Dictionary<string, int> Religions;     //<Religion Name, Number of Believers>

    public string Name;
    public int Population;

    public int ScienceLevel;
    public int FaithLevel;

    public Entity Leader;

    public EntityTemplate EntityType;

    public Faction(FactionTemplate factionTemplate)
    {
        Relationships = new Dictionary<string, int>();
        Religions = new Dictionary<string, int>();

        Type = factionTemplate.Type;

        var index = Random.Range(0, factionTemplate.EntityTypes.Count);

        EntityType = EntityTemplateLoader.GetEntityTemplate(factionTemplate.EntityTypes[index]);

        CreateLeader();
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

    private void CreateLeader()
    {
        Leader = new Entity(EntityType, Name);
    }
}
