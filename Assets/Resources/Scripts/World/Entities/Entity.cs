﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Entity
{
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

    private readonly IDictionary<Direction, Vector2> _directions = new Dictionary<Direction, Vector2>
    {
        {Direction.North, Vector2.up},
        {Direction.NorthEast, Vector2.one},
        {Direction.East, Vector2.right},
        {Direction.SouthEast, new Vector2(1, -1)},
        {Direction.South, Vector2.down},
        {Direction.SouthWest, new Vector2(-1, -1)},
        {Direction.West, Vector2.left},
        {Direction.NorthWest, new Vector2(-1, 1)}
    };

    private Vector2 _startTile;
    private Vector2 _endTile;

    private readonly bool _isPlayer;
    private bool _isDead;
    private bool _isHostile;
    private bool _canMutate;

    private int _coins;

    private readonly GameObject _prefab;
    private GameObject _sprite;

    private readonly string _entityType;
    private readonly string _factionType;

    private int _totalBodyPartCoverage;

    private Vector3 _currentPosition;

    //Base stats

    public int Level { get; private set; }
    public int Xp { get; private set; }

    public int Strength { get; private set; }
    public int Agility { get; private set; }
    public int Constitution { get; private set; }
    public int Intelligence { get; private set; }


    //Stats dependent on base stat values

    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }
    public int Speed { get; private set; }
    public int Defense { get; private set; }

    public IDictionary<Guid, Item> Inventory { get; }
    public IDictionary<BodyPart, Item> Equipped;

    public IDictionary<string, BodyPart> Body { get; } = new Dictionary<string, BodyPart>();

    //SingleNodeBlocker blocker;

    public EntityFluff Fluff { get; set; }

    public Cell CurrentCell;
    public Area CurrentArea;
    public Tile CurrentTile;

    public Vector3 CurrentPosition
    {
        get { return _currentPosition; }

        set
        {
            _currentPosition = value;
            SetSpritePosition(value);
        }
    }

    public Entity(EntityTemplate template, string faction = null, bool isPlayer = false)
    {
        _isPlayer = isPlayer;
        _entityType = template.Type;
        _factionType = faction;

        if (isPlayer)
        {
            Level = 1;
            Xp = 0;
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

        _canMutate = template.CanMutate;

        _prefab = Resources.Load(template.SpritePath) as GameObject;
        
        Inventory = new Dictionary<Guid, Item>();
        BuildBody(template);
        CalculateTotalBodyPartCoverage();
        PopulateEquipped();
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
        return $"{_factionType}  {_entityType}";
    }

    public string GetStatsForEntityInfoWindow()
    {
        return
            $"Current HP: {CurrentHp}\nStrength: {Strength}\nAgility: {Agility}\nConstitution: {Constitution}\nSpeed: {Speed}\nDefense: {Defense}";
    }

    public void EquipItem(Item item, string bodyPartKey)
    {
        Inventory.Remove(item.Id);

        var bodyPart = Body[bodyPartKey];

        if (Equipped[bodyPart].Id != Guid.Empty)
        {
            var oldItem = Equipped[bodyPart];
            Inventory.Add(oldItem.Id, oldItem);
        }

        Equipped[bodyPart] = item;

        EquipmentWindow.Instance.EquipmentChanged = true;
        InventoryWindow.Instance.InventoryChanged = true;
    }

    private void PopulateEquipped()
    {
        Equipped = new Dictionary<BodyPart, Item>();

        foreach (var bodyPart in Body.Values)
        {
            if (!Equipped.ContainsKey(bodyPart))
            {
                Equipped.Add(bodyPart, new Item());
            }
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
        var bodyPartNames = BodyPartLoader.BodyPartNames;
        foreach (var templateBodyPart in template.Parts)
        {
            var part = BodyPartLoader.GetBodyPart(templateBodyPart);
            if (!bodyPartNames.Contains(templateBodyPart))
            {
                return;
            }
            if (part.NeedsPart.Equals(""))
            {
                Body.Add(part.Type, part);
            }
            else if (Body.ContainsKey(part.NeedsPart))
            {
                if (Body[part.NeedsPart].ChildrenBodyParts.Count > Body[part.NeedsPart].MaxChildrenBodyParts)
                {
                    Debug.Log(part.Name + " missing required part " + part.NeedsPart);
                }
                Body.Add(part.Type, part);
            }
            else
            {
                Debug.Log(part.Name + " missing required part " + part.NeedsPart);
            }
        }
    }

    private void CalculateTotalBodyPartCoverage()
    {
        _totalBodyPartCoverage = (from bp in Body.Values select bp.Coverage).Sum();
    }

    public bool IsPlayer()
    {
        return _isPlayer;
    }

    public GameObject GetSpritePrefab()
    {
        return _prefab;
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
        _sprite.transform.position = newPosition;
    }

    public void AreaMove(Vector2 target)
    {
        //todo: clean up this code
        _startTile = CurrentPosition;
        _endTile = target;

//        Debug.Log("entity.currentPosition before move: " + CurrentPosition.x + " " + CurrentPosition.y);
//        Debug.Log("sprite.currentPosition before move: " + _sprite.transform.position.x + " " + _sprite.transform.position.y);
//        Debug.Log("start: " + StartTile);
//        Debug.Log("End: " + EndTile);

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
                    CurrentCell = WorldData.Instance.Map[CurrentCell.X + (int) _directions[direction].x,
                        CurrentCell.Y + (int) _directions[direction].y];

                    CurrentArea = CalculateCellEntryArea(nextAreaPosition);

                    if (!CurrentArea.AreaBuilt())
                    {
                        CurrentArea.BuildArea();
                    }

                    if (IsPlayer())
                    {
                        GameManager.Instance.CurrentTile = CalculateAreaEntryTile(target);
                    }
                    CurrentTile = CalculateAreaEntryTile(target);

                    //update tile data for start and end tiles
                    UpdateTileData(CurrentArea.AreaTiles[(int) _startTile.x, (int) _startTile.y],
                        CurrentArea.AreaTiles[(int) GameManager.Instance.CurrentTile.GetGridPosition().x,
                            (int) GameManager.Instance.CurrentTile.GetGridPosition().y]);

                    CurrentPosition = new Vector3((int) GameManager.Instance.CurrentTile.GetGridPosition().x,
                        (int) GameManager.Instance.CurrentTile.GetGridPosition().y);

                    if (_isPlayer)
                    {
                        GameManager.Instance.Player.CurrentPosition = CurrentPosition;
                        GameManager.Instance.CurrentArea = CurrentArea;
                        GameManager.Instance.CurrentCell = CurrentCell;
                        GameManager.Instance.CurrentState = GameManager.GameState.EnterArea;
                    }
                }
            }
            else
            {
                //move to next area but not next cell
                CurrentArea = CurrentCell.Areas[CurrentArea.X + (int) _directions[direction].x,
                    CurrentArea.Y + (int) _directions[direction].y];

                if (!CurrentArea.AreaBuilt())
                {
                    CurrentArea.BuildArea();
                }

                //calc area entry tile
                if (IsPlayer())
                {
                    GameManager.Instance.CurrentTile = CalculateAreaEntryTile(target);
                }
                CurrentTile = CalculateAreaEntryTile(target);

                //update tile data for start and end tiles
                CurrentPosition = new Vector3((int) GameManager.Instance.CurrentTile.GetGridPosition().x,
                    (int) GameManager.Instance.CurrentTile.GetGridPosition().y);
                UpdateTileData(CurrentArea.AreaTiles[(int) _startTile.x, (int) _startTile.y],
                    CurrentArea.AreaTiles[(int) GameManager.Instance.CurrentTile.GetGridPosition().x,
                        (int) GameManager.Instance.CurrentTile.GetGridPosition().y]);

                if (_isPlayer)
                {
                    GameManager.Instance.Player.CurrentPosition = CurrentPosition;
                    GameManager.Instance.CurrentArea = CurrentArea;
                    GameManager.Instance.CurrentState = GameManager.GameState.EnterArea;
                }
            }
        }
        else
        {
            CurrentPosition = _endTile;
            if (_isPlayer)
            {
                GameManager.Instance.Player.CurrentPosition = CurrentPosition;
            }
            //SetSpritePosition(EndTile);

            //update tile data for start and end tiles
            UpdateTileData(CurrentArea.AreaTiles[(int) _startTile.x, (int) _startTile.y],
                CurrentArea.AreaTiles[(int) _endTile.x, (int) _endTile.y]);
        }

        //Debug.Log("entity.currentPosition after move: " + CurrentPosition.x + " " + CurrentPosition.y);
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

            GameManager.Instance.Player.CurrentPosition = CurrentPosition;

            GameManager.Instance.CurrentCell = WorldData.Instance.Map[(int) targetCell.x, (int) targetCell.y];
            CurrentCell = GameManager.Instance.CurrentCell;

            //Set to middle area of cell
            CurrentArea = CurrentCell.Areas[1, 1];
            GameManager.Instance.CurrentArea = CurrentArea;
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
        return target.x >= CurrentArea.Width || target.x < 0 || target.y >= CurrentArea.Height || target.y < 0;
    }

    public bool AreaOutOfBounds(Vector2 target)
    {
        return target.x >= CurrentCell.GetCellWidth() || target.x < 0 || target.y >= CurrentCell.GetCellHeight() ||
               target.y < 0;
    }

    public bool CellOutOfBounds(Vector2 target)
    {
        return target.x >= WorldData.Instance.Width || target.x < 0 || target.y >= WorldData.Instance.Height ||
               target.y < 0;
    }

    public Tile CalculateAreaEntryTile(Vector2 target)
    {
        var xOffset = 0;
        var yOffset = 0;
        if (target.x >= GameManager.Instance.CurrentArea.Width)
        {
            xOffset = -GameManager.Instance.CurrentArea.Width;
        }
        else if (target.x < 0)
        {
            xOffset = GameManager.Instance.CurrentArea.Width;
        }
        if (target.y >= GameManager.Instance.CurrentArea.Height)
        {
            yOffset = -GameManager.Instance.CurrentArea.Height;
        }
        else if (target.y < 0)
        {
            yOffset = GameManager.Instance.CurrentArea.Height;
        }
        return GameManager.Instance.CurrentArea.AreaTiles[(int) target.x + xOffset, (int) target.y + yOffset];
    }

    public Area CalculateCellEntryArea(Vector2 target)
    {
        var xOffset = 0;
        var yOffset = 0;
        if (target.x >= GameManager.Instance.CurrentCell.GetCellWidth())
        {
            xOffset = -GameManager.Instance.CurrentCell.GetCellWidth();
        }
        else if (target.x < 0)
        {
            xOffset = GameManager.Instance.CurrentCell.GetCellWidth();
        }
        if (target.y >= GameManager.Instance.CurrentCell.GetCellHeight())
        {
            yOffset = -GameManager.Instance.CurrentCell.GetCellHeight();
        }
        else if (target.y < 0)
        {
            yOffset = GameManager.Instance.CurrentCell.GetCellHeight();
        }
        Debug.Log("Original target area:" + target);
        Debug.Log("Calculated target area:" + (int) target.x + xOffset + ", " + (int) target.y + yOffset);
        return GameManager.Instance.CurrentCell.Areas[(int) target.x + xOffset, (int) target.y + yOffset];
    }

    public Direction AreaOutOfBoundsDirection(Vector2 target, Area area)
    {
        if (target.x >= area.Width)
        {
            if (target.y >= area.Height)
            {
                return Direction.NorthEast;
            }
            return target.y < 0 ? Direction.SouthEast : Direction.East;
        }
        if (!(target.x < 0))
        {
            return target.y < 0 ? Direction.South : Direction.North;
        }
        if (target.y >= area.Height)
        {
            return Direction.NorthWest;
        }
        return target.y < 0 ? Direction.SouthWest : Direction.West;
    }

    public Direction CellOutOfBoundsDirection(Vector2 target, Cell cell)
    {
        if (target.x >= cell.GetCellWidth())
        {
            if (target.y >= cell.GetCellHeight())
            {
                return Direction.NorthEast;
            }
            return target.y < 0 ? Direction.SouthEast : Direction.East;
        }
        if (!(target.x < 0))
        {
            return target.y < 0 ? Direction.South : Direction.North;
        }
        if (target.y >= cell.GetCellHeight())
        {
            return Direction.NorthWest;
        }
        return target.y < 0 ? Direction.SouthWest : Direction.West;
    }

    public void MeleeAttack(Entity target)
    {
        if (MeleeRollHit(target))
        {
            ApplyMeleeDamage(target);
            if (!target.IsDead())
            {
                return;
            }
            var message = _entityType + " killed " + target._entityType + "!";
            GameManager.Instance.Messages.Add(message);
            //AreaMap.Instance.RemoveEntity(target);
        }
        else
        {
            var message = _entityType + " missed " + target._entityType + "!";
            GameManager.Instance.Messages.Add(message);
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
                return true;
            }
            if (!EntityPresent(target))
            {
                return false;
            }
            MeleeAttack(GameManager.Instance.CurrentArea.GetTileAt(target).GetPresentEntity());
            return true;
        }
        else if (currentScene.Equals("WorldMap"))
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

    public void CreateFluff()
    {
        Fluff = new EntityFluff(_entityType);
    }

    public BodyPart BodyPartHit()
    {
        var bodyPartsDeck = new BodyPartDeck((List<BodyPart>)Body.Values);

        var dice = new Dice(1, 100);

        BodyPart part = null;
        var partHit = false;
        while (!partHit)
        {
            part = bodyPartsDeck.Draw();

            var roll = DiceRoller.Instance.RollDice(dice);

            var chanceToHit = part.Coverage / _totalBodyPartCoverage;

            partHit = roll <= chanceToHit;
        }

        return part;
    }

    private static bool MeleeRollHit(Entity target)
    {
        var roll = Random.Range(1, 101);

        //Rolling a one always misses
        if (roll == 1)
        {
            return false;
        }

        //unarmed for testing. Will check for equipped weapon and add appropriate bonuses
        const int unarmedBaseToHit = 3;
        roll += unarmedBaseToHit;

        return roll >= target.Defense;
    }

    private void ApplyMeleeDamage(Entity target)
    {
        var unarmedDamageDice = new Dice(1, 4);

        var equippedMeleeWeapon = (Weapon)(from e in Equipped.Values
                                  where e.GetType() == typeof(Weapon) 
                                  && ((Weapon) e).Range < 1
                                  select e).FirstOrDefault();

        var damageDice = equippedMeleeWeapon != null ? equippedMeleeWeapon.ItemDice : unarmedDamageDice;

        var damageRoll = DiceRoller.Instance.RollDice(damageDice);

        var hitBodyPart = target.BodyPartHit();

        target.CurrentHp -= damageRoll;
        hitBodyPart.CurrentHp = hitBodyPart.CurrentHp - damageRoll < 1 ? 0 : hitBodyPart.CurrentHp - damageRoll;

        var message = _entityType + " hits " + target._entityType + " for " + damageRoll + " hit points.";
        GameManager.Instance.Messages.Add(message);

        //Debug.Log("Target remaining hp: " + target.CurrentHp);
    }

    
}
