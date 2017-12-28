using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MoveEntity {
    private bool _isPlayer;
    private bool _isDead;
    private bool _isNameable;
    private bool _isHostile;
    private bool _canMutate;

    private string _type;

	//Base stats
    private int _level;

    private int _strength;
    private int _agility;

    private int _constitution;
	//int intellect;
	//int willpower;
	//int charisma;

	//Stats dependent on base stat values
    private int _maxHP;
    private int _currentHP;

    private int _speed;
    private int _defense; //for testing combat
	
    private List<Item> _inventory;

    private List<Item> _equipped;
    private IDictionary<string, BodyPart> _body = new Dictionary<string, BodyPart>();
    private int _coins;

    private GameObject _sprite;
    //SingleNodeBlocker blocker;

    public Vector3 CurrentPosition;

    public Entity (EntityTemplate template, bool isPlayer) {
        _isPlayer = isPlayer;
        _type = template.type;
        _strength = GenStrength(template.minStrength, template.maxStrength);
        _agility = GenAgility(template.minAgility, template.maxAgility);
        _constitution = GenConstitution(template.minConstitution, template.maxConstitution);
        _isNameable = template.nameable;
        _canMutate = template.canMutate;
        _sprite = Resources.Load(template.spritePath) as GameObject;
        //TODO: gen level
        _level = 1;
        _currentHP =_maxHP = GenMaxHp();
        _speed = GenSpeed();
        _defense = GenDefense();
        //TODO: gen coins
        //TODO: gen inventory
        BuildBody(template);
        //equip
        //gen name
    }

    //Testing constructor
	public Entity(bool isPlayer, GameObject sprite){
		_isPlayer = isPlayer;
		_sprite = sprite;
		_isDead = false;
		_inventory = new List<Item>();
        //blocker = sprite.GetComponent<SingleNodeBlocker>();

        //roll stats based on xml file info
        //hard coded stats for combat testing        
        _maxHP = 45;
        _speed = 35;
        _defense = 35;
	}

    private int GenStrength(int min, int max) {
        return Random.Range(min, max + 1);
    }

    private int GenAgility(int min, int max) {
        return Random.Range(min, max + 1);
    }

    private int GenConstitution(int min, int max) {
        return Random.Range(min, max + 1);
    }

    private int GenMaxHp() {
        //temp for testing
        return (_level + _constitution) * 10;
    }

    private int GenSpeed() {
        //temp for testing
        return (_level + _agility) * 5;
    }

    private int GenDefense() {
        //temp for testing
        return (_level + _agility + _constitution) * 2;
    }

    private void BuildBody(EntityTemplate template) {
        var bodyPartNames = BodyPartLoader._bodyPartNames;
        foreach (var templateBodyPart in template.parts) {
            var part = BodyPartLoader.GetBodyPart(templateBodyPart);
            if (!bodyPartNames.Contains(templateBodyPart)) {
                return;
            }
            if (part.needsPart.Equals("")) {
                _body.Add(part.name, part);
            }
            else if (_body.ContainsKey(part.needsPart)) {
                _body.Add(part.name, part);
            }
            else {
                Debug.Log(part.name + " missing required part " + part.needsPart);
            }
        }
    }

    public bool IsPlayer() {
        return _isPlayer;
    }

	public GameObject GetSprite(){
		return _sprite;
	}

	public void SetSprite(GameObject sprite){
		_sprite = sprite;
	}

	public void SetSpritePosition(Vector3 newPosition){
		_sprite.transform.position = newPosition;
	}   

    public override void Move(Vector2 target) {
        //todo: clean up this code
        StartTile = CurrentPosition;
        EndTile = target;
        var currentArea = GameManager.Instance.CurrentAreaPosition;
        var currentCell = GameManager.Instance.CurrentCellPosition;

//        Debug.Log("entity.currentPosition before move: " + CurrentPosition.x + " " + CurrentPosition.y);
//        Debug.Log("sprite.currentPosition before move: " + _sprite.transform.position.x + " " + _sprite.transform.position.y);
//        Debug.Log("start: " + StartTile);
//        Debug.Log("End: " + EndTile);

        if (TileOutOfBounds(target)) {
            var direction = AreaOutOfBoundsDirection(target, currentArea);
            var nextArea = new Vector2(currentArea.X + Directions[direction].x,
                                       currentArea.Y + Directions[direction].y);
            if (AreaOutOfBounds(nextArea)) {
                direction = CellOutOfBoundsDirection(nextArea, currentCell);
                var nextCell = new Vector2(currentCell.X + Directions[direction].x,
                                           currentCell.Y + Directions[direction].y);
                if (CellOutOfBounds(nextCell)) {
                    Debug.Log("Cannot move. Edge of map.");
                    return;
                } else {
                    currentCell = WorldData.Instance.Map[currentCell.X + (int)Directions[direction].x,
                                                         currentCell.Y + (int)Directions[direction].y];

                    currentArea = CalculateCellEntryArea(nextArea);

                    if (!currentArea.AreaBuilt())
                    {
                        currentArea.BuildArea();
                    }
                    GameManager.Instance.CurrentTilePosition = CalculateAreaEntryTile(target);

                    //update tile data for start and end tiles
                    UpdateTileData(currentArea.AreaTiles[(int)StartTile.x, (int)StartTile.y],
                                   currentArea.AreaTiles[(int)GameManager.Instance.CurrentTilePosition.GetGridPosition().x,
                                                         (int)GameManager.Instance.CurrentTilePosition.GetGridPosition().y]);

                    CurrentPosition = new Vector3((int)GameManager.Instance.CurrentTilePosition.GetGridPosition().x,
                                                  (int)GameManager.Instance.CurrentTilePosition.GetGridPosition().y);
                    if (_isPlayer) { GameManager.Instance.Player.CurrentPosition = CurrentPosition; }
                    GameManager.Instance.CurrentAreaPosition = currentArea;
                    GameManager.Instance.CurrentCellPosition = currentCell;
                    AreaMap.Instance.EnterArea();
                }

            } else {
                //move to next area but not next cell
                currentArea = currentCell.Areas[currentArea.X + (int) Directions[direction].x,
                                                currentArea.Y + (int) Directions[direction].y];

                if (!currentArea.AreaBuilt())
                {
                    currentArea.BuildArea();
                }
                //calc area entry tile
                GameManager.Instance.CurrentTilePosition = CalculateAreaEntryTile(target);

                //update tile data for start and end tiles
                CurrentPosition = new Vector3((int)GameManager.Instance.CurrentTilePosition.GetGridPosition().x,
                                              (int)GameManager.Instance.CurrentTilePosition.GetGridPosition().y);
                UpdateTileData(currentArea.AreaTiles[(int)StartTile.x, (int)StartTile.y],
                               currentArea.AreaTiles[(int)GameManager.Instance.CurrentTilePosition.GetGridPosition().x,
                                                     (int)GameManager.Instance.CurrentTilePosition.GetGridPosition().y]);
                if (_isPlayer) { GameManager.Instance.Player.CurrentPosition = CurrentPosition; }
                GameManager.Instance.CurrentAreaPosition = currentArea;
                AreaMap.Instance.EnterArea();
            }
        } else {
            CurrentPosition = EndTile;
            if (_isPlayer) { GameManager.Instance.Player.CurrentPosition = CurrentPosition; }
            SetSpritePosition(EndTile);

            //update tile data for start and end tiles
            UpdateTileData(currentArea.AreaTiles[(int)StartTile.x, (int)StartTile.y], 
                           currentArea.AreaTiles[(int)EndTile.x, (int)EndTile.y]);
        }

        Debug.Log("entity.currentPosition after move: " + CurrentPosition.x + " " + CurrentPosition.y);
        Debug.Log("sprite.currentPosition after move: " + _sprite.transform.position.x + " " + _sprite.transform.position.y);
    }

    public void UpdateTileData(Tile startTile, Tile endTile)
    {
        startTile.SetBlocksMovement(false);
        startTile.SetPresentEntity(null);

        endTile.SetBlocksMovement(true);
        endTile.SetPresentEntity(this);
    }

    public override bool TileOutOfBounds(Vector2 target) {
        var currentArea = GameManager.Instance.CurrentAreaPosition;
        return target.x >= currentArea.Width || target.x < 0 || target.y >= currentArea.Height || target.y < 0;
    }

    public bool AreaOutOfBounds(Vector2 target) {
        var currentCell = GameManager.Instance.CurrentCellPosition;
        return target.x >= currentCell.GetCellWidth() || target.x < 0 || target.y >= currentCell.GetCellHeight() || target.y < 0;
    }

    public bool CellOutOfBounds(Vector2 target) {
        return target.x >= WorldData.Instance.Width || target.x < 0 || target.y >= WorldData.Instance.Height || target.y < 0;
    }

    public Tile CalculateAreaEntryTile(Vector2 target) {
        var xOffset = 0;
        var yOffset = 0;
        if (target.x >= GameManager.Instance.CurrentAreaPosition.Width) {
            xOffset = -GameManager.Instance.CurrentAreaPosition.Width;
        }else if (target.x < 0) {
            xOffset = GameManager.Instance.CurrentAreaPosition.Width;
        }
        if (target.y >= GameManager.Instance.CurrentAreaPosition.Height)
        {
            yOffset = -GameManager.Instance.CurrentAreaPosition.Height;
        }
        else if (target.y < 0)
        {
            yOffset = GameManager.Instance.CurrentAreaPosition.Height;
        }
        return GameManager.Instance.CurrentAreaPosition.AreaTiles[(int)target.x + xOffset, (int)target.y + yOffset];
    }

    public Area CalculateCellEntryArea(Vector2 target) {
        var xOffset = 0;
        var yOffset = 0;
        if (target.x >= GameManager.Instance.CurrentCellPosition.GetCellWidth()) {
            xOffset = -GameManager.Instance.CurrentCellPosition.GetCellWidth();
        }else if (target.x < 0) {
            xOffset = GameManager.Instance.CurrentCellPosition.GetCellWidth();
        }
        if (target.y >= GameManager.Instance.CurrentCellPosition.GetCellHeight())
        {
            yOffset = -GameManager.Instance.CurrentCellPosition.GetCellHeight();
        }
        else if (target.y < 0)
        {
            yOffset = GameManager.Instance.CurrentCellPosition.GetCellHeight();
        }
        return GameManager.Instance.CurrentCellPosition.Areas[(int)target.x + xOffset, (int)target.y + yOffset];
    }

    public Direction AreaOutOfBoundsDirection(Vector2 target, Area area) {
        if (target.x >= area.Width) {
            if (target.y >= area.Height) {
                return Direction.NorthEast;
            }
            return target.y < 0 ? Direction.SouthEast : Direction.East;
        }
        if (!(target.x < 0)){
            return target.y < 0 ? Direction.South : Direction.North;
        }
        if (target.y >= area.Height)
        {
            return Direction.NorthWest;
        }
        return target.y < 0 ? Direction.SouthWest : Direction.West;
    }

    public Direction CellOutOfBoundsDirection(Vector2 target, Cell cell){
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

    public void MeleeAttack(Entity target) {
        if (MeleeRollHit(target)) {
            ApplyMeleeDamage(target);
            if (target.IsDead()) {
                //TODO: remove target
            }
        }
    }

    public bool MoveOrAttackSuccessful(Vector2 target)
    {
        if (CanMove(target))
        {
            Move(target);
            return true;
        }
        if (EntityPresent(target))
        {
            MeleeAttack(GameManager.Instance.CurrentAreaPosition.GetTileAt(target).GetPresentEntity());
            return true;
        }
        return false;
    }

    public bool CanMove(Vector2 target){
        var currentArea = GameManager.Instance.CurrentAreaPosition;
        var currentCell = GameManager.Instance.CurrentCellPosition;
        if (!TileOutOfBounds(target) && !TargetTileBlocked(target)){
            return true;
        }
        var areaDirectionToCheck = Directions[AreaOutOfBoundsDirection(target, currentArea)];
        var nextArea = new Vector2(currentArea.X + areaDirectionToCheck.x, 
                                   currentArea.Y + areaDirectionToCheck.y);
        if (!AreaOutOfBounds(nextArea)){
            return true;
        }
        var cellDirectionToCheck = Directions[CellOutOfBoundsDirection(nextArea, currentCell)];
        var nextCell = new Vector2(currentCell.X + cellDirectionToCheck.x,
                                   currentCell.Y + cellDirectionToCheck.y);
        return !CellOutOfBounds(nextCell);
    }

    public void MoveToNextArea(Vector2 target) {
        var currentArea = GameManager.Instance.CurrentAreaPosition;
        var currentCell = GameManager.Instance.CurrentCellPosition;
        var areaDirection = Directions[AreaOutOfBoundsDirection(target, currentArea)];
        var nextArea = new Vector2(currentArea.X + areaDirection.x,
                                   currentArea.Y + areaDirection.y);
        GameManager.Instance.CurrentAreaPosition = currentCell.Areas[(int)nextArea.x, (int)nextArea.y];
    }

    public void MoveToNextCell(Vector2 target){
       
        var currentCell = GameManager.Instance.CurrentCellPosition;
        var cellDirection = Directions[CellOutOfBoundsDirection(target, currentCell)];
        var nextArea = new Vector2(currentCell.X + cellDirection.x,
                                   currentCell.Y + cellDirection.y);
        GameManager.Instance.CurrentAreaPosition = currentCell.Areas[(int)nextArea.x, (int)nextArea.y];
    }

    public bool EntityPresent(Vector2 target)
    {
        return TargetTileBlockedByEntity(target);
    }

    private bool MeleeRollHit(Entity target) {
        var roll = Random.Range(1, 101);

        //unarmed for testing. Will check for equipped weapon and add appropriate bonuses
        const int unarmedBaseToHit = 3;
        roll += unarmedBaseToHit;

        return roll >= target._defense;
    }

    private void ApplyMeleeDamage(Entity target) {
        var unarmedDamage = 4;
        target._currentHP -= unarmedDamage;
    }

    private bool IsDead() {
        return _currentHP <= 0;
    }
    
}
