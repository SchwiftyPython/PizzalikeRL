﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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

    public Scene CurrentScene { get; set; }

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
	
    private void Update ()
    {
        CurrentScene = SceneManager.GetActiveScene();

        if (CurrentScene.name.Equals("WorldMap"))
        {
            AreaMap.Instance?.Camera.SetActive(false);
        }
        if (CurrentScene.name.Equals("Area"))
        {
            WorldMap.Instance?.Camera.SetActive(false);
        }

        Debug.Log(CurrentState);
		switch(CurrentState) {
		case GameState.Start:
		        if (WorldMapGenComplete){
                    CurrentState = GameState.EnterArea;
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
		        CheckForDeadEntities();
		        CurrentState = WhoseTurn();
		        break;
		}
//        if (CurrentState == GameState.Playerturn || 
//            CurrentState == GameState.Enemyturn) {
//            CheckMessages();
//            CheckForDeadEntities();
//        }
    }

    private GameState WhoseTurn()
    {
        if (!CurrentArea.EntitiesPresent() || CurrentScene.name.Equals("WorldMap"))
        {
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
        if (CurrentArea.TurnOrder.Peek().IsPlayer())
        {
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
             _messenger.CreateMessage(message);
        }
        Messages.Clear();
    }

    private void CheckForDeadEntities() {
        //var temp = new List<Entity>(CurrentArea.PresentEntities);
        foreach (var entity in CurrentArea.PresentEntities.ToArray())
        {
            if (entity.IsDead())
            {
                AreaMap.Instance.RemoveEntity(entity);
            }
        }

//        foreach (var e in CurrentArea.PresentEntities) {
//            if (!e.IsDead()) {
//                temp.Add(e);
//            }
//            AreaMap.Instance.RemoveEntity(e);
//        }
        //CurrentArea.PresentEntities = temp;
    }
}
