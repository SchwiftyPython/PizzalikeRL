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

    void Update() {
        if (GameManager.instance.currentState == GameManager.TurnState.PLAYERTURN) {
            player = WorldManager.instance.player;
            //Debug.Log ("player reference in update: " + player);

            if (Input.GetKeyDown(KeyCode.Keypad8)) {
                //Attempt move up
                Debug.Log("up key pressed");
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x, player.currentPosition.y + 1))) {
                    //Pass turn or consume action points, unknown presently. 
                    actionTaken = true;

                }
            } else if (Input.GetKeyDown(KeyCode.Keypad7)) {
                //Attempt move diagonal up and left
                Debug.Log("up key pressed");
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x - 1, player.currentPosition.y + 1))) {
                    //Pass turn or consume action points, unknown presently. 
                    actionTaken = true;

                }
            } else if (Input.GetKeyDown(KeyCode.Keypad4)) {
                //Attempt move left
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x - 1, player.currentPosition.y))) {
                    //Pass turn or consume action points, unknown presently
                    actionTaken = true;

                }
            } else if (Input.GetKeyDown(KeyCode.Keypad1)) {
                //Attempt move diagonal down and left
                Debug.Log("up key pressed");
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x - 1, player.currentPosition.y - 1))) {
                    //Pass turn or consume action points, unknown presently. 
                    actionTaken = true;

                }
            } else if (Input.GetKeyDown(KeyCode.Keypad2)) {
                //Attempt move down
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x, player.currentPosition.y - 1))) {
                    //Pass turn or consume action points, unknown presently
                    actionTaken = true;

                }
            } else if (Input.GetKeyDown(KeyCode.Keypad3)) {
                //Attempt move diagonal down and right
                Debug.Log("up key pressed");
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x + 1, player.currentPosition.y - 1))) {
                    //Pass turn or consume action points, unknown presently. 
                    actionTaken = true;

                }
            } else if (Input.GetKeyDown(KeyCode.Keypad6)) {
                //Attempt move right
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x + 1, player.currentPosition.y))) {
                    //Pass turn or consume action points, unknown presently
                    actionTaken = true;

                }
            } else if (Input.GetKeyDown(KeyCode.Keypad9)) {
                //Attempt move diagonal up and right
                Debug.Log("up key pressed");
                if (player.MoveSuccessful(player, new Vector2(player.currentPosition.x + 1, player.currentPosition.y + 1))) {
                    //Pass turn or consume action points, unknown presently. 
                    actionTaken = true;

                }
            }
        }
    }
}
