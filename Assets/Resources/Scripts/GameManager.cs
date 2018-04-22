using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public bool WorldMapGenComplete;
    public bool PlayerInStartingArea;
    public bool BusyGeneratingHistory;

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
        HistoryGeneration,
        EnterArea,
		Playerturn,
		Enemyturn,
		EndTurn
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
                    //CurrentState = GameState.EnterArea;
                }
                break;
            case GameState.HistoryGeneration:
                if (!BusyGeneratingHistory) {
                    BusyGeneratingHistory = true;
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
                //CurrentState = WhoseTurn();
		        CurrentState = GameState.EndTurn;
		    }
		    break;
		case GameState.Enemyturn:
		    if (EnemyController.ActionTaken){
                    //CurrentState = WhoseTurn();
		        CurrentState = GameState.EndTurn;
                }
            break;
		case GameState.EndTurn:
		    CheckMessages();
		    //Todo: This is causing a problem. Create a branch for it.
                //CheckForDeadEntities();
                CurrentState = WhoseTurn();
                break;
		}
//        if (CurrentState == GameState.Playerturn || 
//            CurrentState == GameState.Enemyturn) {
//            CheckMessages();
//            CheckForDeadEntities();
//        }
    }

    private GameState WhoseTurn() {
        if (!CurrentArea.EntitiesPresent()) {
            InputController.Instance.ActionTaken = false;
            return GameState.Playerturn;
        }
        var lastTurn = CurrentArea.TurnOrder.Dequeue();
        CurrentArea.TurnOrder.Enqueue(lastTurn);

        //Remove any entities that were removed from play
        while (!CurrentArea.PresentEntities.Contains(CurrentArea.TurnOrder.Peek()))
        {
            CurrentArea.TurnOrder.Dequeue();
        }
        if (CurrentArea.TurnOrder.Peek().IsPlayer()) {
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
        Messages.Clear();
        
    }

    private void CheckForDeadEntities() {
        var temp = new List<Entity>();
        for (var i = 0; i < CurrentArea.PresentEntities.Count; i++)
        {
            if (!CurrentArea.PresentEntities[i].IsDead())
            {
                temp.Add(CurrentArea.PresentEntities[i]);
            }
            AreaMap.Instance.RemoveEntity(CurrentArea.PresentEntities[i]);
        }

//        foreach (var e in CurrentArea.PresentEntities) {
//            if (!e.IsDead()) {
//                temp.Add(e);
//            }
//            AreaMap.Instance.RemoveEntity(e);
//        }
        CurrentArea.PresentEntities = temp;
    }
}
