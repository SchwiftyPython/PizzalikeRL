using UnityEngine;

public class GameManager : MonoBehaviour {
    public bool WorldMapGenComplete;
    public bool PlayerInStartingArea;

    public Cell CurrentCellPosition;
    public Area CurrentAreaPosition;
    public Tile CurrentTilePosition;

    public Entity Player;
    public GameObject PlayerSprite;

	public enum GameState {
		Start,
        Worldmap,
        Areamap,
		Playerturn,
		Enemyturn,
		End,
	}

    public GameState CurrentState { get; set; }

    public static GameManager Instance;
    
    private void Awake () {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        CurrentState = GameState.Start;
        PlayerInStartingArea = true;
    }
	
    private void Update () {
        Debug.Log(CurrentState);
		switch(CurrentState) {
		case GameState.Start:
		        if (WorldMapGenComplete){
                    CurrentState = GameState.Playerturn;
                }
                break;
		case GameState.Playerturn:
                /*
		    if (InputController.instance.ActionTaken) {
		        CurrentState = GameState.Enemyturn;
		    }
            */
		    break;
		case GameState.Enemyturn:               
            break;
		case GameState.End:
			//go to main menu
			break;
		}
	}    
}
