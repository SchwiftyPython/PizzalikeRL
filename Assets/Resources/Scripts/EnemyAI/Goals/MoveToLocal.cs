using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLocal : Goal
{
    private readonly int _x;
    private readonly int _y;
    private readonly int _maxTurns;
    private readonly Area _area;

    private MonoHelper _monoHelper;

    public MoveToLocal(Area area, int x, int y)
    {
        _area = area;
        _x = x;
        _y = y;
        _maxTurns = -1;
    }

    public MoveToLocal(Area area, int x, int y, int turns)
    {
        _area = area;
        _x = x;
        _y = y;
        _maxTurns = turns;
    }

    public override bool Finished()
    {
        return !ParentController.IsMobile() || (int) ParentController.Self.CurrentTile.GetGridPosition().x == _x &&
               (int) ParentController.Self.CurrentTile.GetGridPosition().y == _y;
    }

    public override void TakeAction()
    {
        if (!ParentController.IsMobile())
        {
            FailToParent();
            return;
        }
        if (ParentController.Self.CurrentArea == null)
        {
            Pop();
            return;
        }
        if (_area == null)
        {
            Pop();
            return;
        }
        if (_monoHelper == null)
        {
            _monoHelper = ParentController.Self.GetSprite().AddComponent<MonoHelper>();
        }
        //ParentController.FindPathToTarget(ParentController.Self.CurrentPosition, new Vector2(_x, _y));
        _monoHelper.StartCoroutine(GetPath());

        if (ParentController.Path.vectorPath.Count > 1)
        {
            var translatedPath = TranslatePathToDirections();
            var numTurns = 0;
            foreach (var node in translatedPath)
            {
                PushGoal(new Step(node));
                numTurns++;
                if (numTurns >= _maxTurns)
                {
                    break;
                }
            }
        }
        else
        {
            FailToParent();
        }

    }

    private IEnumerator GetPath()
    {
        ParentController.FindPathToTarget(ParentController.Self.CurrentPosition, new Vector2(_x, _y));
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerable<GoalDirection> TranslatePathToDirections()
    {
        var translatedPath = new List<GoalDirection>();
        var path = ParentController.Path.vectorPath;

        for (var i = 0; i < path.Count - 1; i++)
        {
            translatedPath.Add(GetDirectionFromTwoPoints(path[i], path[i+1]));
        }
        translatedPath.Reverse(0, translatedPath.Count);
        return translatedPath;
    }

    private GoalDirection GetDirectionFromTwoPoints(Vector2 startPoint, Vector2 endPoint)
    {
        var difference = startPoint - endPoint;

        if (difference == Vector2.up)
        {
            return GoalDirection.North;
        }
        if (difference == new Vector2(1, 1))
        {
            return GoalDirection.NorthEast;
        }
        if (difference == Vector2.right)
        {
            return GoalDirection.East;
        }
        if (difference == new Vector2(1, -1))
        {
            return GoalDirection.SouthEast;
        }
        if (difference == Vector2.down)
        {
            return GoalDirection.South;
        }
        if (difference == new Vector2(-1, -1))
        {
            return GoalDirection.SouthWest;
        }
        if (difference == Vector2.left)
        {
            return GoalDirection.West;
        }
        if (difference == new Vector2(-1, 1))
        {
            return GoalDirection.NorthWest;
        }
        FailToParent();
        return GoalDirection.North;
    }

    internal class MonoHelper : MonoBehaviour
    {
        public IEnumerator DoCoroutine(IEnumerator cor)
        {
            while (cor.MoveNext())
                yield return cor.Current;
        }
    }
}
