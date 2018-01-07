using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public bool WorldMapGenComplete;
    public bool PlayerInStartingArea;

    public Cell CurrentCellPosition;
    public Area CurrentAreaPosition;
    public Tile CurrentTilePosition;

    public Entity Player;
    public GameObject PlayerSprite;

    public Queue<string> Messages;
    private Messenger _messenger;

	public enum GameState {
		Start,
        Worldmap,
        EnterArea,
		Playerturn,
		Enemyturn,
		End
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

        Messages = new Queue<string>();
    }
	
    private void Update () {
        Debug.Log(CurrentState);
		switch(CurrentState) {
		case GameState.Start:
		        if (WorldMapGenComplete){
                    CurrentState = GameState.EnterArea;
                }
                break;
            case GameState.EnterArea:
                AreaMap.Instance.EnterArea();
                if (AreaMap.Instance.AreaReady)
                {
                    CurrentState = GameState.Playerturn;
                }
                break;
		case GameState.Playerturn:
		    if (InputController.Instance.ActionTaken) {
		        CurrentState = WhoseTurn();
		    }
		    break;
		case GameState.Enemyturn:
		    if (EnemyController.ActionTaken){
		        CurrentState = WhoseTurn();
		    }
            break;
		case GameState.End:
			//go to main menu
			break;
		}
	}

    private GameState WhoseTurn() {
        if (!CurrentAreaPosition.EntitiesPresent()) {
            InputController.Instance.ActionTaken = false;
            return GameState.Playerturn;
        }
        var lastTurn = CurrentAreaPosition.TurnOrder.Dequeue();
        CurrentAreaPosition.TurnOrder.Enqueue(lastTurn);
        if (CurrentAreaPosition.TurnOrder.Peek().IsPlayer()) {
            InputController.Instance.ActionTaken = false;
            return GameState.Playerturn;
        }
        EnemyController.ActionTaken = false;
        return GameState.Enemyturn;
    }

    private void CheckMessages() {
        if (_messenger == null) {
            _messenger = Messenger.GetInstance();
        }
        if (Messages.Count <= 0)
        {
            return;
        }
        foreach (var message in Messages) {
            _messenger.CreateMessage(message, Color.black);
        }
    }
};
