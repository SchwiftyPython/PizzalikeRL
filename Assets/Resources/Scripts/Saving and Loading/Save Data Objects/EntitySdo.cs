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

    public SerializableVector3 CurrentPosition;
   
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

    public ToppingSdo ToppingDropped;

    public static SaveGameData.SaveData.SerializableEntitiesDictionary ConvertToEntitySdos(List<Entity> entities)
    {
        var sdos = new SaveGameData.SaveData.SerializableEntitiesDictionary();

        foreach (var entity in entities)
        {
            if (entity.CurrentCell == null)
            {
                continue;
            }

            var sdo = ConvertToEntitySdo(entity);
            sdos.Add(sdo.Id, sdo);
        }

        return sdos;
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
            Mobile = entity.Mobile,
            ToppingDropped = ToppingSdo.ConvertToToppingSdo(entity.ToppingDropped)
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

    public static Dictionary<Guid, Entity> ConvertToEntities(
        SaveGameData.SaveData.SerializableEntitiesDictionary entitySdos)
    {
        var entities = new Dictionary<Guid, Entity>();

        foreach (var entitySdo in entitySdos)
        {
            var entity = new Entity(entitySdo.Key, entitySdo.Value.PrefabPath, entitySdo.Value.IsPlayer)
            {
                PrefabPath = entitySdo.Value.PrefabPath,
                TotalBodyPartCoverage = entitySdo.Value.TotalBodyPartCoverage,
                CurrentPosition = entitySdo.Value.CurrentPosition,
                Level = entitySdo.Value.Level,
                Xp = entitySdo.Value.Xp,
                Strength = entitySdo.Value.Strength,
                Agility = entitySdo.Value.Agility,
                Constitution = entitySdo.Value.Constitution,
                Intelligence = entitySdo.Value.Intelligence,
                MaxHp = entitySdo.Value.MaxHp,
                CurrentHp = entitySdo.Value.CurrentHp,
                Speed = entitySdo.Value.Speed,
                Defense = entitySdo.Value.Defense,
                Body = entitySdo.Value.Body,
                EntityType = entitySdo.Value.EntityType,
                Classification = entitySdo.Value.Classification,
                Fluff = entitySdo.Value.Fluff,
                Goals = entitySdo.Value.Goals,
                Mobile = entitySdo.Value.Mobile,
                CurrentCell = WorldData.Instance.MapDictionary[entitySdo.Value.CurrentCellId],
                ToppingDropped = ToppingSdo.ConvertToTopping(entitySdo.Value.ToppingDropped)
            };
            
            foreach (var itemId in entitySdo.Value.InventoryItemIds)
            {
                var item = WorldData.Instance.Items[itemId];
                entity.Inventory.Add(item.Id, item);
            }

            foreach (var equipped in entitySdo.Value.EquippedIds)
            {
                if (equipped.Value == Guid.Empty)
                {
                    continue;
                }

                var item = entity.Inventory[equipped.Value];

                if (entity.Equipped.ContainsKey(equipped.Key))
                {
                    entity.Equipped[equipped.Key] = item;
                }
                else
                {
                   entity.Equipped.Add(equipped.Key, item);
                }
            }
            entities.Add(entity.Id, entity);
        }
        return entities;
    }
}
