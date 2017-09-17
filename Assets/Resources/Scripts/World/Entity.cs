using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Entity : MoveEntity {

	bool isPlayer;
	bool isDead;

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
	int coins;

	GameObject sprite;
    //SingleNodeBlocker blocker;

    public Vector3 currentPosition;

	public Entity(bool isPlayer, GameObject sprite){
		this.isPlayer = isPlayer;
		this.sprite = sprite;
		isDead = false;
		inventory = new List<Item>();
        //blocker = sprite.GetComponent<SingleNodeBlocker>();
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
        Tile tileToUpdate = WorldManager.instance.GetTileAt(startTile);
        tileToUpdate.SetBlocksMovement(false);
        tileToUpdate.SetPresentEntity(null);

        tileToUpdate = WorldManager.instance.GetTileAt(endTile);
        tileToUpdate.SetBlocksMovement(true);
        tileToUpdate.SetPresentEntity(this);        
    }

    public void MeleeAttack(Entity target) {

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
} 
