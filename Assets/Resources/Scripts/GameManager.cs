using UnityEngine;

public class GameManager : MonoBehaviour {

    public Cell CurrentCellPosition;
    public Area CurrentAreaPosition;
    public Tile CurrentTilePosition;

	public enum TurnState {
		Start,
        Worldmap,
        Areamap,
		Playerturn,
		Enemyturn,
		End,
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

        CurrentState = TurnState.Start;
	}
	
	// Update is called once per frame
    private void Update () {        
		switch(CurrentState) {
		case TurnState.Start:
			break;
		case TurnState.Playerturn:
                if (InputController.instance.actionTaken) {
                    CurrentState = TurnState.Enemyturn;
                }
			break;
		case TurnState.Enemyturn:               
            break;
		case TurnState.End:
			//go to main menu
			break;
		}
	}    
}
