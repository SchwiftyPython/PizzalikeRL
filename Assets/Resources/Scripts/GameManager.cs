using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum CameraPosition
{
    Left,
    Center,
    Right
}

public class GameManager : MonoBehaviour, ISubscriber
{
    private const float CameraY = 10.5f;
    private const float RightCameraX = 63f;
    private const float CenterCameraX = 36f;
    private const float LeftCameraX = 29f;

    private const int PizzaOrderInterval = 500;

    private CameraPosition _currentCameraPosition;

    private readonly IList<string> _subscribedEvents = new List<string>
    {
        GlobalHelper.ToppingDroppedEventName,
        GlobalHelper.ToppingNotDroppedEventName,
        GlobalHelper.DeliveredEventName,
        GlobalHelper.NewActiveWindowEventName
    };

    private readonly IDictionary<CameraPosition, int[]> _playerPositionRangesForCameraPosition = new Dictionary<CameraPosition, int[]>
    {
        {CameraPosition.Left, new[] {0, 34} },
        {CameraPosition.Center, new[] {34, 44} },
        {CameraPosition.Right, new[] {44, 79} }
    };

    private List<GameObject> _activeWindows;

    public const string WorldMapSceneName = "WorldMap";
    public const string AreaMapSceneName = "Area";

    public int ToppingDropChance;

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

    public int TurnNumber;
    public int NextTurnNumberForOrder;

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

        ActiveOrders = new Dictionary<string, PizzaOrder>();

        _currentCameraPosition = CameraPosition.Right;

        _activeWindows = new List<GameObject>();

        SubscribeToEvents();

        ResetToppingDropChance();

        TurnNumber = 0;
        NextTurnNumberForOrder = PizzaOrderInterval;
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

        //Debug.Log(CurrentState);
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

                    //todo find better spot to order first pizza
                    if (Instance.ActiveOrders.Count < 1)
                    {
                        OrderPizza();
                    }
                }
                break;
            case GameState.Playerturn:
                if (!IsWorldMapSceneActive() && (PlayerInvisible() || PlayerNearCameraEdge()))
                {
                    MoveCameraToPlayer();
                }
                if (InputController.Instance.ActionTaken)
                {
                    EventMediator.Instance.Broadcast(GlobalHelper.EndTurnEventName, this);

                    CurrentState = GameState.EndTurn;
                    TurnNumber++;

                    if (TurnNumber >= NextTurnNumberForOrder)
                    {
                        NextTurnNumberForOrder += PizzaOrderInterval;

                        if (ActiveOrders.Count < 3)
                        {
                            OrderPizza();
                        }
                    }
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
                if (IsWorldMapSceneActive())
                {
                    InputController.Instance.ActionTaken = false;
                    CurrentState = GameState.Playerturn;
                }
                else
                {
                    CheckForDeadEntities();
                    CurrentState = WhoseTurn();
                }
                break;
            case GameState.PlayerDeath:
                if (PlayerDeathRoutineComplete)
                {
                    SceneManager.LoadScene(AreaMapSceneName);
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

        if (Player.GetSprite() != null)
        {
            Destroy(Player.GetSprite());
        }

        Player = player;
        WorldData.Instance.Entities.Add(Player.Id, Player);

        CurrentCell = WorldData.Instance.PlayerStartingPlace;
        CurrentArea = CurrentCell.Areas[1, 1];

        Player.CurrentCell = CurrentCell;
        Player.CurrentArea = CurrentArea;

        PlayerInStartingArea = true;

        PlayerDeathRoutineComplete = true;
    }

    public bool IsWorldMapSceneActive()
    {
        return CurrentScene.name.Equals(WorldMapSceneName);
    }

    public void AddActiveWindow(GameObject window)
    {
        if (_activeWindows.Contains(window))
        {
            return;
        }
        _activeWindows.Add(window);
    }

    public void RemoveActiveWindow(GameObject window)
    {
        if (!_activeWindows.Contains(window))
        {
            return;
        }
        _activeWindows.Remove(window);
    }

    public List<GameObject> GetActiveWindows()
    {
        return _activeWindows;
    }

    public bool AnyActiveWindows()
    {
        return _activeWindows.Any();
    }

    private static void RunPlayerDeathRoutine()
    {
        HistoryGenerator.Instance.Generate();
        
        SceneManager.LoadScene("PlayerDeath"); 
    }

    private GameState WhoseTurn()
    {
        if (IsWorldMapSceneActive() || !CurrentArea.EntitiesPresent())
        {
            InputController.Instance.ActionTaken = false;
            return GameState.Playerturn;
        }
        var lastTurn = CurrentArea.TurnOrder.Dequeue();
        CurrentArea.TurnOrder.Enqueue(lastTurn);
        
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
        if (Camera.main == null || Player == null || Player.GetSprite() == null)
        {
            return true;
        }

        var viewPosition = Camera.main.WorldToViewportPoint(Player.GetSprite().GetComponent<Renderer>().bounds.center);

        return viewPosition.x <= 0 || viewPosition.x >= 1 || viewPosition.y <= 0 || viewPosition.y >= 1;
    }

    private bool PlayerNearCameraEdge()
    {
        switch (_currentCameraPosition)
        {
            case CameraPosition.Left:
                if (Player.CurrentPosition.x > _playerPositionRangesForCameraPosition[CameraPosition.Left][1])
                {
                    return true;
                }
                break;
            case CameraPosition.Center:
                if (Player.CurrentPosition.x < _playerPositionRangesForCameraPosition[CameraPosition.Center][0] ||
                    Player.CurrentPosition.x > _playerPositionRangesForCameraPosition[CameraPosition.Center][1])
                {
                    return true;
                }
                break;
            case CameraPosition.Right:
                if (Player.CurrentPosition.x < _playerPositionRangesForCameraPosition[CameraPosition.Right][0])
                {
                    return true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return false;
    }

    private void MoveCameraToPlayer()
    {
        switch (_currentCameraPosition)
        {
            case CameraPosition.Left:
                if (Player.CurrentPosition.x >= _playerPositionRangesForCameraPosition[CameraPosition.Left][1])
                {
                    ChangeCameraPosition(CameraPosition.Center);
                }
                break;
            case CameraPosition.Center:
                if (Player.CurrentPosition.x <= _playerPositionRangesForCameraPosition[CameraPosition.Center][0])
                {
                    ChangeCameraPosition(CameraPosition.Left);
                }
                if(Player.CurrentPosition.x >= _playerPositionRangesForCameraPosition[CameraPosition.Center][1])
                {
                    ChangeCameraPosition(CameraPosition.Right);
                }
                break;
            case CameraPosition.Right:
                if (Player.CurrentPosition.x <= _playerPositionRangesForCameraPosition[CameraPosition.Right][0])
                {
                    ChangeCameraPosition(CameraPosition.Center);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ChangeCameraPosition(CameraPosition newPosition)
    {
        switch (newPosition)
        {
            case CameraPosition.Left:
                Camera.main.transform.localPosition = new Vector3(LeftCameraX, CameraY, -10);
                _currentCameraPosition = CameraPosition.Left;
                break;
            case CameraPosition.Center:
                Camera.main.transform.localPosition = new Vector3(CenterCameraX, CameraY, -10);
                _currentCameraPosition = CameraPosition.Center;
                break;
            case CameraPosition.Right:
                Camera.main.transform.localPosition = new Vector3(RightCameraX, CameraY, -10);
                _currentCameraPosition = CameraPosition.Right;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newPosition), newPosition, null);
        }
    }

    private void ResetToppingDropChance()
    {
        ToppingDropChance = 32;
    }

    private void IncreaseChanceOfToppingDrop()
    {
        ToppingDropChance += 3;
    }
    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals("ToppingDropped"))
        {
            ResetToppingDropChance();
        }
        else if (eventName.Equals("ToppingNotDropped"))
        {
            IncreaseChanceOfToppingDrop();
        }
        else if (eventName.Equals("Delivered") && parameter != null)
        {
            var presentEntity = (Entity)parameter;

            ActiveOrders.Remove(presentEntity.Fluff.Name);
            
            RemoveMarkerFromCustomer(presentEntity.GetSprite());

            if (ActiveOrders.Count < 1)
            {
                OrderPizza();
            }
        }
    }
    private void SubscribeToEvents()
    {
        foreach (var eventName in _subscribedEvents)
        {
            EventMediator.Instance.SubscribeToEvent(eventName, this);
        }
    }

    private void UnsubscribeFromEvents()
    {
        EventMediator.Instance.UnsubscribeFromAllEvents(this);
    }

    private static void RemoveMarkerFromCustomer(GameObject customerSprite)
    {
        GlobalHelper.DestroyAllChildren(customerSprite);
    }

    private void OrderPizza()
    {
        var order = new PizzaOrder((PizzaOrder.OrderDifficulty)Random.Range(0,
            Enum.GetNames(typeof(PizzaOrder.OrderDifficulty)).Length));

        Instance.ActiveOrders.Add(order.Customer.Fluff.Name, order);
    }
}
