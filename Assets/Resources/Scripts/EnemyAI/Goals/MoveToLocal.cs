using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveToLocal : Goal
{
    private const int MaxVisits = 3;

    private readonly int _x;
    private readonly int _y;
    private readonly int _maxTurns;
    private readonly Area _area;

    private int _numVisits;

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
        var self = ParentController.Self;

        if (self.CurrentTile == null)
        {
            self.CurrentTile = _area.AreaTiles[(int) self.CurrentPosition.y,
                (int) self.CurrentPosition.x];
        }

        return !ParentController.IsMobile() || _numVisits > MaxVisits || self.CurrentTile.X == _x &&
               self.CurrentTile.Y == _y;
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

        ParentController.FindPathToTarget(
            new Vector2(ParentController.Self.CurrentPosition.x, ParentController.Self.CurrentPosition.y), new Vector2(_y, _x));

        if (ParentController.Path.vectorPath.Count > 1)
        {
            var translatedPath = TranslatePathToDirections();
            var numTurns = 0;
            foreach (var node in translatedPath)
            {
                PushGoal(new Step(node));
                numTurns++;
                if (_maxTurns > -1 && numTurns >= _maxTurns)
                {
                    break;
                }
            }
        }
        else
        {
            FailToParent();
        }
        _numVisits++;
    }

    private IEnumerable<GoalDirection> TranslatePathToDirections()
    {
        var translatedPath = new List<GoalDirection>();
        var path = ParentController.Path.vectorPath;

        for (var i = 0; i < path.Count - 1; i++)
        {
            var direction = GetDirectionFromTwoPoints(path[i], path[i + 1]);

            if (direction == null)
            {
                continue;
            }

            translatedPath.Add((GoalDirection) direction);
        }
        translatedPath.Reverse(0, translatedPath.Count);
        return translatedPath;
    }

    private GoalDirection? GetDirectionFromTwoPoints(Vector2 startPoint, Vector2 endPoint)
    {
        var difference = endPoint - startPoint;

        if (difference.x > 0 && difference.y == 0)
        {
            return GoalDirection.East;
        }
        if (difference.x > 0 && difference.y > 0)
        {
            return GoalDirection.NorthEast;
        }
        if (difference.x == 0 && difference.y > 0)
        {
            return GoalDirection.North;
        }
        if (difference.x < 0 && difference.y > 0)
        {
            return GoalDirection.NorthWest;
        }
        if (difference.x < 0 && difference.y == 0)
        {
            return GoalDirection.West;
        }
        if (difference.x < 0 && difference.y < 0)
        {
            return GoalDirection.SouthWest;
        }
        if (difference.x == 0 && difference.y < 0)
        {
            return GoalDirection.South;
        }
        if (difference.x > 0 && difference.y < 0)
        {
            return GoalDirection.SouthEast;
        }
        return null;
    }

    private static List<Vector3> RoundPath(IReadOnlyList<Vector3> path)
    {
        var roundedPath = new List<Vector3> {path.First()};

        for (var i = 1; i < path.Count; i++)
        {
            var startX = roundedPath[i - 1].x;
            var startY = roundedPath[i - 1].y;

            var endX = path[i].x;
            var endY = path[i].y;

            if (Mathf.Abs(startX - endX) > 1)
            {
                endX = Mathf.Floor(endX);
            }
            else
            {
                endX = Mathf.Ceil(endX);
            }

            if (Mathf.Abs(startY - endY) > 1)
            {
                endY = Mathf.Floor(endY);
            }
            else
            {
                endY = Mathf.Ceil(endY);
            }

            roundedPath.Add(new Vector3(endX, endY, -0.1f));
        }

        return roundedPath;
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
