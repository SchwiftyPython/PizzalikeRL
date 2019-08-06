using UnityEngine;

public class Step : Goal
{
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

        var directionDictionary = GlobalHelper.DirectionVectorDictionary;

        var currentPosition = new Vector2(ParentController.Self.CurrentTile.X, ParentController.Self.CurrentTile.Y);
        var targetposition = new Vector2(
            (int) (currentPosition.x + directionDictionary[StepDirection].x),
            (int) (currentPosition.y + directionDictionary[StepDirection].y));

        if (!ParentController.Self.AreaMapCanMoveLocal(targetposition))
        {
            FailToParent();
            return;
        }
        ParentController.Self.AreaMove(targetposition);
        Pop();
    }
}
