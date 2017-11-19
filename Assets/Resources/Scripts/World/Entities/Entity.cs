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
        startTile = CurrentPosition;
        endTile = target;
        //RaycastHit2D hit = new RaycastHit2D();

        Debug.Log("entity.currentPosition before move: " + CurrentPosition.x + " " + CurrentPosition.y);
        Debug.Log("sprite.currentPosition before move: " + _sprite.transform.position.x + " " + _sprite.transform.position.y);
        Debug.Log("start: " + startTile);
        Debug.Log("End: " + endTile);

        
        CurrentPosition = endTile;
        SetSpritePosition(endTile);

        //update tile data for start and end tiles
        var tileToUpdate = GameManager.Instance.CurrentAreaPosition.AreaTiles[(int)startTile.x, (int)startTile.y];
        tileToUpdate.SetBlocksMovement(false);
        tileToUpdate.SetPresentEntity(null);

        tileToUpdate = GameManager.Instance.CurrentAreaPosition.AreaTiles[(int)endTile.x, (int)endTile.y];
        tileToUpdate.SetBlocksMovement(true);
        tileToUpdate.SetPresentEntity(this);

        Debug.Log("entity.currentPosition after move: " + CurrentPosition.x + " " + CurrentPosition.y);
        Debug.Log("sprite.currentPosition after move: " + _sprite.transform.position.x + " " + _sprite.transform.position.y);
    }

    public void MeleeAttack(Entity target) {
        if (MeleeRollHit(target)) {
            ApplyMeleeDamage(target);
            if (target.IsDead()) {
                //remove target
            }
        }
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

    /*
   public override bool MoveSuccessful(Vector2 end) {
       startTile = this.currentPosition;
       endTile = end;
       //RaycastHit2D hit = new RaycastHit2D();

       Debug.Log("entity.currentPosition: " + this.currentPosition.x + " " + this.currentPosition.y);
       Debug.Log("start: " + startTile);
       Debug.Log("End: " + endTile);

       if (!WorldManager.instance.GetTileAt(endTile).GetBlocksMovement()) {
           this.currentPosition = endTile;
           this.SetSpritePosition(endTile);

           //update tile data for start and end tiles
           Tile tileToUpdate = WorldManager.instance.GetTileAt(startTile);
           tileToUpdate.SetBlocksMovement(false);
           tileToUpdate.SetPresentEntity(null);

           tileToUpdate = WorldManager.instance.GetTileAt(endTile);
           tileToUpdate.SetBlocksMovement(true);
           tileToUpdate.SetPresentEntity(this);
           Debug.Log("move successful. Tile Blocked.");
           return true;
       } else {
           Debug.Log("move unsuccessful");
           return false;
       }
   }
   */
}
