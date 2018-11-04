using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLocal : Goal
{
    private int _x;
    private int _y;
    private int _maxTurns;
    private Area _area;


    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

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
        ParentController.FindPathToTarget(ParentController.Self.CurrentPosition, new Vector2(_x, _y));

        //todo add step for each path element
    }
}
