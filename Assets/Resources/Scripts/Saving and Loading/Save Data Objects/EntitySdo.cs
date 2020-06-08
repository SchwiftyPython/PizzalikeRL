using System;
using System.Collections.Generic;
using UnityEditor;
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

    public IDictionary<Entity.EquipmentSlot, Guid> EquippedIds;

    public Entity.BodyDictionary Body { get; set; } 

    public string EntityType { get; set; }

    public Entity.EntityClassification Classification { get; set; }

    public EntityFluff Fluff { get; set; }

    //todo may store this to kick off ai on load. May not need to store anything at all as it's not like the player knows what the ai is thinking.
    public string ParentGoalName; 

    public string CurrentCellId;

    public string CurrentAreaId;

    public string CurrentTileId;

    public bool Mobile;

    public ToppingSdo ToppingDropped;

    public Entity.ToppingCountDictionary ToppingCounts;

    public Guid BirthMotherId;

    public Guid BirthFatherId;

    public  List<Guid> ChildrenIds;

    public Reputation Reputation;

    public List<string> AbilityNames;

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
        EntitySdo sdo;
        sdo = new EntitySdo();
        sdo.Id = entity.Id;
        sdo.IsPlayer = entity.IsPlayer();
        sdo.PrefabPath = entity.PrefabPath;
        sdo.FactionName = entity.Faction?.Name;
        sdo.TotalBodyPartCoverage = entity.TotalBodyPartCoverage;
        sdo.CurrentPosition = entity.CurrentPosition;
        sdo.Level = entity.Level;
        sdo.Xp = entity.Xp;
        sdo.Strength = entity.Strength;
        sdo.Agility = entity.Agility;
        sdo.Constitution = entity.Constitution;
        sdo.Intelligence = entity.Intelligence;
        sdo.MaxHp = entity.MaxHp;
        sdo.CurrentHp = entity.CurrentHp;
        sdo.Speed = entity.Speed;
        sdo.Defense = entity.Defense;
        sdo.InventoryItemIds = new List<Guid>();
        sdo.EquippedIds = new Dictionary<Entity.EquipmentSlot, Guid>();
        sdo.Body = entity.Body;
        sdo.EntityType = entity.EntityType;
        sdo.Classification = entity.Classification;
        sdo.Fluff = entity.Fluff;
        sdo.CurrentCellId = entity.CurrentCell?.Id;
        sdo.CurrentAreaId = entity.CurrentArea?.Id;
        sdo.CurrentTileId = entity.CurrentTile?.Id;
        sdo.Mobile = entity.Mobile;
        sdo.ToppingDropped = ToppingSdo.ConvertToToppingSdo(entity.ToppingDropped);
        sdo.ToppingCounts = entity.ToppingCounts;
        sdo.BirthMotherId = entity.BirthMother?.Id ?? Guid.Empty;
        sdo.BirthFatherId = entity.BirthFather?.Id ?? Guid.Empty;
        sdo.ChildrenIds = new List<Guid>();
        sdo.Reputation = entity.EntityReputation;
        sdo.AbilityNames = new List<string>();

        foreach (var itemId in entity.Inventory.Keys)
        {
            sdo.InventoryItemIds.Add(itemId);
        }

        foreach (var slot in entity.Equipped.Keys)
        {
           sdo.EquippedIds.Add(new KeyValuePair<Entity.EquipmentSlot, Guid>(slot, entity.Equipped[slot]?.Id ?? Guid.Empty));
        }

        if (entity.Children != null && entity.Children.Count > 0)
        {
            foreach (var child in entity.Children)
            {
                sdo.ChildrenIds.Add(child.Id);
            }
        }

        if (entity.Abilities != null && entity.Abilities.Count > 0)
        {
            foreach (var abilityName in entity.Abilities.Keys)
            {
                sdo.AbilityNames.Add(abilityName);
            }
        }

        return sdo;
    }

    public static Dictionary<Guid, Entity> ConvertToEntities(
        SaveGameData.SaveData.SerializableEntitiesDictionary entitySdos)
    {
        var entities = new Dictionary<Guid, Entity>();

        foreach (var entitySdo in entitySdos)
        {
            var entity = ConvertToEntity(entitySdo.Value);

            entities.Add(entity.Id, entity);
        }

        foreach (var entitySdo in entitySdos)
        {
            var entity = entities[entitySdo.Key];

            if (entities.ContainsKey(entitySdo.Value.BirthMotherId))
            {
                entity.BirthMother = entities[entitySdo.Value.BirthMotherId];
            }

            if (entities.ContainsKey(entitySdo.Value.BirthFatherId))
            {
                entity.BirthFather = entities[entitySdo.Value.BirthFatherId];
            }

            foreach (var childId in entitySdo.Value.ChildrenIds)
            {
                if (entities.ContainsKey(childId))
                {
                    entity.Children.Add(entities[childId]);
                }
            }
        }
        return entities;
    }

    public static Entity ConvertToEntity(EntitySdo entitySdo)
    {
        Entity entity;
        entity = new Entity(entitySdo.Id, entitySdo.PrefabPath, entitySdo.IsPlayer);
        entity.PrefabPath = entitySdo.PrefabPath;
        entity.TotalBodyPartCoverage = entitySdo.TotalBodyPartCoverage;
        entity.Level = entitySdo.Level;
        entity.Xp = entitySdo.Xp;
        entity.Strength = entitySdo.Strength;
        entity.Agility = entitySdo.Agility;
        entity.Constitution = entitySdo.Constitution;
        entity.Intelligence = entitySdo.Intelligence;
        entity.MaxHp = entitySdo.MaxHp;
        entity.CurrentHp = entitySdo.CurrentHp;
        entity.Speed = entitySdo.Speed;
        entity.Defense = entitySdo.Defense;
        entity.Body = entitySdo.Body;
        entity.EntityType = entitySdo.EntityType;
        entity.Classification = entitySdo.Classification;
        entity.Fluff = entitySdo.Fluff;
        entity.Mobile = entitySdo.Mobile;
        entity.ToppingDropped = ToppingSdo.ConvertToTopping(entitySdo.ToppingDropped);
        entity.ToppingCounts = entitySdo.ToppingCounts;
        entity.EntityReputation = entitySdo.Reputation;
        entity.Abilities = new Entity.AbilityDictionary();

        if (string.IsNullOrEmpty(entitySdo.CurrentCellId))
        {
            entity.CurrentCell = null;
            entity.CurrentArea = null;
            entity.CurrentTile = null;
        }
        else
        {
            entity.CurrentCell = WorldData.Instance.MapDictionary[entitySdo.CurrentCellId];
            entity.CurrentArea = entity.CurrentCell.GetAreaById(entitySdo.CurrentAreaId);
            entity.CurrentTile = entity.CurrentArea?.GetTileById(entitySdo.CurrentTileId);
        }

        entity.CurrentPosition = new Vector3(entitySdo.CurrentPosition.Y, entitySdo.CurrentPosition.X,
            entitySdo.CurrentPosition.Z);

        foreach (var itemId in entitySdo.InventoryItemIds)
        {
            if (itemId == Guid.Empty)
            {
                continue;
            }

            var item = WorldData.Instance.Items[itemId];
            entity.Inventory.Add(item.Id, item);
        }

        foreach (var equipped in entitySdo.EquippedIds)
        {
            if (equipped.Value == Guid.Empty)
            {
                continue;
            }

            var item = WorldData.Instance.Items[equipped.Value];

            if (entity.Equipped.ContainsKey(equipped.Key))
            {
                entity.Equipped[equipped.Key] = item;
            }
            else
            {
                entity.Equipped.Add(equipped.Key, item);
            }
        }

        if (entitySdo.AbilityNames != null && entitySdo.AbilityNames.Count > 0)
        {
            foreach (var abilityName in entitySdo.AbilityNames)
            {
                var template = AbilityStore.GetAbilityByName(abilityName);

                var ability = AbilityStore.CreateAbility(template, entity);

                entity.Abilities.Add(abilityName, ability);
            }
        }

        return entity;
    }
}
