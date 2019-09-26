using System;
using System.Collections.Generic;
using UnityEngine;

public class PickTarget : MonoBehaviour, ISubscriber
{
    private bool _pickerActive;
    private List<Tile> _selectedTiles;

    public GameObject TileOverlayPrefab; //todo pizzas?! lol
    private List<Tile> _highlightedTiles;

    private Tile[,] _currentAreaTiles;

    private void Start()
    {
        _pickerActive = false;

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.SingleTileAbilityEventName, this);
    }

    private void Update()
    {
        if (_pickerActive)
        {
            //todo highlight tiles under mouse or where keyboard moves shape

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                //Attempt move up
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                //Attempt move diagonal up and left
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                //Attempt move left
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                //Attempt move diagonal down and left                
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                //Attempt move down
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                //Attempt move diagonal down and right
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                //Attempt move right
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                //Attempt move diagonal up and right
                
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                //todo cancel
            }

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                //todo check if tiles are in range
                //todo add tile to _selected tiles
                //todo return via broadcast
            }
        }
    }

    private Entity GetEntityAt(int x, int y)
    {
        return GetTileAt(x, y).GetPresentEntity();
    }

    private Tile GetTileAt(int x, int y)
    {
        return GameManager.Instance.CurrentArea.GetTileAt(new Vector3(x, y));
    }

    private void ShowSingleTilePicker(int range, Vector2 startPosition)
    {
        Tile targetTile = null;

        //todo pick some adjacent tile to start
        foreach (GoalDirection direction in Enum.GetValues(typeof(GoalDirection)))
        {
            var vector = GlobalHelper.GetVectorForDirection(direction);

            var target = startPosition + vector;

            try
            {
                targetTile = _currentAreaTiles[(int) target.x, (int) target.y];
            }
            catch (Exception)
            {
                continue;
            }

            if (!targetTile.IsWall())
            {
                break;
            }
        }

        if (targetTile == null || targetTile.IsWall())
        {
            Debug.Log("Nowhere to place ability selector!");
            return;
        }

        var instance = Instantiate(TileOverlayPrefab, new Vector2(targetTile.Y, targetTile.X), Quaternion.identity);
    }

    //todo area of effect picker Range, Size, Vector2 some corner or start point

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.SingleTileAbilityEventName))
        {
            if (!(broadcaster is Entity abilityUser))
            {
                return;
            }

            if (!(parameter is int range))
            {
                return;
            }

            _currentAreaTiles = GameManager.Instance.CurrentArea.AreaTiles;

            ShowSingleTilePicker(range, new Vector2(abilityUser.CurrentTile.X, abilityUser.CurrentTile.Y));
        }
    }
}
