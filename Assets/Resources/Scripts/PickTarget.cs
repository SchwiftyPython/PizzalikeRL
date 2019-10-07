using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickTarget : MonoBehaviour, ISubscriber
{
    private bool _pickerActive;
    private bool _processingInput;
    private Tile _selectedTile;
    private Entity _selectedTarget;
    private List<Entity> _validTargets;

    private Tile[,] _currentAreaTiles;
    private List<Entity> _presentEntities;

    private readonly Color _highlightedColor = Color.cyan;

    private void Start()
    {
        _pickerActive = false;
        _processingInput = false;

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.SingleTileAbilityEventName, this);
    }

    private void Update()
    {
        if (_pickerActive && !_processingInput)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _processingInput = true;
                MoveToNextTarget();
                _processingInput = false;
            }
            
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                EventMediator.Instance.Broadcast(GlobalHelper.InputReceivedEventName, this);
                _pickerActive = false;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                //todo check if target is in range
                //todo return via broadcast

                _processingInput = true;
                _pickerActive = false;

                Debug.Log("Ability Target Selected");

                EventMediator.Instance.Broadcast(GlobalHelper.AbilityTileSelectedEventName, this, _selectedTarget);

                EventMediator.Instance.Broadcast(GlobalHelper.InputReceivedEventName, this);
            }
        }
    }

    private void MoveToNextTarget()
    {
        //todo cycle through list of valid targets
        //todo highlight tile or show popup of target info. Some kind of indicator
    }

    private Entity GetEntityAt(int x, int y)
    {
        return GetTileAt(x, y).GetPresentEntity();
    }

    private void GetAllValidTargets(int range)
    {
        _validTargets = new List<Entity>();

        foreach (var currentEntity in _presentEntities)
        {
            var distance = CalculateDistanceToTarget(currentEntity);

            if (distance > range)
            {
                continue;
            }

            _validTargets.Add(currentEntity);
        }
    }

    private Tile GetTileAt(int x, int y)
    {
        if (!ReferenceEquals(_currentAreaTiles, GameManager.Instance.CurrentArea.AreaTiles))
        {
            _currentAreaTiles = GameManager.Instance.CurrentArea.AreaTiles;
        }

        return _currentAreaTiles[x, y];
    }

    private static int CalculateDistanceToTarget(Entity target)
    {
        var currentTile = GameManager.Instance.CurrentTile;

        var a = target.CurrentTile.X - currentTile.X;
        var b = target.CurrentTile.Y - currentTile.Y;

        return (int)Math.Sqrt(a * a + b * b);
    }

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
            _presentEntities = GameManager.Instance.CurrentArea.PresentEntities;

            GetAllValidTargets(range); 
        }
    }
}
