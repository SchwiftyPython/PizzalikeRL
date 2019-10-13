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
    private Entity[] _validTargets;

    private int _targetIndex;

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
                _selectedTarget = _validTargets[_targetIndex];

                ClearHighlight(_selectedTarget.CurrentTile);
                EventMediator.Instance.Broadcast(GlobalHelper.InputReceivedEventName, this);
                _pickerActive = false;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                //todo check if target is in range
                //todo return via broadcast

                _processingInput = true;
                _pickerActive = false;
                //ClearHighlight(_selectedTile);

//                _selectedTarget = _validTargets[_targetIndex];
//                ClearHighlight(_selectedTarget.CurrentTile);

                Tile highlightedTile = null; 
                foreach (var tile in _currentAreaTiles)
                {
                    if (tile.TextureInstance.GetComponent<SpriteRenderer>().color == _highlightedColor)
                    {
                        highlightedTile = tile;
                        break;
                    }
                }

                Debug.Log("Ability Target Selected");

                if (highlightedTile != null)
                {
                    ClearHighlight(highlightedTile);

                    EventMediator.Instance.Broadcast(GlobalHelper.AbilityTileSelectedEventName, this,
                        highlightedTile.GetPresentEntity());
                }

                EventMediator.Instance.Broadcast(GlobalHelper.InputReceivedEventName, this);
            }
        }
    }

    private void MoveToNextTarget()
    {
        if (_validTargets == null || _validTargets.Length < 1)
        {
            return;
        }

        var tile = _currentAreaTiles[_validTargets[_targetIndex].CurrentTile.X,
            _validTargets[_targetIndex].CurrentTile.Y];
        ClearHighlight(tile);

        _targetIndex++;

        if (_targetIndex >= _validTargets.Length)
        {
            _targetIndex = 0;
        }

        tile = _currentAreaTiles[_validTargets[_targetIndex].CurrentTile.X,
            _validTargets[_targetIndex].CurrentTile.Y];
        tile.TextureInstance.GetComponent<SpriteRenderer>().color = _highlightedColor;

        Debug.Log("Current index: " + _targetIndex);

        //        if (_selectedTarget != null)
        //        {
        //            _validTargets.Enqueue(_selectedTarget);
        //            ClearHighlight(_selectedTarget.CurrentTile);
        //        }

        //_selectedTarget = _validTargets.Dequeue();

        //        var lastSelection = _validTargets.Dequeue();
        //
        //        ClearHighlight(lastSelection.CurrentTile);
        //
        //        _validTargets.Enqueue(lastSelection);
        //
        //        var tile = _validTargets.Peek().CurrentTile;
        //        tile.TextureInstance.GetComponent<SpriteRenderer>().color = _highlightedColor;
        //
        //        _selectedTile = tile;
    }

    private void ClearHighlight(Tile tile)
    {
        tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private Entity GetEntityAt(int x, int y)
    {
        return GetTileAt(x, y).GetPresentEntity();
    }

    private void GetAllValidTargets(int range)
    {
        _selectedTarget = null;
        _selectedTile = null;
        _targetIndex = 0;

        var tempList = new List<Entity>();
        foreach (var currentEntity in _presentEntities)
        {
            if (currentEntity == GameManager.Instance.Player)
            {
                continue;
            }

            var distance = CalculateDistanceToTarget(currentEntity);

            if (distance > range)
            {
                continue;
            }

            tempList.Add(currentEntity);
        }

        _validTargets = tempList.ToArray();
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

            if (_validTargets == null || _validTargets.Length < 1)
            {
                EventMediator.Instance.Broadcast(GlobalHelper.InputReceivedEventName, this);
                _pickerActive = false;

                Debug.Log("No valid targets in range!");

                return;
            }

            MoveToNextTarget();

            _pickerActive = true;
            _processingInput = false;
            EventMediator.Instance.Broadcast(GlobalHelper.AwaitingInputElsewhereEventName, this);
        }
    }
}
