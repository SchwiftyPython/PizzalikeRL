using System.Collections.Generic;
using UnityEngine;

public class Step : Goal
{
    public Dictionary<GoalDirection, Vector2> DirectionDictionary = new Dictionary<GoalDirection, Vector2>
    {
        {GoalDirection.North, new Vector2(1, 0)},
        {GoalDirection.NorthEast, new Vector2(1, 1)},
        {GoalDirection.East, new Vector2(0, 1)},
        {GoalDirection.SouthEast, new Vector2(-1, 1)},
        {GoalDirection.South, new Vector2(-1, 0)},
        {GoalDirection.SouthWest, new Vector2(-1, -1)},
        {GoalDirection.West, new Vector2(0, -1)},
        {GoalDirection.NorthWest, new Vector2(1, -1)}
    };

    public GoalDirection StepDirection;

    public Step(GoalDirection direction)
    {
        StepDirection = direction;
    }

    public override bool Finished()
    {
        return false;
    }

    public override void TakeAction()
    {
        if (!ParentController.IsMobile())
        {
            Pop();
            return;
        }

        var currentPosition = new Vector2(ParentController.Self.CurrentTile.X, ParentController.Self.CurrentTile.Y);
        var targetposition = new Vector2(
            (int) (currentPosition.x + DirectionDictionary[StepDirection].x),
            (int) (currentPosition.y + DirectionDictionary[StepDirection].y));

        if (!ParentController.Self.AreaMapCanMoveLocal(targetposition))
        {
            FailToParent();
            return;
        }
        ParentController.Self.AreaMove(targetposition);
        Pop();
    }
}
