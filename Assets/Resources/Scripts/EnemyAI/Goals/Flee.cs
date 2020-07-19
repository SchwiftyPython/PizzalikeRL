public class Flee : Goal
{
    public Entity ScaryThing;

    public int Duration;

    public Flee(Entity scaryThing, int duration)
    {
        ScaryThing = scaryThing;
        Duration = duration;
    }

    public Flee(Entity scaryThing)
    {
        ScaryThing = scaryThing;
        Duration = 3;

        // var entityName = Self.Fluff != null ? Self.Fluff.Name : Self.EntityType;
        //
        // EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, $@"{entityName} is fleeing!");
    }

    public override void Push(EnemyController parentController)
    {
        if (parentController.Self == ScaryThing)
        {
            return;
        }
        base.Push(parentController);
    }

    public override void TakeAction()
    {
        if (Duration > 0)
        {
            Duration--;
        }
        else if (Duration <= 0)
        {
            FailToParent();
            return;
        }

        if (ScaryThing == null || ScaryThing.IsDead())
        {
            FailToParent();
            return;
        }

        var directionFromTile = ScaryThing.CurrentTile.GetDirectionFromTile(Self.CurrentTile);

        if (directionFromTile == null)
        {
            FailToParent();
            return;
        }

        var controller = Self.GetSprite().GetComponent<EnemyController>();

        if (controller == null)
        {
            FailToParent();
            return;
        }

        controller.PushGoal(new Step((GoalDirection) directionFromTile));
    }

    public override bool Finished()
    {
        return Duration <= 0;
    }
}
