using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private LayerMask _clickable;

    private bool _popupWindowOpen;

    public static InputController Instance;

    Entity _player;

    public bool ActionTaken; //for basic AI pathfinding testing
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
        if (GameManager.Instance.CurrentState == GameManager.GameState.Playerturn)
        {
            if (_player == null)
            {
                _player = GameManager.Instance.Player;
                //Debug.Log ("player reference in update: " + _player);
            }

            if (_player.GetSprite() == null)
            {
                _player.SetSprite(GameManager.Instance.PlayerSprite);
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

                if (mainWindow.activeSelf)
                {
                    GameMenuWindow.Instance.HideMainWindow();
                    _popupWindowOpen = false;
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
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.positiveInfinity);

                if (hit && !_popupWindowOpen)
                {
                    hit.collider.GetComponent<EntityInfo>()?.OnLeftClick();
                }
            }
        }
    }
}

    
