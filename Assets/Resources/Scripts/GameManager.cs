using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public const string WorldMapSceneName = "WorldMap";
    public const string AreaMapSceneName = "Area";

    public bool WorldMapGenComplete;
    public bool PlayerInStartingArea;
    public bool PlayerEnteringAreaFromWorldMap;
    public bool BusyGeneratingHistory;
    public bool PlayerDeathRoutineComplete;
    public bool PlayerDead;

    public Cell CurrentCell;
    public Area CurrentArea;
    public Tile CurrentTile;

    public Entity Player;
    public GameObject PlayerSpritePrefab;

    public EnemyController CurrentEntityController;

    public Dictionary<string, PizzaOrder> ActiveOrders;

    public List<string> Messages;
    private Messenger _messenger;

    public enum GameState
    {
        Start,
        Worldmap,
        HistoryGeneration,
        EnterArea,
        Playerturn,
        Enemyturn,
        EndTurn,
        PlayerDeath
    }

    public GameState CurrentState { get; set; }

    public Scene CurrentScene { get; set; }
    
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        CurrentState = GameState.Start;

        PlayerInStartingArea = true;

        Messages = new List<string>();

        ActiveOrders = new Dictionary<string, PizzaOrder>();
    }

    private void Update()
    {
        CurrentScene = SceneManager.GetActiveScene();

        if (PlayerDead)
        {
            PlayerDead = false;
            PlayerDeathRoutineComplete = false;
            CurrentState = GameState.PlayerDeath;
            RunPlayerDeathRoutine();
        }

        Debug.Log(CurrentState);
        switch (CurrentState)
        {
            case GameState.Start:
                if (WorldMapGenComplete)
                {
                    CurrentState = GameState.EnterArea;
                }
                break;
            case GameState.HistoryGeneration:
                if (!BusyGeneratingHistory)
                {
                    BusyGeneratingHistory = true;
                }
                break;
            case GameState.EnterArea:
                if (AreaMap.Instance == null)
                {
                    break;
                }

                AreaMap.Instance.EnterArea();
                if (AreaMap.Instance.AreaReady)
                {
                    CurrentState = GameState.Playerturn;

                    if (Instance.ActiveOrders.Count < 1)
                    {
                        //todo create intelligent difficulty system - easy, medium, then whatever
                        var order = new PizzaOrder((PizzaOrder.OrderDifficulty) Random.Range(0,
                            Enum.GetNames(typeof(PizzaOrder.OrderDifficulty)).Length));

                        Instance.ActiveOrders.Add(order.Customer.Fluff.Name, order);
                    }
                }
                break;
            case GameState.Playerturn:
                if (PlayerInvisible())
                {
                    Debug.Log("Player not in camera!");
                    MoveCameraToPlayer();
                }
                if (InputController.Instance.ActionTaken)
                {
                    CurrentState = GameState.EndTurn;
                }
                break;
            case GameState.Enemyturn:
                if (!CurrentEntityController.TurnStarted)
                {
                    CurrentEntityController.TakeAction();
                }
                if (CurrentEntityController.ActionTaken)
                {
                    CurrentState = GameState.EndTurn;
                }
                break;
            case GameState.EndTurn:
                if (CurrentScene.name.Equals(WorldMapSceneName))
                {
                    InputController.Instance.ActionTaken = false;
                    CurrentState = GameState.Playerturn;
                }
                else
                {
                    CheckMessages();
                    CheckForDeadEntities();
                    CurrentState = WhoseTurn();
                }
                break;
            case GameState.PlayerDeath:
                if (PlayerDeathRoutineComplete)
                {
                    CurrentState = GameState.EnterArea;
                }
                break;
        }
    }

    public void ContinueGame(Entity player)
    {
        if (WorldData.Instance.Entities.ContainsKey(Player.Id))
        {
            WorldData.Instance.Entities.Remove(Player.Id);
        }

        Player = player;
        WorldData.Instance.Entities.Add(Player.Id, Player);

        CurrentCell = WorldData.Instance.PlayerStartingPlace;
        CurrentArea = CurrentCell.Areas[1, 1];

        Player.CurrentCell = CurrentCell;
        Player.CurrentArea = CurrentArea;

        PlayerDeathRoutineComplete = true;
    }

    private void RunPlayerDeathRoutine()
    {
        HistoryGenerator.Instance.Generate();
        
        SceneManager.LoadScene("PlayerDeath"); 
    }

    private GameState WhoseTurn()
    {
        if (CurrentScene.name.Equals(WorldMapSceneName) || !CurrentArea.EntitiesPresent())
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
        CurrentEntityController = CurrentArea.TurnOrder.Peek().GetSprite().GetComponent<EnemyController>();
        CurrentEntityController.ActionTaken = false;
        CurrentEntityController.TurnStarted = false;
        return GameState.Enemyturn;
    }

    private void CheckMessages()
    {
        if (_messenger == null)
        {
            _messenger = Messenger.GetInstance();
        }
        if (Messages.Count <= 0)
        {
            return;
        }
        foreach (var message in Messages)
        {
            _messenger.CreateMessage(message);
        }
        Messages.Clear();
    }

    private void CheckForDeadEntities()
    {
        foreach (var entity in CurrentArea.PresentEntities.ToArray())
        {
            if (!entity.IsDead())
            {
                continue;
            }

            AreaMap.Instance.RemoveDeadEntity(entity);

            if (entity.IsPlayer())
            {
                PlayerDead = true;
            }
        }
    }

    private bool PlayerInvisible()
    {
        var viewPosition = Camera.main.WorldToViewportPoint(Player.GetSprite().GetComponent<Renderer>().bounds.center);

        return viewPosition.x <= 0 || viewPosition.x >= 1 || viewPosition.y <= 0 || viewPosition.y >= 1;
    }

    private void MoveCameraToPlayer()
    {
        //todo make this smoother by setting min max settings for x and y for camera
        Camera.main.transform.localPosition = new Vector3(Player.CurrentPosition.x + .5f, Player.CurrentPosition.y + .5f, -10);
    }
}
