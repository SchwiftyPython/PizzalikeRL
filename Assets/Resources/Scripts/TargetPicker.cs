using System;
using System.Collections.Generic;

public class TargetPicker
{
    private Entity _selectedTarget;
    private Queue<Entity> _validTargets;

    private Tile[,] _currentAreaTiles;
    private readonly List<Entity> _presentEntities;

    public TargetPicker(Ability ability)
    {
        _currentAreaTiles = GameManager.Instance.CurrentArea.AreaTiles;
        _presentEntities = GameManager.Instance.CurrentArea.PresentEntities;

        GetAllValidTargets(ability.Range);
    }

    public Entity GetCurrentTarget()
    {
        return _validTargets.Count > 0 ? _validTargets.Peek() : null;
    }

    public Entity GetNextTarget()
    {
        if (_validTargets.Count <= 1)
        {
            return _validTargets.Peek();
        }

        var target = _validTargets.Dequeue();
        _validTargets.Enqueue(target);
        return _validTargets.Peek();
    }

    private void GetAllValidTargets(int range)
    {
        _selectedTarget = null;

        var tempList = new Queue<Entity>();
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

            tempList.Enqueue(currentEntity);
        }

        _validTargets = tempList;

        if (_validTargets.Count > 0)
        {
            _selectedTarget = _validTargets.Peek();
        }
        else
        {
            _selectedTarget = null;
        }
    }

    private static int CalculateDistanceToTarget(Entity target)
    {
        var currentTile = GameManager.Instance.CurrentTile;

        var a = target.CurrentTile.X - currentTile.X;
        var b = target.CurrentTile.Y - currentTile.Y;

        return (int)Math.Sqrt(a * a + b * b);
    }
}
