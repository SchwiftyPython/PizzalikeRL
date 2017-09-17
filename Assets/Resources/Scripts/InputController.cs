using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    public static InputController instance = null;

    Entity player;
    public bool actionTaken = false; //for basic AI pathfinding testing
    //Vector2 target;

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
                Vector2 target = new Vector2(player.currentPosition.x, player.currentPosition.y + 1);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());                        
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                }else{                    
                    player.Move(target);
                    actionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad7)) {
                //Attempt move diagonal up and left
                Debug.Log("up key pressed");
                Vector2 target = new Vector2(player.currentPosition.x - 1, player.currentPosition.y + 1);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                } else {
                    player.Move(target);
                    actionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad4)) {
                //Attempt move left
                Vector2 target = new Vector2(player.currentPosition.x - 1, player.currentPosition.y);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                } else {
                    player.Move(target);
                    actionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad1)) {
                //Attempt move diagonal down and left                
                Vector2 target = new Vector2(player.currentPosition.x - 1, player.currentPosition.y - 1);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                } else {
                    player.Move(target);
                    actionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad2)) {
                //Attempt move down
                Vector2 target = new Vector2(player.currentPosition.x, player.currentPosition.y - 1);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                } else {
                    player.Move(target);
                    actionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad3)) {
                //Attempt move diagonal down and right
                Vector2 target = new Vector2(player.currentPosition.x + 1, player.currentPosition.y - 1);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                } else {
                    player.Move(target);
                    actionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad6)) {
                //Attempt move right
                Vector2 target = new Vector2(player.currentPosition.x + 1, player.currentPosition.y);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                } else {
                    player.Move(target);
                    actionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad9)) {
                //Attempt move diagonal up and right
                Vector2 target = new Vector2(player.currentPosition.x + 1, player.currentPosition.y + 1);
                if (!CanMove(target)) {
                    if (EntityPresent(target)) {
                        player.MeleeAttack(WorldManager.instance.GetTileAt(target).GetPresentEntity());
                        //Pass turn or consume action points, unknown presently. 
                        actionTaken = true;
                    }
                } else {
                    player.Move(target);
                    actionTaken = true;
                }
            }
        }
    }

    bool CanMove(Vector2 target) {
        if (player.TargetTileBlocked(target)) {
            return false;
        } else {
            return true;
        }
    }

    bool EntityPresent(Vector2 target) {
        if (player.TargetTileBlockedByEntity(target)) {
            return true;
        } else {
            return false;
        }
    }
}
