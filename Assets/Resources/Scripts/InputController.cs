using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private LayerMask _clickable;

    private bool _popupWindowOpen;

    public static InputController Instance;

    private Entity _player;
    private Seeker _seeker;

    public Path Path;
    public bool PathCalculated;

    public Color HighlightedColor;
    private List<Tile> _highlightedTiles;

    public bool ActionTaken; 
    //Vector2 target;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // Destroy the current object, so there is just one 
            Destroy(gameObject);
        }

        /*
        BUG: NOT GETTING REFERENCE TO PLAYER IN START

        //while (WorldManager.instance.player == null) {}
        player = WorldManager.instance.player;

        Debug.Log ("player reference in start: " + player);

        */
    }

    private void Update()
    {
        var currentScene = GameManager.Instance.CurrentScene.name;

        if (GameManager.Instance.CurrentState == GameManager.GameState.Playerturn)
        {
            if (_player == null)
            {
                _player = GameManager.Instance.Player;
                //Debug.Log ("player reference in update: " + _player);
            }

            if (_player.GetSprite() == null)
            {
                AreaMap.Instance.InstantiatePlayerSprite();
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                //Attempt move up                
                var target = new Vector2(_player.CurrentPosition.x, _player.CurrentPosition.y + 1);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                //Attempt move diagonal up and left                
                var target = new Vector2(_player.CurrentPosition.x - 1, _player.CurrentPosition.y + 1);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                //Attempt move left
                var target = new Vector2(_player.CurrentPosition.x - 1, _player.CurrentPosition.y);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                //Attempt move diagonal down and left                
                var target = new Vector2(_player.CurrentPosition.x - 1, _player.CurrentPosition.y - 1);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                //Attempt move down
                var target = new Vector2(_player.CurrentPosition.x, _player.CurrentPosition.y - 1);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                //Attempt move diagonal down and right
                var target = new Vector2(_player.CurrentPosition.x + 1, _player.CurrentPosition.y - 1);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                //Attempt move right
                var target = new Vector2(_player.CurrentPosition.x + 1, _player.CurrentPosition.y);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                //Attempt move diagonal up and right
                var target = new Vector2(_player.CurrentPosition.x + 1, _player.CurrentPosition.y + 1);
                if (_player.MoveOrAttackSuccessful(target))
                {
                    ActionTaken = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                var mainWindow = GameMenuWindow.Instance.MainWindow;

                if (mainWindow.activeSelf && !GameMenuWindow.Instance.FilteredInventoryWindow.activeSelf)
                {
                    GameMenuWindow.Instance.HideMainWindow();
                    _popupWindowOpen = false;
                }
                else if (ActionWindow.Instance.Window.activeSelf)
                {
                    ActionWindow.Instance.Window.SetActive(false);
                    ClearHighlights();
                }
                else
                {
                    GameMenuWindow.Instance.ShowMainWindow();
                    _popupWindowOpen = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                if (GameMenuWindow.Instance.MainWindow.activeSelf)
                {
                    if (GameMenuWindow.Instance.CurrentWindow == GameMenuWindow.Instance.PizzaOrderJournal)
                    {
                        GameMenuWindow.Instance.HideMainWindow();
                        _popupWindowOpen = false;
                    }
                    else
                    {
                        GameMenuWindow.Instance.OnTabSelected(GameMenuWindow.Instance.PizzaOrderJournalTab);
                    }
                }
                else
                {
                    GameMenuWindow.Instance.ShowMainWindow();
                    _popupWindowOpen = true;
                    if (GameMenuWindow.Instance.CurrentWindow != GameMenuWindow.Instance.PizzaOrderJournal)
                    {
                        GameMenuWindow.Instance.OnTabSelected(GameMenuWindow.Instance.PizzaOrderJournalTab);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (currentScene.Equals("Area") && !ActionWindow.Instance.isActiveAndEnabled &&
                    !GameMenuWindow.Instance.MainWindow.activeSelf && !AreaMap.Instance.ObjectInfoWindow.activeSelf)
                {
                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (pos.x < 0 || pos.y < 0 || pos.x > GameManager.Instance.CurrentArea.Width ||
                        pos.y > GameManager.Instance.CurrentArea.Height)
                    {
                        return;
                    }

                    var selectedTile = GameManager.Instance.CurrentArea.AreaTiles[(int)pos.x, (int)pos.y];

                    //highlight tile and path to it
                    StartCoroutine(HighlightPathToTarget(_player, selectedTile.GetGridPosition()));

                    ActionWindow.Instance.OnTileSelected(selectedTile);
                }

            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (currentScene.Equals("Area"))
                {
                    var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        Vector2.positiveInfinity);

                    if (hit && !_popupWindowOpen)
                    {
                        hit.collider.GetComponent<EntityInfo>()?.OnRightClick();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (currentScene.Equals("Area"))
                {
                    SceneManager.LoadScene("WorldMap");
                }
            }
            else if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (currentScene.Equals("WorldMap"))
                {
                    GameManager.Instance.PlayerEnteringAreaFromWorldMap = true;
                    GameManager.Instance.CurrentArea = GameManager.Instance.CurrentCell.Areas[1, 1];
                    GameManager.Instance.Player.CurrentArea = GameManager.Instance.CurrentArea;
                    GameManager.Instance.CurrentState = GameManager.GameState.EnterArea;
                    SceneManager.LoadScene("Area");
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
            var tile = currentArea.AreaTiles[(int) tilePosition.x, (int) tilePosition.y];
            tile.TextureInstance.GetComponent<SpriteRenderer>().color = HighlightedColor;
            _highlightedTiles.Add(tile);
        }
    }

    public void ClearHighlights()
    {
        if (_highlightedTiles == null || _highlightedTiles.Count <= 0)
        {
            return;
        }

        foreach (var tile in _highlightedTiles)
        {
            tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}

    
