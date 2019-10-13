using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputController : MonoBehaviour, ISubscriber
{
    private enum InputMode
    {
        Adventure,
        TargetPicker
    }

    private InputMode _currentInputMode;

    private Dictionary<KeyCode, GameObject> _abilityMap;

    private string _areaMapSceneName;
    private string _worldMapSceneName;

    private bool _popupWindowOpen;

    public static InputController Instance;

    private Entity _player;
    private Seeker _seeker;

    public Path Path;
    public bool PathCalculated;

    public Color HighlightedColor;
    private List<Tile> _highlightedTiles;

    public bool ActionTaken;

    public GameObject Canvas;
    private GraphicRaycaster _canvasGraphicRaycaster;
    private EventSystem _canvasEventSystem;

    private bool _awaitingInputElsewhere;

    private TargetPicker _picker;
    private Entity _selectedTarget;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _areaMapSceneName = GameManager.AreaMapSceneName;
        _worldMapSceneName = GameManager.WorldMapSceneName;

        if (Canvas == null)
        {
            Canvas = GameObject.Find("UI");
        }
        _canvasGraphicRaycaster = Canvas.GetComponent<GraphicRaycaster>();
        _canvasEventSystem = Canvas.GetComponent<EventSystem>();

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.LoadAbilityBarEventName, this);
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AwaitingInputElsewhereEventName, this);
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.SingleTileAbilityEventName, this);
    }

    private void Update()
    {
        if (_awaitingInputElsewhere)
        {
            return;
        }

        var currentScene = GameManager.Instance.CurrentScene.name;

        if (GameManager.Instance.CurrentState == GameManager.GameState.Playerturn)
        {
            if (_currentInputMode == InputMode.Adventure)
            {
                if (_player == null || _player != GameManager.Instance.Player)
                {
                    _player = GameManager.Instance.Player;
                    //Debug.Log ("player reference in update: " + _player);
                }

                if (Input.GetKeyDown(KeyCode.Keypad8))
                {
                    //Attempt move up
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X + 1, _player.CurrentTile.Y)
                        : new Vector2(_player.CurrentCell.X + 1, _player.CurrentCell.Y);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad7))
                {
                    //Attempt move diagonal up and left
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X + 1, _player.CurrentTile.Y - 1)
                        : new Vector2(_player.CurrentCell.X + 1, _player.CurrentCell.Y - 1);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    //Attempt move left
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X, _player.CurrentTile.Y - 1)
                        : new Vector2(_player.CurrentCell.X, _player.CurrentCell.Y - 1);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad1))
                {
                    //Attempt move diagonal down and left                
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X - 1, _player.CurrentTile.Y - 1)
                        : new Vector2(_player.CurrentCell.X - 1, _player.CurrentCell.Y - 1);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad2))
                {
                    //Attempt move down
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X - 1, _player.CurrentTile.Y)
                        : new Vector2(_player.CurrentCell.X - 1, _player.CurrentCell.Y);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad3))
                {
                    //Attempt move diagonal down and right
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X - 1, _player.CurrentTile.Y + 1)
                        : new Vector2(_player.CurrentCell.X - 1, _player.CurrentCell.Y + 1);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    //Attempt move right
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X, _player.CurrentTile.Y + 1)
                        : new Vector2(_player.CurrentCell.X, _player.CurrentCell.Y + 1);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad9))
                {
                    //Attempt move diagonal up and right
                    Vector2 target;
                    target = currentScene == _areaMapSceneName
                        ? new Vector2(_player.CurrentTile.X + 1, _player.CurrentTile.Y + 1)
                        : new Vector2(_player.CurrentCell.X + 1, _player.CurrentCell.Y + 1);

                    if (_player.MoveOrAttackSuccessful(target))
                    {
                        ActionTaken = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Keypad5))
                {
                    ActionTaken = true;
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    EventMediator.Instance.Broadcast("GameMenuPopup", this);
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    EventMediator.Instance.Broadcast("PizzaJournal", this);
                }
                //Open dropped item popup for player's current tile
                else if (Input.GetKeyDown(KeyCode.G))
                {
                    if (currentScene.Equals(_areaMapSceneName) && !GameManager.Instance.AnyActiveWindows())
                    {
                        EventMediator.Instance.Broadcast("DroppedItemPopup", this);
                    }
                }
                //Interact 
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    EventMediator.Instance.Broadcast("Interact", this);
                }
                //Starts Look interaction
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    //todo open journal
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (currentScene.Equals(_areaMapSceneName) && !GameManager.Instance.AnyActiveWindows() &&
                        !IsUiClicked())
                    {
                        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        if (pos.x < 0 || pos.y < 0 || pos.x > GameManager.Instance.CurrentArea.Width ||
                            pos.y > GameManager.Instance.CurrentArea.Height)
                        {
                            return;
                        }

                        var selectedTile = GameManager.Instance.CurrentArea.AreaTiles[(int) pos.y, (int) pos.x];

                        //highlight tile and path to it
                        StartCoroutine(HighlightPathToTarget(_player, selectedTile.GetGridPosition()));

                        EventMediator.Instance.Broadcast("ActionPopup", this, selectedTile);
                    }

                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (currentScene.Equals(_areaMapSceneName))
                    {
                        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                            Vector2.positiveInfinity);

                        if (hit && !GameManager.Instance.AnyActiveWindows())
                        {
                            var position = hit.collider.transform.localPosition;

                            var entity = GameManager.Instance.CurrentArea.AreaTiles[(int) position.y, (int) position.x]
                                .GetPresentEntity();

                            EventMediator.Instance.Broadcast(GlobalHelper.InspectEntityEventName, this, entity);
                        }
                    }

                    if (currentScene.Equals(_worldMapSceneName))
                    {
                        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                            Vector2.positiveInfinity);

                        if (hit && !_popupWindowOpen)
                        {
                            hit.collider.GetComponent<WorldTileInfo>()?.OnRightClick();
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    if (currentScene.Equals(_areaMapSceneName))
                    {
                        SceneManager.LoadScene(_worldMapSceneName);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    if (currentScene.Equals(_worldMapSceneName))
                    {
                        GameManager.Instance.PlayerEnteringAreaFromWorldMap = true;
                        GameManager.Instance.CurrentArea = GameManager.Instance.CurrentCell.Areas[1, 1];
                        GameManager.Instance.Player.CurrentArea = GameManager.Instance.CurrentArea;
                        GameManager.Instance.CurrentState = GameManager.GameState.EnterArea;
                        SceneManager.LoadScene(_areaMapSceneName);
                    }
                }
            }

            if (_currentInputMode == InputMode.TargetPicker)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    ClearHighlight(_selectedTarget.CurrentTile);

                    _selectedTarget = _picker.GetNextTarget();

                    HighlightTile(_selectedTarget.CurrentTile);
                }

                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ClearHighlight(_selectedTarget.CurrentTile);

                    _currentInputMode = InputMode.Adventure;
                }

                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    ClearHighlight(_selectedTarget.CurrentTile);

                    Debug.Log("Target Selected");

                    _currentInputMode = InputMode.Adventure;

                    EventMediator.Instance.Broadcast(GlobalHelper.AbilityTileSelectedEventName, this, _selectedTarget);

                }
            }
        }
    }

    public IEnumerator HighlightPathToTarget(Entity currentEntity, Vector2 target)
    {
        ClearHighlights();
        PathCalculated = false;
        if (_seeker == null)
        {
            _seeker = GameManager.Instance.Player.GetSprite().GetComponent<Seeker>();
        }
        _seeker.StartPath(currentEntity.CurrentPosition, target, OnPathComplete);
        yield return new WaitForSeconds(0.1f);
        Path.vectorPath.Add(target);
        HighlightPath();
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("Path returned. Error? " + p.error);
        if (!p.error)
        {
            Path = p;
            PathCalculated = true;
            //Debug.Log("Vector Path: " + Path.vectorPath[1]);
        }
    }

    public void HighlightPath()
    {
        var currentArea = GameManager.Instance.CurrentArea;
        _highlightedTiles = new List<Tile>();
        foreach (var tilePosition in Path.vectorPath)
        {
            var tile = currentArea.AreaTiles[(int) tilePosition.y, (int) tilePosition.x];
            HighlightTile(tile);
            _highlightedTiles.Add(tile);
        }
    }

    public void HighlightTile(Tile tile)
    {
        tile.TextureInstance.GetComponent<SpriteRenderer>().color = HighlightedColor;
    }

    public void ClearHighlights()
    {
        if (_highlightedTiles == null || _highlightedTiles.Count <= 0)
        {
            return;
        }

        foreach (var tile in _highlightedTiles)
        {
            if (tile.TextureInstance == null)
            {
                continue;
            }

            ClearHighlight(tile);
        }
    }

    public void ClearHighlight(Tile tile)
    {
        tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void LoadStartingAbilitiesIntoAbilityBar()
    {
        var playerAbilities = GameManager.Instance.Player.Abilities;

        if (playerAbilities.Count < 1)
        {
            return;
        }

        var mappingIndex = 0;

        foreach (var ability in playerAbilities.Values)
        {
            var mapping = _abilityMap.ElementAt(mappingIndex).Key;

            AbilityManager.AssignAbilityToButton(ability, _abilityMap[mapping]);

            mappingIndex++;
        }
    }

    private bool IsUiClicked()
    {
        if (Canvas == null)
        {
            Canvas = GameObject.Find("UI");
        }
        _canvasGraphicRaycaster = Canvas.GetComponent<GraphicRaycaster>();
        _canvasEventSystem = Canvas.GetComponent<EventSystem>();

        var pointerEventData = new PointerEventData(_canvasEventSystem) { position = Input.mousePosition };

        var results = new List<RaycastResult>();

        _canvasGraphicRaycaster.Raycast(pointerEventData, results);

        return results.Any();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.LoadAbilityBarEventName))
        {
            _abilityMap = (Dictionary<KeyCode, GameObject>) parameter;

            if (_abilityMap == null)
            {
                return;
            }

            LoadStartingAbilitiesIntoAbilityBar();
        }

        if (eventName.Equals(GlobalHelper.AwaitingInputElsewhereEventName))
        {
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.InputReceivedEventName, this);
            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AwaitingInputElsewhereEventName, this);

            _awaitingInputElsewhere = true;
        }

        if (eventName.Equals(GlobalHelper.InputReceivedEventName))
        {
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.AwaitingInputElsewhereEventName, this);
            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.InputReceivedEventName, this);

            _awaitingInputElsewhere = false;
        }

        if (eventName.Equals(GlobalHelper.SingleTileAbilityEventName))
        {
            if (!(broadcaster is Ability ability))
            {
                return;
            }

            _picker = new TargetPicker(ability);

            _selectedTarget = _picker.GetCurrentTarget();

            if (_selectedTarget == null)
            {
                return;
            }

            HighlightTile(_selectedTarget.CurrentTile);

            _currentInputMode = InputMode.TargetPicker;
        }
    }
}

    
