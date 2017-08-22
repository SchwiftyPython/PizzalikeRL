using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public enum TurnState {
		START,
		PLAYERTURN,
		ENEMYTURN,
		END,
	}

	public TurnState currentState;

	// Use this for initialization
	void Awake () {
		currentState = TurnState.START;
	}
	
	// Update is called once per frame
	void Update () {

		switch(currentState) {
		case TurnState.START:
			WorldManager.instance.BoardSetup ();
			currentState = TurnState.PLAYERTURN;
			break;
		case TurnState.PLAYERTURN:
                //while (!InputController.instance.actionTaken) { }
                //currentState = TurnState.ENEMYTURN;
			break;
		case TurnState.ENEMYTURN:
			//make decision
			break;
		case TurnState.END:
			//quit game or go to main menu
			break;
		}
	}
}
