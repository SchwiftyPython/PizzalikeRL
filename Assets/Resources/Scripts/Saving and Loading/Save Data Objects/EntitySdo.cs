using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntitySdo
{
    public Guid Id;

    public bool IsPlayer;

    public string PrefabPath;

    public string FactionName;

    public int TotalBodyPartCoverage;

    public Vector3 CurrentPosition;
   
    public int Level { get; set; }

    public int Xp { get; set; }

    public int Strength { get;  set; }

    public int Agility { get;  set; }

    public int Constitution { get;  set; }

    public int Intelligence { get;  set; }

    public int MaxHp { get;  set; }

    public int CurrentHp { get;  set; }

    public int Speed { get;  set; }

    public int Defense { get;  set; }
   
    public List<Guid> InventoryItemIds { get; set; }

    public IDictionary<BodyPart, Guid> EquippedIds;

    public Entity.BodyDictionary Body { get; set; } 

    public string EntityType { get; set; }

    public Entity.EntityClassification Classification { get; set; }

    public EntityFluff Fluff { get; set; }

    public Stack<Goal> Goals;

    public string CurrentCellId;

    public string CurrentAreaId;

    public string CurrentTileId;

    public bool Mobile;

    public static List<EntitySdo> ConvertToEntitySdos(List<Entity> entities)
    {
        return entities.Select(ConvertToEntitySdo).ToList();
    }

    public static EntitySdo ConvertToEntitySdo(Entity entity)
    {
        var sdo =  new EntitySdo
        {
            Id = entity.Id,
            IsPlayer = entity.IsPlayer(),
            PrefabPath = entity.PrefabPath,
            FactionName = entity.Faction?.Name,
            TotalBodyPartCoverage = entity.TotalBodyPartCoverage,
            CurrentPosition = entity.CurrentPosition,
            Level = entity.Level,
            Xp = entity.Xp,
            Strength = entity.Strength,
            Agility = entity.Agility,
            Constitution = entity.Constitution,
            Intelligence = entity.Intelligence,
            MaxHp = entity.MaxHp,
            CurrentHp = entity.CurrentHp,
            Speed = entity.Speed,
            Defense = entity.Defense,
            InventoryItemIds = new List<Guid>(),
            EquippedIds = new Dictionary<BodyPart, Guid>(),
            Body = entity.Body,
            EntityType = entity.EntityType,
            Classification = entity.Classification,
            Fluff = entity.Fluff,
            Goals = entity.Goals,
            CurrentCellId = entity.CurrentCell?.Id,
            CurrentAreaId = entity.CurrentArea?.Id,
            CurrentTileId = entity.CurrentTile?.Id,
            Mobile = entity.Mobile
        };

        foreach (var itemId in entity.Inventory.Keys)
        {
            sdo.InventoryItemIds.Add(itemId);
        }

        foreach (var part in entity.Equipped.Keys)
        {
           sdo.EquippedIds.Add(new KeyValuePair<BodyPart, Guid>(part, entity.Equipped[part].Id));
        }

        return sdo;
    }
}
