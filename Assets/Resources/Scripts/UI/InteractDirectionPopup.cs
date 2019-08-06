using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractDirectionPopup : MonoBehaviour, ISubscriber
{
    private IDictionary<KeyCode, GoalDirection> _keypadDirections = new Dictionary<KeyCode, GoalDirection>
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

    private void Awake()
    {
        EventMediator.Instance.SubscribeToEvent("Interact", this);

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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }

            GoalDirection chosenDirection;

            foreach (var keypadDirection in _keypadDirections)
            {
                if (Input.GetKeyDown(keypadDirection.Key))
                {
                    chosenDirection = keypadDirection.Value;
                    break;
                }
            }

            //todo get vector from globalhelper and get tile in selected direction
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
        _listeningForInput = true;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        _listeningForInput = false;
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        //todo check for open windows like game menu and whatnot
        //todo we'll keep a list of windows in Game manager, loop through for any active

        Show();
    }
}
