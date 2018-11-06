using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : Goal
{
    public Dictionary<GoalDirection, Vector2> DirectionDictionary = new Dictionary<GoalDirection, Vector2>
    {
        {GoalDirection.North, Vector2.up},
        {GoalDirection.NorthEast, new Vector2(1, 1)},
        {GoalDirection.East, Vector2.right},
        {GoalDirection.SouthEast, new Vector2(1, -1)},
        {GoalDirection.South, Vector2.down},
        {GoalDirection.SouthWest, new Vector2(-1, -1)},
        {GoalDirection.West, Vector2.left},
        {GoalDirection.NorthWest, new Vector2(-1, 1)}
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

        var currentPosition = ParentController.Self.CurrentPosition;
        var targetposition = new Vector2(
            (int) (currentPosition.x + DirectionDictionary[StepDirection].x),
            (int) (currentPosition.y + DirectionDictionary[StepDirection].y));

        if (!ParentController.Self.AreaMapCanMove(targetposition))
        {
            FailToParent();
            return;
        }
        ParentController.Self.AreaMove(targetposition);
        Pop();
    }
}
