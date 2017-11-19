using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    public static InputController instance = null;

    Entity _player;
    public bool ActionTaken = false; //for basic AI pathfinding testing
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
        if (GameManager.Instance.CurrentState == GameManager.GameState.Playerturn) {
            _player = GameManager.Instance.Player;
            //Debug.Log ("player reference in update: " + _player);

            if (Input.GetKeyDown(KeyCode.Keypad8)) {
                //Attempt move up                
                var target = new Vector2(_player.CurrentPosition.x, _player.CurrentPosition.y + 1);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad7)) {
                //Attempt move diagonal up and left                
                var target = new Vector2(_player.CurrentPosition.x - 1, _player.CurrentPosition.y + 1);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad4)) {
                //Attempt move left
                var target = new Vector2(_player.CurrentPosition.x - 1, _player.CurrentPosition.y);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad1)) {
                //Attempt move diagonal down and left                
                var target = new Vector2(_player.CurrentPosition.x - 1, _player.CurrentPosition.y - 1);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad2)) {
                //Attempt move down
                var target = new Vector2(_player.CurrentPosition.x, _player.CurrentPosition.y - 1);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad3)) {
                //Attempt move diagonal down and right
                var target = new Vector2(_player.CurrentPosition.x + 1, _player.CurrentPosition.y - 1);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad6)) {
                //Attempt move right
                var target = new Vector2(_player.CurrentPosition.x + 1, _player.CurrentPosition.y);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Keypad9)) {
                //Attempt move diagonal up and right
                var target = new Vector2(_player.CurrentPosition.x + 1, _player.CurrentPosition.y + 1);
                if (PlayerMoveOrAttack(target)) {
                    ActionTaken = true;
                }
            }
        }
    }

    private bool PlayerMoveOrAttack(Vector2 target) {
        if (CanMove(target)) {
            _player.Move(target);
            return true;
        }
        if(EntityPresent(target)) {
            _player.MeleeAttack(WorldManager.Instance.GetTileAt(target).GetPresentEntity());                 
            return true;
        }
        return false;
    }

    private bool CanMove(Vector2 target) {
        return !_player.TargetTileBlocked(target);
    }

    private bool EntityPresent(Vector2 target) {
        return _player.TargetTileBlockedByEntity(target);
    }
}

    
