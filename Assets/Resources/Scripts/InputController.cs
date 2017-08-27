using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    private static InputController instance = null;

    Entity player;
    private bool actionTaken = false; //for basic AI pathfinding testing

    public static InputController Instance {
        get {
            return instance;
        }

        set {
            instance = value;
        }
    }

    public Entity Player {
        get {
            return player;
        }

        set {
            player = value;
        }
    }

    public bool ActionTaken {
        get {
            return actionTaken;
        }

        set {
            actionTaken = value;
        }
    }

    void Start () {
		if (Instance == null) {
			Instance = this;
		}else if(Instance != this){
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

        Player = WorldManager.instance.player;
        //Debug.Log ("player reference in update: " + player);

        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            //Attempt move up
            Debug.Log("up key pressed");
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x, Player.currentPosition.y + 1))) {
                //Pass turn or consume action points, unknown presently. 
                ActionTaken = true;
                Debug.Log("move successful");
            }
        } else if (Input.GetKeyDown(KeyCode.Keypad7)) {
            //Attempt move diagonal up and left
            Debug.Log("up key pressed");
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x - 1, Player.currentPosition.y + 1))) {
                //Pass turn or consume action points, unknown presently. 
                ActionTaken = true;
                Debug.Log("move successful");
            }
        } else if (Input.GetKeyDown(KeyCode.Keypad4)) {
            //Attempt move left
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x - 1, Player.currentPosition.y))) {
                //Pass turn or consume action points, unknown presently
                ActionTaken = true;
                Debug.Log("move successful");
            }
        } else if (Input.GetKeyDown(KeyCode.Keypad1)) {
            //Attempt move diagonal down and left
            Debug.Log("up key pressed");
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x - 1, Player.currentPosition.y - 1))) {
                //Pass turn or consume action points, unknown presently. 
                ActionTaken = true;
                Debug.Log("move successful");
            }
        } else if (Input.GetKeyDown(KeyCode.Keypad2)) {
            //Attempt move down
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x, Player.currentPosition.y - 1))) {
                //Pass turn or consume action points, unknown presently
                ActionTaken = true;
                Debug.Log("move successful");
            }
        } else if (Input.GetKeyDown(KeyCode.Keypad3)) {
            //Attempt move diagonal down and right
            Debug.Log("up key pressed");
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x + 1, Player.currentPosition.y - 1))) {
                //Pass turn or consume action points, unknown presently. 
                ActionTaken = true;
                Debug.Log("move successful");
            }
        } else if (Input.GetKeyDown(KeyCode.Keypad6)) {
            //Attempt move right
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x + 1, Player.currentPosition.y))) {
                //Pass turn or consume action points, unknown presently
                ActionTaken = true;
                Debug.Log("move successful");
            }
        } else if (Input.GetKeyDown(KeyCode.Keypad9)) {
            //Attempt move diagonal up and right
            Debug.Log("up key pressed");
            if (MoveEntity.MoveSuccessful(Player, new Vector2(Player.currentPosition.x + 1, Player.currentPosition.y + 1))) {
                //Pass turn or consume action points, unknown presently. 
                ActionTaken = true;
                Debug.Log("move successful");
            }
        }
    }
}
