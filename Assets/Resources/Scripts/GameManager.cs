using UnityEngine;

public class GameManager : MonoBehaviour {

	public enum TurnState {
		START,
        WORLDMAP,
		PLAYERTURN,
		ENEMYTURN,
		END,
	}

    public TurnState CurrentState { get; set; }

    public static GameManager Instance;

    // Use this for initialization
    void Awake () {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            // Destroy the current object, so there is just one 
            Destroy(gameObject);
        }

        CurrentState = TurnState.START;
	}
	
	// Update is called once per frame
	void Update () {        
		switch(CurrentState) {
		case TurnState.START:
                if (!WorldManager.Instance.WorldSetup) {
                    WorldManager.Instance.BoardSetup();
                }
                if (WorldManager.Instance.WorldSetup) {
                    CurrentState = TurnState.PLAYERTURN;
                }
			break;
		case TurnState.PLAYERTURN:
                if (InputController.instance.actionTaken) {
                    CurrentState = TurnState.ENEMYTURN;
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
