using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public bool WorldMapGenComplete;
    public bool PlayerInStartingArea;

    public Cell CurrentCell;
    public Area CurrentArea;
    public Tile CurrentTile;

    public Entity Player;
    public GameObject PlayerSprite;

    public List<string> Messages;
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

        Messages = new List<string>();
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
        CheckMessages();
    }

    private GameState WhoseTurn() {
        if (!CurrentArea.EntitiesPresent()) {
            InputController.Instance.ActionTaken = false;
            return GameState.Playerturn;
        }
        var lastTurn = CurrentArea.TurnOrder.Dequeue();
        CurrentArea.TurnOrder.Enqueue(lastTurn);
        if (CurrentArea.TurnOrder.Peek().IsPlayer()) {
            InputController.Instance.ActionTaken = false;
            return GameState.Playerturn;
        }
        EnemyController.ActionTaken = false;
        return GameState.Enemyturn;
    }

    public void CheckMessages() {
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
        if (Messages.Count > 0) {
            Messages.Clear();
        }
    }
}
