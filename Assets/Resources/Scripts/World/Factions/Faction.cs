using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Faction
{
    public enum PopulationType
    {
        Monospecies,
        MixedSpecies
    }

    private const int MaxRelationshipLevel = 1000;
    private const int MinRelationshipLevel = MaxRelationshipLevel * -1;

    private PopulationType _popType;

    public string Type;

    public Dictionary<string, int> Relationships; //<Faction Name, Affection Level>
    public Dictionary<string, int> Religions;     //<Religion Name, Number of Believers>

    public string Name;
    public int Population;
    public List<Entity> Citizens;

    public int ScienceLevel;
    public int FaithLevel;

    public Entity Leader;

    public List<Entity> EntitiesWithFluff;

    public EntityTemplate EntityType;

    public Faction()
    {
        GeneratePopulation();
        //pick random name for faction
        //create leader - pick an entity from deck
    }

    public Faction(FactionTemplate factionTemplate)
    {
        Relationships = new Dictionary<string, int>();
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
        //TODO: Change Type to Name when Faction Name Gen is done
        Relationships[otherFaction.Type] += relationshipChange;

        if (Relationships[otherFaction.Type] > MaxRelationshipLevel)
        {
            Relationships[otherFaction.Type] = MaxRelationshipLevel;
            return;
        }
        if (Relationships[otherFaction.Type] < MinRelationshipLevel)
        {
            Relationships[otherFaction.Type] = MinRelationshipLevel;
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
        Leader = new Entity(EntityType, Name) {Fluff = new EntityFluff(EntityType.Type, Type)};

        EntitiesWithFluff.Add(Leader);
    }

    private void PickPopulationType()
    {
        var roll = Random.Range(0, 100);

        _popType = roll <= 40 ? PopulationType.MixedSpecies : PopulationType.Monospecies;
    }

    private void GeneratePopulation()
    {
        PickPopulationType();
        Population = Random.Range(100, 1000);

        var availableEntityTypes = new List<string>();
        if (_popType == PopulationType.MixedSpecies)
        {
            availableEntityTypes = new List<string>(EntityTemplateLoader.GetEntityTemplateTypes());
        }
        else
        {
            var allTypes = EntityTemplateLoader.GetEntityTemplateTypes();
            var index = Random.Range(0, allTypes.Length);
            availableEntityTypes.Add(allTypes[index]);
        }

        Citizens = new List<Entity>();

        for (var i = 0; i < Population; i += 50)
        {
            var index = Random.Range(0, availableEntityTypes.Count);
            Citizens.Add(new Entity(EntityTemplateLoader.GetEntityTemplate(availableEntityTypes[index])));
        }
    }
}
