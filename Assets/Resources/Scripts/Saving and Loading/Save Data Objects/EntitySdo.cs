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

    public FactionSdo FactionSdo;

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

    public IDictionary<Guid, Item> Inventory { get; set; }

    public IDictionary<BodyPart, Item> Equipped;

    public IDictionary<Guid, BodyPart> Body { get; set; } = new Dictionary<Guid, BodyPart>();

    public string EntityType { get; set; }

    public Entity.EntityClassification Classification { get; set; }

    public EntityFluff Fluff { get; set; }

    public Stack<Goal> Goals;

    public CellSdo CurrentCellSdo;

    public AreaSdo CurrentAreaSdo;

    public TileSdo CurrentTileSdo;

    public bool Mobile;

    public static List<EntitySdo> ConvertToEntitySdos(List<Entity> entities)
    {
        return entities.Select(ConvertToEntitySdo).ToList();
    }

    public static EntitySdo ConvertToEntitySdo(Entity entity)
    {
        return new EntitySdo
        {
            Id = entity.Id,
            IsPlayer = entity.IsPlayer(),
            PrefabPath = entity.PrefabPath,
            FactionSdo = FactionSdo.ConvertToFactionSdo(entity.Faction),
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
            Inventory = entity.Inventory,
            Equipped = entity.Equipped,
            Body = entity.Body,
            EntityType = entity.EntityType,
            Classification = entity.Classification,
            Fluff = entity.Fluff,
            Goals = entity.Goals,
            CurrentCellSdo = CellSdo.ConvertToCellSdo(entity.CurrentCell),
            CurrentAreaSdo = AreaSdo.ConvertAreaForSaving(entity.CurrentArea),
            CurrentTileSdo = TileSdo.ConvertToTileSdo(entity.CurrentTile),
            Mobile = entity.Mobile
        };
    }
}
