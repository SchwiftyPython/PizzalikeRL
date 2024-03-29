﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[Serializable]
public class Entity : ISubscriber
{
    public enum EntityClassification
    {
        Humanoid,
        NonHumanoid
    }

    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    [Description("Equipment Slot")]
    public enum EquipmentSlot
    {
        Body,
        Head,
        [Description("Right Arm")]
        RightArmOne,
        [Description("Right Arm Two")]
        RightArmTwo,
        [Description("Left Arm")]
        LeftArmOne,
        [Description("Left Arm Two")]
        LeftArmTwo,
        [Description("Right Hand")]
        RightHandOne,
        [Description("Right Hand Two")]
        RightHandTwo,
        [Description("Left Hand")]
        LeftHandOne,
        [Description("Left Hand Two")]
        LeftHandTwo,
        [Description("Missile Weapon")]
        MissileWeaponOne,
        Hands,
        Feet,
        Special,
        Thrown,
        Consumable
    }
    
    private readonly IDictionary<Direction, Vector2> _directions = new Dictionary<Direction, Vector2>
    {
        {Direction.North, new Vector2(1, 0)},
        {Direction.NorthEast, new Vector2(1, 1)},
        {Direction.East, new Vector2(0, 1)},
        {Direction.SouthEast, new Vector2(-1, 1)},
        {Direction.South, new Vector2(-1, 0)},
        {Direction.SouthWest, new Vector2(-1, -1)},
        {Direction.West,new Vector2(0, -1)},
        {Direction.NorthWest, new Vector2(1, -1)}
    };

    private Vector2 _startTile;
    private Vector2 _endTile;

    private readonly bool _isPlayer;
    private bool _isDead;
    private bool _isHostile;
    private bool _isWild;

    public Reputation EntityReputation { get; set; }

    private EventMediator _eventMediator;

    private int _lastTurnMoved;

    public Entity BirthFather { get; set; }
    public Entity BirthMother { get; set; }
    public List<Entity> Children { get; set; }

    private int _coins;

    public  GameObject Prefab;
    private GameObject _sprite;

    public Faction Faction;

    public int TotalBodyPartCoverage { get; set; }

    private Vector3 _currentPosition; //on screen position

    public Vector3 SpriteActualPosition;

    public string PrefabPath;

    public Guid Id;

    //Base stats

    public int Level { get; set; } //todo remove
    public int Xp { get; set; } //todo remove or modify for specific ability categories  

    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Constitution { get; set; }
    public int Intelligence { get; set; }

    //Stats dependent on base stat values

    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public int Speed { get; set; }
    public int Defense { get; set; }

    private List<Effect> _currentEffects;
    
    [Serializable]
    public class BodyDictionary : SerializableDictionary<Guid, BodyPart> { }

    public BodyDictionary Body { get; set; }

    public IDictionary<Guid, Item> Inventory { get; } //todo create method for adding items to inventory
    public IDictionary<EquipmentSlot, Item> Equipped;

    [Serializable]
    public class AbilityDictionary : SerializableDictionary<string, Ability> { }

    public AbilityDictionary Abilities;

    [Serializable]
    public class ToppingCountDictionary : SerializableDictionary<Toppings, int> { }

    public ToppingCountDictionary ToppingCounts = new ToppingCountDictionary();

    public Topping ToppingDropped;

    public string EntityType { get; set; }
    public EntityClassification Classification { get; set; }
    public EntityFluff Fluff { get; set; }

    public Stack<Goal> Goals;

    public Cell CurrentCell;
    public Area CurrentArea;
    public Tile CurrentTile;

    public bool Mobile;
    public bool IsCustomer;
    public bool IsMultiArmed; //can't we just make a method that checks how many arms we got?

    public Vector3 CurrentPosition
    {
        get => _currentPosition;

        set
        {
            _currentPosition = new Vector2(value.y, value.x);

            if (!IsPlayer() && CurrentTile != null)
            {
                if (!CurrentTile.Revealed || CurrentTile.Visibility == Visibilities.Invisible)
                {
                    return;
                }

                SetSpritePosition(_currentPosition);
            }
            else
            {
                SetSpritePosition(_currentPosition);
            }
        }
    }

    public Entity(Guid id, string prefabPath, bool isPlayer = false)
    {
        Id = id;
        _isPlayer = isPlayer;
        Inventory = new Dictionary<Guid, Item>();
        PopulateEquipped();

        Prefab = Resources.Load(prefabPath) as GameObject;

        if (isPlayer)
        {
            ToppingCounts = new ToppingCountDictionary
            {
                {Toppings.Bacon, 0 },
                {Toppings.BellPepper, 0 },
                {Toppings.Cheese, 0 },
                {Toppings.Jalapeno, 0 },
                {Toppings.Mushrooms, 0 },
                {Toppings.Olives, 0 },
                {Toppings.Onion, 0 },
                {Toppings.Pepperoni, 0 },
                {Toppings.Pineapple, 0 },
                {Toppings.Tomato, 0 },
                {Toppings.Wheat, 0 },
                {Toppings.Sausage, 0 }
            };
        }

        SubscribeToBaseEvents();

        _currentEffects = new List<Effect>();
        _lastTurnMoved = 0;
    }

    public Entity(Entity parent, Faction faction = null, bool isPlayer = false)
    {
        Id = Guid.NewGuid();

        _isPlayer = isPlayer;

        EntityType = GetEntityTypeFromParents(parent);

        Faction = faction;

        Inventory = new Dictionary<Guid, Item>();

        if (_isPlayer)
        {
            Level = 1;
            Xp = 0;

            ToppingCounts = new ToppingCountDictionary
            {
                {Toppings.Bacon, 0 },
                {Toppings.BellPepper, 0 },
                {Toppings.Cheese, 0 },
                {Toppings.Jalapeno, 0 },
                {Toppings.Mushrooms, 0 },
                {Toppings.Olives, 0 },
                {Toppings.Onion, 0 },
                {Toppings.Pepperoni, 0 },
                {Toppings.Pineapple, 0 },
                {Toppings.Tomato, 0 },
                {Toppings.Wheat, 0 },
                {Toppings.Sausage, 0 }
            };
        }

        const int minModifier = 4;
        const int maxModifier = 2;

        Strength = GenStrength(parent.Strength - minModifier, parent.Strength + maxModifier);
        Agility = GenAgility(parent.Agility - minModifier, parent.Agility + maxModifier);
        Constitution = GenConstitution(parent.Constitution - minModifier, parent.Constitution + maxModifier);
        Intelligence = GenIntelligence(parent.Intelligence - minModifier, parent.Intelligence + maxModifier);

        CurrentHp = MaxHp = GenMaxHp();
        Speed = GenSpeed();
        Defense = GenDefense();

        _isWild = false;
        Mobile = true;

        var template = EntityTemplateLoader.GetEntityTemplate(EntityType);

        Classification = template.Classification;

        if (WorldData.Instance.EntityGroupRelationships.ContainsKey(EntityType))
        {
            EntityReputation = WorldData.Instance.EntityGroupRelationships[EntityType];
        }
        else
        {
            EntityReputation = new Reputation(EntityGroupType.EntityType, EntityType);

            WorldData.Instance.EntityGroupRelationships.Add(EntityType, EntityReputation);
        }

        var allBackgrounds = CharacterBackgroundLoader.GetCharacterBackgroundTypes().ToList();

        var selectedBackground =
            CharacterBackgroundLoader.GetCharacterBackground(allBackgrounds[Random.Range(0, allBackgrounds.Count)]);

        CreateFluff(template, faction != null ? faction.Name : string.Empty);

        Fluff.BackgroundType = selectedBackground;

        PrefabPath = template.SpritePath;
        Prefab = Resources.Load(template.SpritePath) as GameObject;

        BuildBody(template);
        CalculateTotalBodyPartCoverage();
        PopulateEquipped();

        Abilities = BuildAbilityDictionary();

        CreateFluff(template);
        Fluff.Background = BackgroundGenerator.Instance.GenerateBackground();

        if (!string.IsNullOrEmpty(template.Topping))
        {
            ToppingDropped = new Topping(template.Topping);
        }

        SubscribeToBaseEvents();

        _currentEffects = new List<Effect>();
        _lastTurnMoved = 0;
    }

    public Entity(EntityTemplate template, Faction faction = null, bool isPlayer = false)
    {
        Id = Guid.NewGuid();

        _isPlayer = isPlayer;
        EntityType = template.Type;
        Classification = template.Classification;
        Faction = faction;

        Inventory = new Dictionary<Guid, Item>();

        if (_isPlayer)
        {
            Level = 1;
            Xp = 0;

            ToppingCounts = new ToppingCountDictionary
            {
                {Toppings.Bacon, 0 },
                {Toppings.BellPepper, 0 },
                {Toppings.Cheese, 0 },
                {Toppings.Jalapeno, 0 },
                {Toppings.Mushrooms, 0 },
                {Toppings.Olives, 0 },
                {Toppings.Onion, 0 },
                {Toppings.Pepperoni, 0 },
                {Toppings.Pineapple, 0 },
                {Toppings.Tomato, 0 },
                {Toppings.Wheat, 0 },
                {Toppings.Sausage, 0 }
            };
        }
        else
        {
            Strength = GenStrength(template.MinStrength, template.MaxStrength);
            Agility = GenAgility(template.MinAgility, template.MaxAgility);
            Constitution = GenConstitution(template.MinConstitution, template.MaxConstitution);
            Intelligence = GenIntelligence(template.MinIntelligence, template.MaxIntelligence);

            CurrentHp = MaxHp = GenMaxHp();
            Speed = GenSpeed();
            Defense = GenDefense();

            //TODO: gen level
            //TODO: gen coins
        }

        _isWild = template.Wild;
        Mobile = true;

        PrefabPath = template.SpritePath;
        Prefab = Resources.Load(template.SpritePath) as GameObject;

        Classification = template.Classification;

        if (WorldData.Instance.EntityGroupRelationships.ContainsKey(EntityType))
        {
            EntityReputation = WorldData.Instance.EntityGroupRelationships[EntityType];
        }
        else
        {
            EntityReputation = new Reputation(EntityGroupType.EntityType, EntityType);

            WorldData.Instance.EntityGroupRelationships.Add(EntityType, EntityReputation);
        }

        BuildBody(template);
        CalculateTotalBodyPartCoverage();
        PopulateEquipped();

        if (!string.IsNullOrEmpty(template.Topping))
        {
            ToppingDropped = new Topping(template.Topping);
        }

        SubscribeToBaseEvents();

        _currentEffects = new List<Effect>();
        _lastTurnMoved = 0;
    }

    public void SetStats(int strength, int agility, int constitution, int intelligence)
    {
        Strength = strength;
        Agility = agility;
        Constitution = constitution;
        Intelligence = intelligence;

        CurrentHp = MaxHp = GenMaxHp();
        Speed = GenSpeed();
        Defense = GenDefense();
    }

    public string GetTypeForEntityInfoWindow()
    {
        return $"{EntityType}";
    }

    public string GetFluffForEntityInfoWindow()
    {
        return $"{Fluff.Name}, {Fluff.FactionName}";
    }

    public string GetStatsForEntityInfoWindow()
    {
        return
            $"Current HP: {CurrentHp}\nStrength: {Strength}\nAgility: {Agility}\nConstitution: {Constitution}\nSpeed: {Speed}\nDefense: {Defense}";
    }

    public void EquipItem(Item item, EquipmentSlot slot)
    {
        //todo equipment that occupies more then one slot
        //todo two slot boolean in item.xml

        if (item == null)
        {
            return;
        }

        if (Inventory.ContainsKey(item.Id))
        {
            Inventory.Remove(item.Id);
        }

        if (Equipped[slot] != null)
        {
            Inventory.Add(Equipped[slot].Id, Equipped[slot]);
        }

        Equipped[slot] = item;

        //todo issue with this is NPCs' abilities will also be subscribed to this event
        //todo may be able to limit NPC subscription by current area then cost will be negligible
        //todo then can just check if the broadcaster is the ability's owner
        EventMediator.Instance.Broadcast(GlobalHelper.ItemEquippedEventName, this);

        //todo subscribe InventoryWindow to event
        if (InventoryWindow.Instance != null) InventoryWindow.Instance.InventoryChanged = true;
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        if (Equipped[slot] == null)
        {
            return;
        }

        Inventory.Add(Equipped[slot].Id, Equipped[slot]);

        Equipped[slot] = null;

        EventMediator.Instance.Broadcast(GlobalHelper.ItemUnequippedEventName, this);
        InventoryWindow.Instance.InventoryChanged = true;
    }

    private void PopulateEquipped()
    {
        Equipped = new Dictionary<EquipmentSlot, Item>();

        foreach (var slot in (EquipmentSlot[])(Enum.GetValues(typeof(EquipmentSlot))))
        {
            if (!Equipped.ContainsKey(slot))
            {
                Equipped.Add(slot, null);
            }
        }
    }

    private string GetEntityTypeFromParents(Entity parent)
    {
        if (parent.Fluff.Sex.Equals("male"))
        {
            BirthFather = parent;
        }
        if (parent.Fluff.Sex.Equals("female"))
        {
            BirthMother = parent;
        }

        var templateTypes = EntityTemplateLoader.GetAllEntityTemplateTypes();

        var entityType = templateTypes[Random.Range(0, templateTypes.Length)];

        var template = EntityTemplateLoader.GetEntityTemplate(entityType);

        if (BirthFather == null)
        {
            BirthFather = new Entity(template);
        }
        if (BirthMother == null)
        {
            BirthMother = new Entity(template);
        }

        var inheritFromChances = new Dictionary<string, int>
        {
            { "mother", 95 },
            { "father", 95 }
            //{ "both", 60 }
        };

        var chanceTotal = (double)inheritFromChances.Values.Sum(n => n);

        var picked = false;
        var inheritFrom = string.Empty;

        while (!picked)
        {
            var index = Random.Range(0, inheritFromChances.Count);

            var chosenParent = inheritFromChances.ElementAt(index);

            var chance = chosenParent.Value / chanceTotal;

            var roll = Random.Range(0f, 1f);

            if (roll < chance)
            {
                inheritFrom = chosenParent.Key;
                picked = true;
            }
        }

        switch (inheritFrom)
        {
            case "mother": return BirthMother.EntityType;
            case "father": return BirthFather.EntityType;
            //case "both": todo figure out some mish mash deal
            default: return string.Empty;
        }
    }

    private static int GenStrength(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    private static int GenAgility(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    private static int GenConstitution(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    private static int GenIntelligence(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    private int GenMaxHp()
    {
        //temp for testing
        return (Level + Constitution) * 10;
    }

    private int GenSpeed()
    {
        //temp for testing
        return (Level + Agility) * 5;
    }

    private int GenDefense()
    {
        //temp for testing
        return (Level + Agility + Constitution) * 2;
    }

    private void BuildBody(EntityTemplate template)
    {
        Body = new BodyDictionary();
        var bodyPartNames = BodyPartLoader.BodyPartNames;
        foreach (var templateBodyPart in template.Parts)
        {
            var partTemplate = BodyPartLoader.GetBodyPartTemplate(templateBodyPart);
            if (!bodyPartNames.Contains(templateBodyPart))
            {
                return;
            }
            if (partTemplate.NeedsPart.Equals(""))
            {
                AddBodyPart(new BodyPart(partTemplate));
            }
            else
            {
                BodyPart parent = null;
                var possibleParents = Body.Values.Where(bodyPart =>
                    bodyPart.Type.Equals(partTemplate.NeedsPart, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var possibleParent in possibleParents)
                {
                    if (possibleParent.ChildrenBodyParts.Count >= possibleParent.MaxChildrenBodyParts)
                    {
                        continue;
                    }

                    parent = possibleParent;
                    break;
                }

                if (parent != null)
                {
                    var part = new BodyPart(partTemplate);
                    parent.ChildrenBodyParts.Add(part);

                    part.ParentId = parent.Id;
                    AddBodyPart(part);
                }
                else
                {
                    Debug.Log($"Error building {template.Type}");
                    Debug.Log(partTemplate.Name + " missing required part " + partTemplate.NeedsPart);
                }
            }
        }

        var armCount = 0;
        foreach (var templateBodyPart in template.Parts.Where(templateBodyPart => templateBodyPart.Contains("arm")))
        {
            armCount++;

            if (armCount <= 3)
            {
                continue;
            }

            IsMultiArmed = true;
            break;
        }
    }

    public List<BodyPart> GetBodyPartsByType(string type)
    {
        var parts = new List<BodyPart>();

        foreach (var part in Body)
        {
            if (part.Value.Type.Equals(type))
            {
                parts.Add(part.Value);
            }
        }

        return parts;
    }

    private void AddBodyPart(BodyPart part)
    {
        part.Id = Guid.NewGuid();
        while (Body.ContainsKey(part.Id))
        {
            part.Id = Guid.NewGuid();
        }

        Body.Add(part.Id, part);
    }

    private void CalculateTotalBodyPartCoverage()
    {
        TotalBodyPartCoverage = (from bp in Body.Values select bp.Coverage).Sum();
    }

    public EntitySdo ConvertToEntitySdo()
    {
        return EntitySdo.ConvertToEntitySdo(this);
    }

    public bool IsPlayer()
    {
        return _isPlayer;
    }

    public GameObject GetSpritePrefab()
    {
        return Prefab;
    }

    public GameObject GetSprite()
    {
        return _sprite;
    }

    public void SetSprite(GameObject sprite)
    {
        _sprite = sprite;
    }

    public void SetSpritePosition(Vector3 newPosition)
    {
        if (_sprite == null)
        {
            return;
        }

        _sprite.transform.position = newPosition;
    }

    public void AreaMove(Vector2 target)
    {
        //todo: clean up this code
        _startTile = new Vector2(CurrentTile.X, CurrentTile.Y);
        _endTile = target;

        if (TileOutOfBounds(target))
        {
            var direction = AreaOutOfBoundsDirection(target, CurrentArea);
            var nextAreaPosition = new Vector2(CurrentArea.X + _directions[direction].x,
                CurrentArea.Y + _directions[direction].y);
            if (AreaOutOfBounds(nextAreaPosition))
            {
                direction = CellOutOfBoundsDirection(nextAreaPosition, CurrentCell);
                var nextCellPositon = new Vector2(CurrentCell.X + _directions[direction].x,
                    CurrentCell.Y + _directions[direction].y);
                if (CellOutOfBounds(nextCellPositon))
                {
                    Debug.Log("Cannot move. Edge of map.");
                }
                else
                {
                    CurrentCell = WorldData.Instance.Map[(int) nextCellPositon.x,
                        (int) nextCellPositon.y];

                    CurrentArea = CalculateCellEntryArea(nextAreaPosition);

                    if (!CurrentArea.AreaBuilt())
                    {
                        CurrentArea.Build();
                    }
                    
                    CurrentTile = CalculateAreaEntryTile(target);

                    //update tile data for start and end tiles
                    UpdateTileData(CurrentArea.AreaTiles[(int) _startTile.x, (int) _startTile.y],
                        CurrentArea.AreaTiles[CurrentTile.X, CurrentTile.Y]);

                    CurrentPosition = new Vector3(CurrentTile.X, CurrentTile.Y);

                    if (_isPlayer)
                    {
                        GameManager.Instance.CurrentTile = CurrentTile;
                        GameManager.Instance.CurrentArea = CurrentArea;
                        GameManager.Instance.CurrentCell = CurrentCell;
                        GameManager.Instance.CurrentState = GameManager.GameState.EnterArea;
                    }
                }
            }
            else
            {
                //move to next area but not next cell
                CurrentArea = CurrentCell.Areas[(int)nextAreaPosition.x,
                    (int)nextAreaPosition.y];

                if (!CurrentArea.AreaBuilt())
                {
                    CurrentArea.Build();
                }

                CurrentTile = CalculateAreaEntryTile(target);

                //update tile data for start and end tiles
                CurrentPosition = new Vector3(CurrentTile.X, CurrentTile.Y);
                UpdateTileData(CurrentArea.AreaTiles[(int) _startTile.x, (int) _startTile.y],
                    CurrentArea.AreaTiles[CurrentTile.X, CurrentTile.Y]);

                if (_isPlayer)
                {
                    GameManager.Instance.CurrentTile = CurrentTile;
                    GameManager.Instance.CurrentArea = CurrentArea;
                    GameManager.Instance.CurrentState = GameManager.GameState.EnterArea;
                }
            }
        }
        else
        {
            CurrentPosition = _endTile;
            CurrentTile = CurrentArea.AreaTiles[(int) _endTile.x, (int) _endTile.y];

            //update tile data for start and end tiles
            UpdateTileData(CurrentArea.AreaTiles[(int) _startTile.x, (int) _startTile.y],
                CurrentArea.AreaTiles[(int) _endTile.x, (int) _endTile.y]);

            if (_isPlayer)
            {
                GameManager.Instance.CurrentTile = CurrentTile;
            }
        }

        _lastTurnMoved = GameManager.Instance.TurnNumber;

        //Debug.Log("entity.currentPosition after move: " + CurrentTile.X + " " + CurrentTile.Y);
        //Debug.Log("sprite.currentPosition after move: " + _sprite.transform.position.x + " " + _sprite.transform.position.y);
    }

    public void WorldMapMove(Vector2 targetCell)
    {
        if (CellOutOfBounds(targetCell))
        {
            Debug.Log("Cannot move. Edge of map.");
        }
        else
        {
            CurrentPosition = new Vector3((int) targetCell.x, (int) targetCell.y);

            GameManager.Instance.CurrentCell = WorldData.Instance.Map[(int) targetCell.x, (int) targetCell.y];
            CurrentCell = GameManager.Instance.CurrentCell;

            //Set to middle area of cell
            CurrentArea = CurrentCell.Areas[1, 1];
            GameManager.Instance.CurrentArea = CurrentArea;
            _lastTurnMoved = GameManager.Instance.TurnNumber;
        }
    }

    public void UpdateTileData(Tile startTile, Tile endTile)
    {
        startTile.SetBlocksMovement(false);
        startTile.SetPresentEntity(null);

        endTile.SetBlocksMovement(true);
        endTile.SetPresentEntity(this);
    }

    public bool TileOutOfBounds(Vector2 target)
    {
        return target.y >= CurrentArea.Width || target.y < 0 || target.x >= CurrentArea.Height || target.x < 0;
    }

    public bool AreaOutOfBounds(Vector2 target)
    {
        return target.y >= CurrentCell.GetCellWidth() || target.y < 0 || target.x >= CurrentCell.GetCellHeight() ||
               target.x < 0;
    }

    public bool CellOutOfBounds(Vector2 target)
    {
        return target.y >= WorldData.Instance.Width || target.y < 0 || target.x >= WorldData.Instance.Height ||
               target.x < 0;
    }

    public Tile CalculateAreaEntryTile(Vector2 target)
    {
        var xOffset = 0;
        var yOffset = 0;
        if (target.x >= GameManager.Instance.CurrentArea.Height)
        {
            xOffset = -GameManager.Instance.CurrentArea.Height;
        }
        else if (target.x < 0)
        {
            xOffset = GameManager.Instance.CurrentArea.Height;
        }
        if (target.y >= GameManager.Instance.CurrentArea.Width)
        {
            yOffset = -GameManager.Instance.CurrentArea.Width;
        }
        else if (target.y < 0)
        {
            yOffset = GameManager.Instance.CurrentArea.Width;
        }
        return GameManager.Instance.CurrentArea.AreaTiles[(int) target.x + xOffset, (int) target.y + yOffset];
    }

    public Area CalculateCellEntryArea(Vector2 target)
    {
        var xOffset = 0;
        var yOffset = 0;
        if (target.x >= GameManager.Instance.CurrentCell.GetCellHeight())
        {
            xOffset = -GameManager.Instance.CurrentCell.GetCellHeight();
        }
        else if (target.x < 0)
        {
            xOffset = GameManager.Instance.CurrentCell.GetCellHeight();
        }
        if (target.y >= GameManager.Instance.CurrentCell.GetCellWidth())
        {
            yOffset = -GameManager.Instance.CurrentCell.GetCellWidth();
        }
        else if (target.y < 0)
        {
            yOffset = GameManager.Instance.CurrentCell.GetCellWidth();
        }
        var targetAreaPosition = new Vector2((int)target.x + xOffset, (int)target.y + yOffset);
        Debug.Log("Original target area:" + target);
        Debug.Log("Calculated target area:" + (int)targetAreaPosition.x + ", " + (int)targetAreaPosition.y);
        return CurrentCell.Areas[(int)targetAreaPosition.x, (int)targetAreaPosition.y];
    }

    public Direction AreaOutOfBoundsDirection(Vector2 target, Area area)
    {
        if (target.x >= area.Height)
        {
            if (target.y >= area.Width)
            {
                return Direction.NorthEast;
            }
            return target.y < 0 ? Direction.NorthWest : Direction.North;
        }
        if (target.x < 0)
        {
            if (target.y >= area.Width)
            {
                return Direction.SouthEast;
            }
            return target.y < 0 ? Direction.SouthWest : Direction.South;
        }
        return target.y >= area.Width ? Direction.East : Direction.West;
    }

    public Direction CellOutOfBoundsDirection(Vector2 target, Cell cell)
    {
        if (target.x >= cell.GetCellHeight())
        {
            if (target.y >= cell.GetCellWidth())
            {
                return Direction.NorthEast;
            }
            return target.y < 0 ? Direction.NorthWest : Direction.North;
        }
        if (target.x < 0)
        {
            if (target.y >= cell.GetCellWidth())
            {
                return Direction.SouthEast;
            }
            return target.y < 0 ? Direction.SouthWest : Direction.South;
        }
        return target.y >= cell.GetCellWidth() ? Direction.East : Direction.West;
    }

    public void MeleeAttack(Entity target)
    {
        if (MeleeRollHit(target))
        {
            ApplyMeleeDamage(target);
            if (!target.IsDead())
            {
                TargetReactToAttacker(target);
                return;
            }
            var message = $"{(target.Fluff != null ? target.Fluff.Name : target.EntityType)} died!";

            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, GlobalHelper.Capitalize(message));
        }
        else
        {
            TargetReactToAttacker(target);

            var message = string.Empty;

            if (IsPlayer())
            {
                message = $"You missed {(target.Fluff != null ? target.Fluff.Name : target.EntityType)}!";
            }
            else if (target.IsPlayer())
            {
                message = $"{(Fluff != null ? Fluff.Name : EntityType)} missed you!";
            }

            if (!string.IsNullOrEmpty(message))
            {
                EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, GlobalHelper.Capitalize(message));
            }
        }
    }

    public bool MoveOrAttackSuccessful(Vector2 target)
    {
        var currentScene = SceneManager.GetActiveScene().name;

        if (currentScene.Equals("Area"))
        {
            if (AreaMapCanMove(target))
            {
                AreaMove(target);
                var v = new Vinteger(CurrentTile.X, CurrentTile.Y);
                AreaMap.Instance.Fov.Refresh(v);
                AutoPickupToppingsInCurrentTile();
                AutoHarvestFields();
                AutoHarvestCheeseTree();
                return true;
            }
            if (!EntityPresent(target))
            {
                return false;
            }
            MeleeAttack(GameManager.Instance.CurrentArea.GetTileAt(target).GetPresentEntity());
            return true;
        }

        if (currentScene.Equals("WorldMap"))
        {
            if (WorldMapCanMove(target))
            {
                WorldMapMove(target);
                return true;
            }
        }
        return false;
    }

    public bool AreaMapCanMove(Vector2 target)
    {
        if (GameManager.Instance.AnyActiveWindows())
        {
            return false;
        }

        var currentArea = GameManager.Instance.CurrentArea;
        var currentCell = GameManager.Instance.CurrentCell;
        if (!TileOutOfBounds(target))
        {
            return !TargetTileBlocked(target);
        }
        var direction = AreaOutOfBoundsDirection(target, currentArea);
        var nextArea = new Vector2(currentArea.X + _directions[direction].x,
            currentArea.Y + _directions[direction].y);
        if (!AreaOutOfBounds(nextArea))
        {
            return true;
        }
        direction = CellOutOfBoundsDirection(nextArea, currentCell);
        var nextCell = new Vector2(currentCell.X + _directions[direction].x,
            currentCell.Y + _directions[direction].y);
        if (!CellOutOfBounds(nextCell))
        {
            return true;
        }
        Debug.Log("Cannot move. Edge of map.");
        return false;
    }

    public bool AreaMapCanMoveLocal(Vector2 target)
    {
        if (!TileOutOfBounds(target))
        {
            return !TargetTileBlocked(target);
        }
        Debug.Log("Cannot move. Edge of area.");
        return false;
    }

    public bool WorldMapCanMove(Vector2 targetCell)
    {
        if (!CellOutOfBounds(targetCell))
        {
            return true;
        }
        Debug.Log("Cannot move. Edge of map.");
        return false;
    }

    public bool TargetTileBlocked(Vector2 target)
    {
        return GameManager.Instance.CurrentArea.AreaTiles[(int) target.x, (int) target.y].GetBlocksMovement();
    }

    public bool TargetTileBlockedByEntity(Vector2 target)
    {
        return GameManager.Instance.CurrentArea.AreaTiles[(int) target.x, (int) target.y].GetPresentEntity() != null;
    }

    public void MoveToNextArea(Vector2 target)
    {
        var currentArea = GameManager.Instance.CurrentArea;
        var currentCell = GameManager.Instance.CurrentCell;
        var areaDirection = _directions[AreaOutOfBoundsDirection(target, currentArea)];
        var nextArea = new Vector2(currentArea.X + areaDirection.x,
            currentArea.Y + areaDirection.y);
        GameManager.Instance.CurrentArea = currentCell.Areas[(int) nextArea.x, (int) nextArea.y];
    }

    public void MoveToNextCell(Vector2 target)
    {
        var currentCell = GameManager.Instance.CurrentCell;
        var cellDirection = _directions[CellOutOfBoundsDirection(target, currentCell)];
        var nextArea = new Vector2(currentCell.X + cellDirection.x,
            currentCell.Y + cellDirection.y);
        GameManager.Instance.CurrentArea = currentCell.Areas[(int) nextArea.x, (int) nextArea.y];
    }

    public bool EntityPresent(Vector2 target)
    {
        return TargetTileBlockedByEntity(target);
    }

    public bool IsDead()
    {
        return CurrentHp <= 0;
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (_eventMediator == null)
        {
            _eventMediator = EventMediator.Instance;
        }

        if (eventName.Equals(GlobalHelper.UnderAttackEventName))
        {
            if (IsPlayer() || !(broadcaster is Entity victim) || victim.CurrentArea != CurrentArea)
            {
                return;
            }

            var controller = _sprite.GetComponent<EnemyController>();

            if (controller == null)
            {
                return;
            }

            if (!(parameter is Entity attacker))
            {
                return;
            }

            var attitude = GetAttitudeTowards(victim);

            if (attitude == Attitude.Allied)
            {
                controller.GetAngryAt(attacker);
            }
            else if (attitude == Attitude.Hostile)
            {
                controller.GetAngryAt(victim);
            }
        }
        else if (eventName.Equals(GlobalHelper.EffectDoneEventName))
        {
            var effect = (Effect)broadcaster;

            if (effect == null || !_currentEffects.Contains(effect))
            {
                return;
            }

            _currentEffects.Remove(effect);
        }
    }

    public void CreateFluff(EntityTemplate template)
    {
        Fluff = new EntityFluff(template.Type, template.NameFiles);
    }

    public void CreateFluff(EntityTemplate template, string factionName)
    {
        Fluff = new EntityFluff(template.Type, factionName, template.NameFiles);
    }
    
    //todo need some kind of cap that varies depending on entity
    //todo raise cap as necessary for others
    //todo a "hero type" would have up to a legendary
    //todo a named character read about in books requires a legendary item
    //todo possibly base on background
    //todo might use a capper to limit number of rarest items. Test without first
    public void GenerateStartingEquipment(ItemRarity rarityCap = ItemRarity.Uncommon)
    {
        const int minEquippedItems = 5;
        const int maxEquippedItems = 6;
        const int minInventoryItems = 3;
        const int maxInventoryItems = 7;

        var numEquippedItems = Random.Range(minEquippedItems, maxEquippedItems + 1);

        var emptyEquipmentSlots = Equipped.Keys.ToList();

        for (var i = 0; i < numEquippedItems; i++)
        {
            if (emptyEquipmentSlots.Count < 1)
            {
                break;
            }

            var currentEquipmentSlot = emptyEquipmentSlots[Random.Range(0, emptyEquipmentSlots.Count)];

            var itemForSlot = ItemStore.Instance.GetRandomItemByEquipmentSlot(currentEquipmentSlot, rarityCap);

            EquipItem(itemForSlot, currentEquipmentSlot);

            emptyEquipmentSlots.Remove(currentEquipmentSlot);
        }

        var numInventoryItems = Random.Range(minInventoryItems, maxInventoryItems + 1);

        var items = ItemStore.Instance.GetRandomItems(numInventoryItems, rarityCap);

        foreach(var item in items)
        {
            Inventory.Add(item.Id, item);
        }
    }

    public BodyPart BodyPartHit()
    {
        var bodyPartsDeck = new BodyPartDeck(Body.Values.ToList());

        var dice = new Dice(1, 100);

        BodyPart part = null;
        var partHit = false;
        var triesToHit = 0;
        while (!partHit && triesToHit < Body.Values.Count)
        {
            part = bodyPartsDeck.Draw();

            var roll = DiceRoller.Instance.RollDice(dice);

            var chanceToHit = part.Coverage / (float)TotalBodyPartCoverage * 100;

            partHit = roll <= chanceToHit;

            triesToHit++;
        }

        return part;
    }

    public void RangedAttack(Entity target, Weapon equippedRangedWeapon)
    {
        //todo get weapon here so bonuses can be applied and you can just as a param to ranged hit and apply damage

        if (RangedHit(target, equippedRangedWeapon))
        {
            ApplyRangedDamage(target, equippedRangedWeapon);

            if (!target.IsDead())
            {
                TargetReactToAttacker(target);
                return;
            }

            var message = $"{(target.Fluff != null ? target.Fluff.Name : target.EntityType)} died!";

            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                GlobalHelper.Capitalize(message));
        }
        else
        {
            TargetReactToAttacker(target);

            var message = string.Empty;

            if (IsPlayer())
            {
                message = $"You missed {(target.Fluff != null ? target.Fluff.Name : target.EntityType)}!";
            }
            else if (target.IsPlayer())
            {
                message = $"{(Fluff != null ? Fluff.Name : EntityType)} missed you!";
            }

            if (!string.IsNullOrEmpty(message))
            {
                EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                    GlobalHelper.Capitalize(message));
            }
        }
    }

    public void RangedAttackAOE(List<Tile> aoeTiles, Weapon aoeWeapon)
    {
        var targets = new List<Entity>();

        foreach (var tile in aoeTiles)
        {
            var presentEntity = tile.GetPresentEntity();

            if (presentEntity != null)
            {
                targets.Add(presentEntity);
            }
        }

        foreach (var target in targets)
        {
            RangedAttack(target, aoeWeapon);
        }
    }

    public bool HasMissileWeaponsEquipped()
    {
        var equippedRangedWeapons = GetEquippedMissileWeapon();
        return equippedRangedWeapons != null;
    }

    public bool HasThrownWeaponEquipped()
    {
        var equippedRangedWeapons = GetEquippedThrownWeapon();
        return equippedRangedWeapons != null;
    }

    public bool EquippedMissileWeaponsInRangeOfTarget(Entity target)
    {
        var equippedRangedWeapon = GetEquippedMissileWeapon();

        if (equippedRangedWeapon == null)
        {
            //todo log this as an error.
            //todo This method would have been called after verifying there were ranged weapons equipped.
            return false;
        }

        return equippedRangedWeapon.Range >= CalculateDistanceToTarget(target);
    }

    public bool ThrownWeaponInRangeOfTarget(Entity target)
    {
        const int baseThrowRange = 6;

        var equippedThrownWeapon = GetEquippedThrownWeapon();

        if (equippedThrownWeapon == null)
        {
            return false;
        }

        return baseThrowRange >= CalculateDistanceToTarget(target);
    }

    public Attitude GetAttitudeTowards(Entity target)
    {
        if (target == null)
        {
            return Attitude.Neutral;
        }
        if (target == this)
        {
            return Attitude.Allied;
        }

        var reputation = GetReputationStateForTarget(target);

        if (reputation == ReputationState.Loved)
        {
            return Attitude.Allied;
        }
        if (reputation == ReputationState.Hated)
        {
            return Attitude.Hostile;
        }

        return Attitude.Neutral;
    }

    //todo maybe have some chance to inherit parent's compatible ability
    private AbilityDictionary BuildAbilityDictionary()
    {
        var abilities = new AbilityDictionary();

        //todo choose random free ability for now
        var freeAbility = AbilityStore.ChooseRandomFreeAbility(this);

        if (freeAbility != null)
        {
            abilities.Add(freeAbility.Name, freeAbility);
        }

        var startingBodyPartAbilities = new List<AbilityTemplate>();

        foreach (var bodyPart in Body.Values)
        {
            var partAbilities = AbilityStore.GetAbilitiesByBodyPart(bodyPart.Name);

            if (partAbilities == null)
            {
                continue;
            }

            foreach (var ability in partAbilities)
            {
                if (ability.StartingAbility) startingBodyPartAbilities.Add(ability);
            }
        }

        foreach (var ability in startingBodyPartAbilities)
        {
            if (abilities.ContainsKey(ability.Name))
            {
                continue;
            }

            abilities.Add(ability.Name, AbilityStore.CreateAbility(ability, this));
        }

        var startingBackgroundAbilities = AbilityStore.GetAbilitiesByBackground(Fluff.BackgroundType);

        foreach (var ability in startingBackgroundAbilities)
        {
            if (abilities.ContainsKey(ability.Name) || !ability.StartingAbility)
            {
                continue;
            }

            abilities.Add(ability.Name, AbilityStore.CreateAbility(ability, this));
        }

        var startingDamageTypeAbilities = AbilityStore.GetAllDamageTypeAbilities();

        foreach (var damageType in startingDamageTypeAbilities.Keys)
        {
            foreach (var ability in startingDamageTypeAbilities[damageType])
            {
                if (abilities.ContainsKey(ability.Name) || !ability.StartingAbility)
                {
                    continue;
                }

                abilities.Add(ability.Name, AbilityStore.CreateAbility(ability, this));
            }
        }

        return abilities;
    }

    private ReputationState GetReputationStateForTarget(Entity target)
    {
        var factionReputationValue = 0;
        if (Faction != null && target.Faction != null)
        {
            factionReputationValue = Faction.FactionReputation.GetReputationValueForGroup(target.Faction.Name);
        }

        var entityTypeReputationValue = EntityReputation.GetReputationValueForGroup(target.EntityType);

        var reputationValueTotal = factionReputationValue + entityTypeReputationValue;

        return EntityReputation.GetReputationStateForValue(reputationValueTotal);
    }

    private bool MeleeRollHit(Entity target)
    {
        var roll = DiceRoller.Instance.RollDice(new Dice(1, 100));

        //Rolling a one always misses
        if (roll == 1)
        {
            return false;
        }

        var chanceToHit = GetChanceToHitMeleeTarget(target);

        return roll <= chanceToHit;
    }

    private List<Weapon> GetEquippedMeleeWeapons()
    {
        var meleeSlots = new List<EquipmentSlot>
        {
            EquipmentSlot.LeftHandOne,
            EquipmentSlot.LeftHandTwo,
            EquipmentSlot.RightHandOne,
            EquipmentSlot.RightHandTwo
        };

        var meleeWeapons = new List<Weapon>();

        foreach (var slot in meleeSlots)
        {
            if (!Equipped.ContainsKey(slot))
            {
                continue;
            }

            if (Equipped[slot] != null)
            {
                meleeWeapons.Add((Weapon) Equipped[slot]);
            }
        }

        return meleeWeapons;
    }

    private void ApplyMeleeDamage(Entity target)
    {
        var unarmedDamageDice = new Dice(1, 4);

        var equippedMeleeWeapons = GetEquippedMeleeWeapons();

        var damageDice = new List<Dice>();
        if (equippedMeleeWeapons != null && equippedMeleeWeapons.Count > 0)
        {
            foreach (var weapon in equippedMeleeWeapons)
            {
                damageDice.Add(weapon.ItemDice);
            }
        }
        else
        {
            damageDice.Add(unarmedDamageDice);
        }

        var damageRoll = 0;

        foreach (var dice in damageDice)
        {
            var roll = DiceRoller.Instance.RollDice(dice);
            damageRoll += roll;
        }

        ApplyDamage(target, damageRoll);

        EventMediator.Instance.Broadcast("MeleeHit", this);
    }

    private bool RangedHit(Entity target, Weapon equippedRangedWeapon)
    {
        var roll = DiceRoller.Instance.RollDice(new Dice(1, 100));

        var chanceToHit = CalculateChanceToHitRanged(target, equippedRangedWeapon);

        return roll <= chanceToHit;
    }

    private int CalculateChanceToHitRanged(Entity target, Weapon equippedRangedWeapon)
    {
        //todo apply any bonuses from equipped weapons

        const int startingChanceToHit = 60;

        var chanceToHit = startingChanceToHit;

        var distanceToTarget = CalculateDistanceToTarget(target);

        //- 10 if in melee range
        //+ 3 if within 6 tiles, but not in melee range
        if (distanceToTarget < 2)
        {
            chanceToHit -= 10;
        }
        else if (distanceToTarget < 6)
        {
            chanceToHit += 3;
        }

        var currentTurn = GameManager.Instance.TurnNumber;

        //+ 10 if attacker has not moved in last two turns
        //- 10 otherwise
        if (currentTurn - _lastTurnMoved > 2)
        {
            chanceToHit += 10;
        }
        else
        {
            chanceToHit -= 10;
        }

        //- 8 if defender moved last turn
        if (target.MovedLastTurn(currentTurn))
        {
            chanceToHit -= 8;
        }

        return chanceToHit;
    }

    public int GetChanceToHitRangedTarget(Entity target)
    {
        var equippedMissileWeapons = GetEquippedMissileWeapon();

        return CalculateChanceToHitRanged(target, equippedMissileWeapons);
    }

    public int GetChanceToHitMeleeTarget(Entity target)
    {
        var chanceToHit = 100 - target.Defense;

        //todo unarmed for testing. Will check for equipped weapon and add appropriate bonuses
        const int unarmedBaseToHit = 3;
        chanceToHit += unarmedBaseToHit;

        return chanceToHit;
    }

    public bool MovedLastTurn(int currentTurn)
    {
        return currentTurn - _lastTurnMoved <= 1;
    }

    public int CalculateDistanceToTarget(Entity target)
    {
        var a = target.CurrentTile.X - CurrentTile.X;
        var b = target.CurrentTile.Y - CurrentTile.Y;

        return (int) Math.Sqrt(a * a + b * b);
    }

    private void ApplyRangedDamage(Entity target, Weapon equippedRangedWeapon)
    {
        var damageDice = equippedRangedWeapon.ItemDice;

        var damageRoll = DiceRoller.Instance.RollDice(damageDice);

        ApplyDamage(target, damageRoll);
    }

    public void ApplyDamage(Entity target, int damage)
    {
        var hitBodyPart = target.BodyPartHit();

        target.CurrentHp -= damage;
        hitBodyPart.CurrentHp = hitBodyPart.CurrentHp - damage < 1 ? 0 : hitBodyPart.CurrentHp - damage;

        var message = string.Empty;

        if (IsPlayer())
        {
            message = $"You hit {(target.Fluff != null ? target.Fluff.Name : target.EntityType)}'s {hitBodyPart.Type} for {damage} hit points.";
        }
        else if (target.IsPlayer())
        {
            message = $"{(Fluff != null ? Fluff.Name : EntityType)} hit your {hitBodyPart.Type} for {damage} hit points.";
        }

        if (!string.IsNullOrEmpty(message))
        {
            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, GlobalHelper.Capitalize(message));
        }
    }

    public void ApplyRecurringDamage(int damage)
    {
        CurrentHp -= damage;
    }

    private void TargetReactToAttacker(Entity target)
    {
        target.GetSprite().GetComponent<EnemyController>()?.ReactToAttacker(this);
    }

    public Weapon GetEquippedMissileWeapon()
    {
        return (Weapon) Equipped[EquipmentSlot.MissileWeaponOne]; 
    }

    public Weapon GetEquippedThrownWeapon()
    {
        return (Weapon) Equipped[EquipmentSlot.Thrown];
    }
    
    //<Summary>
    // Picks up any pizza topping occupying player's current tile
    // This method assumes only one topping per tile
    //</Summary>
    private void AutoPickupToppingsInCurrentTile()
    {
        if (CurrentTile.PresentTopping == null)
        {
            return;
        }

        ToppingCounts[CurrentTile.PresentTopping.Type] += 1;

        var message = $"Picked up {CurrentTile.PresentTopping.Type}";

        EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, message);

        if (CurrentTile.PresentItems.Count < 1 && CurrentTile.PresentTopping.WorldSprite != null)
        {
            Object.Destroy(CurrentTile.PresentTopping.WorldSprite);
        }
        CurrentTile.PresentTopping = null;
    }

    private void AutoHarvestFields()
    {
        if (!(CurrentTile.PresentProp is Field))
        {
            return;
        }

        var field = (Field) CurrentTile.PresentProp;
        
        CurrentTile.PresentTopping = new Topping(field.Type.ToString());

        ToppingCounts[CurrentTile.PresentTopping.Type] += 1;

        var message = $"Picked up {CurrentTile.PresentTopping.Type}";

        EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, message);

        if (CurrentTile.PresentItems.Count < 1 && CurrentTile.PresentTopping.WorldSprite != null)
        {
            Object.Destroy(CurrentTile.PresentTopping.WorldSprite);
        }

        Object.Destroy(CurrentTile.PresentProp.Texture);
        CurrentTile.PresentProp = null;
        CurrentTile.PresentTopping = null;
    }

    private void AutoHarvestCheeseTree()
    {
        if (!(CurrentTile.PresentProp is CheeseTree))
        {
            return;
        }

        CurrentTile.PresentTopping = new Topping("Cheese");

        ToppingCounts[CurrentTile.PresentTopping.Type] += 1;

        var message = $"Picked up {CurrentTile.PresentTopping.Type}";

        EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, message);

        if (CurrentTile.PresentItems.Count < 1 && CurrentTile.PresentTopping.WorldSprite != null)
        {
            Object.Destroy(CurrentTile.PresentTopping.WorldSprite);
        }

        Object.Destroy(CurrentTile.PresentProp.Texture);
        CurrentTile.PresentProp = null;
        CurrentTile.PresentTopping = null;
    }

    private void SubscribeToBaseEvents()
    {
        var baseEvents = new List<string>
        {
            GlobalHelper.UnderAttackEventName,
            GlobalHelper.EffectDoneEventName
        };

        _eventMediator = EventMediator.Instance;

        foreach (var baseEvent in baseEvents)
        {
            _eventMediator.SubscribeToEvent(baseEvent, this);
        }
    }

    public void UnsubscribeFromAllEvents()
    {
        if (_eventMediator == null)
        {
            _eventMediator = EventMediator.Instance;
        }

        _eventMediator.UnsubscribeFromAllEvents(this);
    }

    public void PositionRevealed()
    {
        SetSpritePosition(_currentPosition);
    }

    public void Heal(int amount)
    {
        CurrentHp += amount;

        if (CurrentHp > MaxHp)
        {
            CurrentHp = MaxHp;
        }
    }

    public void ApplyEffect(string effectName, int duration, int amount, GoalDirection direction = GoalDirection.North)
    {
        //todo we should be able to have the effect handle the specifics and just add to current effects.
        //todo change to dictionary so it's not this ugly switch statement

        switch (effectName)
        {
            case "heal":
                var healEffect = new Healing(duration, amount, this);

                if (duration < 0)
                {
                    return;
                }

                _currentEffects.Add(healEffect); //todo event
                break;
            case "daze":
                var dazeEffect = new Daze(duration, this);

                if (duration < 0)
                {
                    return;
                }

                _currentEffects.Add(dazeEffect);
                break;
            case "push":
                if (amount < 1)
                {
                    var pushEffect = new Push(this, direction);
                }
                else
                {
                    var pushEffect = new Push(this, direction, amount);
                }
                break;
            case "bleed":
                var bleedEffect = new Bleed(duration, amount, this);

                if (duration < 0)
                {
                    return;
                }

                _currentEffects.Add(bleedEffect);
                break;
        }
    }

    public void ApplyEffect(Effect effect)
    {
        _currentEffects.Add(effect);

        effect.Apply(this);
    }
}
