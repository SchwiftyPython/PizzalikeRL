using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	Entity presentEntity;
	Prop presentProp;
	Item presentItem;
	GameObject texture;
	Vector3 gridPosition;

	bool blocksMovement;
	bool blocksLight;

	public Tile(){}

	public Tile(GameObject texture, Vector3 position, bool blocksMovement, bool blocksLight){
		this.texture = texture;
		gridPosition = position;
		this.blocksMovement = blocksMovement;
		this.blocksLight = blocksLight;
	}

	public void SetPresentEntity(Entity entity){
		presentEntity = entity;
	}

	public Entity GetPresentEntity(){
		return this.presentEntity;
	}

	public void SetGridPosition(Vector3 position){
		gridPosition = position;
	}

	public Vector3 GetGridPosition(){
		return gridPosition;
	}

	public void SetTileTexture(GameObject texture){
		this.texture = texture;
	}

	public GameObject GetTileTexture(){
		return this.texture;
	}

	public void SetBlocksMovement(bool blocksMovement){
		this.blocksMovement = blocksMovement;
	}

	public bool GetBlocksMovement(){
		return this.blocksMovement;
	}

	public void SetBlocksLight(bool blocksLight){
		this.blocksLight = blocksLight;
	}

	public bool GetBlocksLight(){
		return this.blocksLight;
	}
}
