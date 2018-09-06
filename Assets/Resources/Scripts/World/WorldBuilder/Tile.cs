﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    private Entity _presentEntity;
    private Prop _presentProp;
    private Item _presentItem;
    private GameObject _prefabTexture;
    private Vector2 _gridPosition;

    private bool _blocksMovement;
    private bool _blocksLight;

    public GameObject TextureInstance;


    public Tile(){}

	public Tile(GameObject texture, Vector2 position, bool blocksMovement, bool blocksLight){
		_prefabTexture = texture;
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

	public void SetGridPosition(Vector2 position){
		_gridPosition = position;
	}

	public Vector2 GetGridPosition(){
		return _gridPosition;
	}

	public void SetPrefabTileTexture(GameObject texture){
		_prefabTexture = texture;
	}

	public GameObject GetPrefabTileTexture(){
		return _prefabTexture;
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
