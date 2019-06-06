//<summary>
// For easier management of relationships
//</summary>

public enum EntityGroupType
{
    Faction,
    EntityType
}

public class EntityGroup
{
    public EntityGroupType GroupType { get; set; }

    public string Name;

    public EntityGroup(Faction faction)
    {
        GroupType = EntityGroupType.Faction;
        Name = faction.Name;
    }

    public EntityGroup(string entityType)
    {
        GroupType = EntityGroupType.EntityType;
        Name = entityType;
    }
}
