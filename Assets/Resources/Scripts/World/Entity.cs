using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity  {

	bool isPlayer;
	bool isDead;

	//Base stats
	int level;
	int strength;
	int agility;
	int constitution;
	int intellect;
	int willpower;
	int charisma;

	//Stats dependent on base stat values
	int hp;
	int speed;

	//Inventory stuff
	List<Item> inventory;
	List<Item> equipped;
	int coins;

	GameObject sprite;

	public Vector3 currentPosition;

	public Entity(bool isPlayer, GameObject sprite){
		this.isPlayer = isPlayer;
		this.sprite = sprite;
		isDead = false;
		inventory = new List<Item>();
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
}
