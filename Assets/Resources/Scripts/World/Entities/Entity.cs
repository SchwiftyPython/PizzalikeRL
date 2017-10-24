using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Entity : MoveEntity {

    bool isPlayer;
	bool isDead;
    bool isNameable;
    bool isHostile;
    bool canMutate;

    string type;

    //IDictionary<string, BodyPart> body;

	//Base stats
	int level;
	int strength;
	int agility;
	int constitution;
	//int intellect;
	//int willpower;
	//int charisma;

	//Stats dependent on base stat values
	int hp;
	int speed;
    int defense; //for testing combat

	//Inventory stuff
	List<Item> inventory;
	List<Item> equipped;
    List<BodyPart> bodyParts;
	int coins;

	GameObject sprite;
    //SingleNodeBlocker blocker;

    public Vector3 currentPosition;

    public Entity (EntityTemplate template, bool isPlayer) {
        this.isPlayer = isPlayer;
        strength = GenStrength(template.minStrength, template.maxStrength);
        agility = GenAgility(template.minAgility, template.maxAgility);
        constitution = GenConstitution(template.minConstitution, template.maxConstitution);
        isNameable = template.nameable;
        canMutate = template.canMutate;
        sprite = Resources.Load(template.spritePath) as GameObject;
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
		this.isPlayer = isPlayer;
		this.sprite = sprite;
		isDead = false;
		inventory = new List<Item>();
        //blocker = sprite.GetComponent<SingleNodeBlocker>();

        //roll stats based on xml file info
        //hard coded stats for combat testing        
        hp = 45;
        speed = 35;
        defense = 35;
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
        return isPlayer;
    }

	public GameObject GetSprite(){
		return this.sprite;
	}

	public void SetSprite(GameObject sprite){
		this.sprite = sprite;
	}

	public void SetSpritePosition(Vector3 newPosition){
		sprite.transform.position = newPosition;
	}   

    public override void Move(Vector2 target) {
        startTile = this.currentPosition;
        endTile = target;
        //RaycastHit2D hit = new RaycastHit2D();

        Debug.Log("entity.currentPosition: " + this.currentPosition.x + " " + this.currentPosition.y);
        Debug.Log("start: " + startTile);
        Debug.Log("End: " + endTile);

        
        this.currentPosition = endTile;
        this.SetSpritePosition(endTile);

        //update tile data for start and end tiles
        Tile tileToUpdate = WorldManager.Instance.GetTileAt(startTile);
        tileToUpdate.SetBlocksMovement(false);
        tileToUpdate.SetPresentEntity(null);

        tileToUpdate = WorldManager.Instance.GetTileAt(endTile);
        tileToUpdate.SetBlocksMovement(true);
        tileToUpdate.SetPresentEntity(this);        
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

        if(roll >= target.defense) {
            //target attempt block
            //if blocked, return false
            //else
            return true;
        } else {
            return false;
        }
        
    }

    void ApplyMeleeDamage(Entity target) {
        int unarmedDamage = 4;
        target.hp -= unarmedDamage;
    }

    bool IsDead() {
        if(this.hp <= 0) {
            return true;
        } else {
            return false;
        }
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
