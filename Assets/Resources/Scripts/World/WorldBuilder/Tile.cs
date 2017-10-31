using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	Entity _presentEntity;
	Prop _presentProp;
	Item _presentItem;
	GameObject _texture;
	Vector3 _gridPosition;

	bool _blocksMovement;
	bool _blocksLight;

	public Tile(){}

	public Tile(GameObject texture, Vector3 position, bool blocksMovement, bool blocksLight){
		_texture = texture;
		_gridPosition = position;
		_blocksMovement = blocksMovement;
		_blocksLight = blocksLight;
	}

	public void SetPresentEntity(Entity entity){
		_presentEntity = entity;
	}

	public Entity GetPresentEntity(){
		return _presentEntity;
	}

	public void SetGridPosition(Vector3 position){
		_gridPosition = position;
	}

	public Vector3 GetGridPosition(){
		return _gridPosition;
	}

	public void SetTileTexture(GameObject texture){
		_texture = texture;
	}

	public GameObject GetTileTexture(){
		return _texture;
	}

	public void SetBlocksMovement(bool blocksMovement){
		_blocksMovement = blocksMovement;
	}

	public bool GetBlocksMovement(){
		return _blocksMovement;
	}

	public void SetBlocksLight(bool blocksLight){
		_blocksLight = blocksLight;
	}

	public bool GetBlocksLight(){
		return _blocksLight;
	}
}
