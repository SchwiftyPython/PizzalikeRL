using System.Collections.Generic;
using UnityEngine;

public class Entity : MoveEntity {

    bool _isPlayer;
	bool _isDead;
    bool _isNameable;
    bool _isHostile;
    bool _canMutate;

    string _type;

    //IDictionary<string, BodyPart> body;

	//Base stats
	int _level;
	int _strength;
	int _agility;
	int _constitution;
	//int intellect;
	//int willpower;
	//int charisma;

	//Stats dependent on base stat values
	int _hp;
	int _speed;
    int _defense; //for testing combat

	//Inventory stuff
	List<Item> _inventory;
	List<Item> _equipped;
    List<BodyPart> _bodyParts;
	int _coins;

	GameObject _sprite;
    //SingleNodeBlocker blocker;

    public Vector3 CurrentPosition;

    public Entity (EntityTemplate template, bool isPlayer) {
        _isPlayer = isPlayer;
        _strength = GenStrength(template.minStrength, template.maxStrength);
        _agility = GenAgility(template.minAgility, template.maxAgility);
        _constitution = GenConstitution(template.minConstitution, template.maxConstitution);
        _isNameable = template.nameable;
        _canMutate = template.canMutate;
        _sprite = Resources.Load(template.spritePath) as GameObject;
        //gen level
        //gen hp
        //gen speed
        //gen defense
        //gen coins
        //gen inventory
        //build body
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
        _hp = 45;
        _speed = 35;
        _defense = 35;
	}

    int GenStrength(int min, int max) {
        return Random.Range(min, max + 1);
    }

    int GenAgility(int min, int max) {
        return Random.Range(min, max + 1);
    }

    int GenConstitution(int min, int max) {
        return Random.Range(min, max + 1);
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
        if (MeleeRollToHit(target)) {
            ApplyMeleeDamage(target);
            if (target.IsDead()) {

            }
        }
    }

    bool MeleeRollToHit(Entity target) {
        int roll = Random.Range(1, 101);

        //unarmed for testing. Will check for equipped weapon and add appropriate bonuses
        int unarmedBaseToHit = 3;
        roll += unarmedBaseToHit;

        if(roll >= target._defense) {
            //target attempt block
            //if blocked, return false
            //else
            return true;
        }
        return false;
    }

    void ApplyMeleeDamage(Entity target) {
        int unarmedDamage = 4;
        target._hp -= unarmedDamage;
    }

    bool IsDead() {
        if(_hp <= 0) {
            return true;
        }
        return false;
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
