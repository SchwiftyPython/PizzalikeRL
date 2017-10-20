using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public enum TurnState {
		START,
        WORLDMAP,
		PLAYERTURN,
		ENEMYTURN,
		END,
	}

    public TurnState currentState { get; set; }

    public static GameManager instance = null;

    // Use this for initialization
    void Awake () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            // Destroy the current object, so there is just one 
            Destroy(gameObject);
        }

        currentState = TurnState.START;
	}
	
	// Update is called once per frame
	void Update () {        
		switch(currentState) {
		case TurnState.START:
                if (!WorldManager.instance.worldSetup) {
                    WorldManager.instance.BoardSetup();
                }
                if (WorldManager.instance.worldSetup) {
                    currentState = TurnState.PLAYERTURN;
                }
			break;
		case TurnState.PLAYERTURN:
                if (InputController.instance.actionTaken) {
                    currentState = TurnState.ENEMYTURN;
                }
			break;
		case TurnState.ENEMYTURN:               
            break;
		case TurnState.END:
			//go to main menu
			break;
		}
	}    
}
