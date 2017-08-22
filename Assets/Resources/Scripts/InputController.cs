using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	public static InputController instance = null;

	Entity player;
    public bool actionTaken = false; //for basic AI pathfinding testing

	void Start () {
		if (instance == null) {
			instance = this;
		}else if(instance != this){
			// Destroy the current object, so there is just one 
			Destroy(gameObject);
		}

		/*
        POSSIBLE BUG: NOT GETTING REFERENCE TO PLAYER IN START

		//while (WorldManager.instance.player == null) {}
		player = WorldManager.instance.player;

		Debug.Log ("player reference in start: " + player);

		*/
	}
	
	void Update () {

		player = WorldManager.instance.player;
		//Debug.Log ("player reference in update: " + player);

		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			//Attempt move up
			Debug.Log("up key pressed");
			if (MoveEntity.MoveSuccessful (player, new Vector3 (player.currentPosition.x, player.currentPosition.y + 1, 0))) {
                //Pass turn or consume action points, unknown presently. 
                actionTaken = true;
                Debug.Log("move successful");
			}
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			//Attempt move left
			if (MoveEntity.MoveSuccessful (player, new Vector3 (player.currentPosition.x - 1, player.currentPosition.y, 0))) {
                //Pass turn or consume action points, unknown presently
                actionTaken = true;
                Debug.Log("move successful");
			}
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			//Attempt move down
			if (MoveEntity.MoveSuccessful (player, new Vector3 (player.currentPosition.x, player.currentPosition.y - 1, 0))) {
                //Pass turn or consume action points, unknown presently
                actionTaken = true;
                Debug.Log("move successful");
			}
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			//Attempt move right
			if (MoveEntity.MoveSuccessful (player, new Vector3 (player.currentPosition.x + 1, player.currentPosition.y, 0))) {
                //Pass turn or consume action points, unknown presently
                actionTaken = true;
                Debug.Log("move successful");
			}
		}
	}
}
