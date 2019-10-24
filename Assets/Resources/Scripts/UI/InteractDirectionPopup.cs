using System.Collections.Generic;
using UnityEngine;

public class InteractDirectionPopup : MonoBehaviour, ISubscriber
{
    //todo throw in global helper
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

    private bool _listeningForInput;
    private Vector2 _startingPosition = new Vector2(-185, 65);

    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.InteractEventName, this);

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

            GoalDirection chosenDirection = GoalDirection.North;
            bool validDirectionChosen = false;

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

            var targetTile = GameManager.Instance.CurrentArea.AreaTiles[(int) targetVector.x, (int) targetVector.y];

            if (targetTile.PresentProp is Grave)
            {
                //todo show grave inscription
            }

            EventMediator.Instance.Broadcast("DroppedItemPopup", this, targetTile);

            Hide();
        }
    }

    private void Show()
    {
        transform.localPosition = _startingPosition;
        if (PopupIsObstructingPlayer())
        {

        }

        gameObject.SetActive(true);
        _listeningForInput = true;
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        _listeningForInput = false;
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private bool PopupIsObstructingPlayer()
    {
        var playerSprite = GameManager.Instance.Player.GetSprite();

        var popupRectTransform = gameObject.GetComponent<RectTransform>();

        var playerPositionWorld = transform.TransformPoint(playerSprite.transform.position);

        if (RectTransformUtility.RectangleContainsScreenPoint(popupRectTransform, playerPositionWorld))
        {
            return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.InteractEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (GameManager.Instance.AnyActiveWindows())
        {
            return;
        }

        Show();
    }
}
