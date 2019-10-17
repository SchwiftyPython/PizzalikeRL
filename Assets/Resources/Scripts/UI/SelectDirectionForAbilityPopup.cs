using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectDirectionForAbilityPopup : MonoBehaviour, ISubscriber
{
    private readonly IDictionary<KeyCode, GoalDirection> _keypadDirections = new Dictionary<KeyCode, GoalDirection>
    {
        { KeyCode.Keypad7, GoalDirection.NorthWest },
        { KeyCode.Keypad8, GoalDirection.North },
        { KeyCode.Keypad9, GoalDirection.NorthEast },
        { KeyCode.Keypad4, GoalDirection.West },
        { KeyCode.Keypad6, GoalDirection.East },
        { KeyCode.Keypad1, GoalDirection.SouthWest },
        { KeyCode.Keypad2, GoalDirection.South },
        { KeyCode.Keypad3, GoalDirection.SouthEast }
    };

    private Color _highlightedColor = Color.cyan;
    private List<Tile> _highlightedTiles;

    private int _abilityRange;

    private bool _listeningForInput;

    private object _broadcaster;

    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.DirectionalAbilityEventName, this);

        if (gameObject.activeSelf)
        {
            Hide();
        }
    }

    
    private void Update()
    {
        if (Input.anyKeyDown && _listeningForInput)
        {
            _listeningForInput = false;

            var chosenDirection = GoalDirection.North;
            var validDirectionChosen = false;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
                return;
            }

            foreach (var keypadDirection in _keypadDirections)
            {
                if (Input.GetKeyDown(keypadDirection.Key))
                {
                    chosenDirection = keypadDirection.Value;
                    validDirectionChosen = true;
                    break;
                }
            }

            if (!validDirectionChosen)
            {
                _listeningForInput = true;
                return;
            }

            var currentTile = GameManager.Instance.Player.CurrentTile;

            var directionVector = GlobalHelper.GetVectorForDirection(chosenDirection);

            var targetVector = new Vector2(currentTile.X + directionVector.x, currentTile.Y + directionVector.y);

            var targetTile = GameManager.Instance.CurrentArea.AreaTiles[(int)targetVector.x, (int)targetVector.y];

            var targetEntity = targetTile.GetPresentEntity();

            var i = 1;

            while (targetEntity == null && i < _abilityRange)
            {
                targetVector = new Vector2(targetTile.X + directionVector.x, targetTile.Y + directionVector.y);

                try
                {
                    targetTile = GameManager.Instance.CurrentArea.AreaTiles[(int)targetVector.x, (int)targetVector.y];
                }
                catch (Exception)
                {
                    break;
                }

                //todo check if tile blocks movement

                targetEntity = targetTile.GetPresentEntity();

                i++;
            }

            if (targetEntity == null)
            {
                //todo broadcast message no valid target
                Debug.Log("No valid target for ability!");

                _listeningForInput = true;
                return;
            }
            
            var directionStruct = new DirectionStruct { target = targetEntity, direction = chosenDirection };

            EventMediator.Instance.Broadcast(GlobalHelper.AbilityTileSelectedEventName, this, directionStruct);

            Hide();
        }
    }

    private void HighlightTilesInRange()
    {
        _highlightedTiles = new List<Tile>();

        var directions = Enum.GetValues(typeof(GoalDirection));

        foreach (var direction in directions)
        {
            _highlightedTiles.AddRange(HighLightTilesInDirection((GoalDirection) direction, _abilityRange));
        }
    }

    private IEnumerable<Tile> HighLightTilesInDirection(GoalDirection direction, int distance)
    {
        var directionVector = GlobalHelper.GetVectorForDirection(direction);

        var areaTiles = GameManager.Instance.CurrentArea.AreaTiles;

        var highlightedTiles = new List<Tile>();

        var currentTile = GameManager.Instance.CurrentTile;

        for (var i = 0; i < distance; i++)
        {
            var nextTileId = new Vector2(currentTile.X + directionVector.x, currentTile.Y + directionVector.y);

            try
            {
                currentTile = areaTiles[(int)nextTileId.x, (int)nextTileId.y];
            }
            catch (Exception)
            {
                break;
            }

            //todo check if tile blocks movement

            HighlightTile(currentTile);

            highlightedTiles.Add(currentTile);
        }

        return highlightedTiles;
    }

    private void HighlightTile(Tile tile)
    {
        tile.TextureInstance.GetComponent<SpriteRenderer>().color = _highlightedColor;
    }

    private void ClearHighlights()
    {
        if (_highlightedTiles == null || _highlightedTiles.Count < 1)
        {
            return;
        }

        foreach (var tile in _highlightedTiles)
        {
            tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void Show()
    {
        HighlightTilesInRange();
        gameObject.SetActive(true);
        _listeningForInput = true;
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void Hide()
    {
        ClearHighlights();
        gameObject.SetActive(false);
        _listeningForInput = false;
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.DirectionalAbilityEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (GameManager.Instance.AnyActiveWindows())
        {
            return;
        }

        _broadcaster = broadcaster;

        if (!(parameter is int abilityRange))
        {
            _abilityRange = 1;
        }
        else
        {
            _abilityRange = abilityRange;
        }

        if (eventName == GlobalHelper.DirectionalAbilityEventName)
        {
            Show();
        }
    }
}
