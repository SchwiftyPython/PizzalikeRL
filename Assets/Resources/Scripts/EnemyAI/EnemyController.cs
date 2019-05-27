using System.Collections.Generic;

public class EnemyController : AstarAI
{
    public bool ActionTaken; //for basic AI pathfinding testing
    public bool TurnStarted;
    public bool Mobile;

    public Goal ParentGoal;
    public Entity Self;

    public Stack<Goal> Goals;

    public void TakeAction()
    {
        TurnStarted = true;

        if (Self == null)
        {
            return;
        }

        if (Goals == null)
        {
            if (Self.Goals == null)
            {
                Self.Goals = new Stack<Goal>();
            }
            Goals = Self.Goals;
        }

        if (Goals.Count == 0)
        {
            //Debug.Log(Self + " is bored.");
            new Bored().Push(this);
            Goals.Peek().TakeAction();

            //Find path
            if (Goals.Peek().GetType() == typeof(MoveToLocal))
            {
                Goals.Peek().TakeAction();
            }
        }

        while (Goals.Count > 0 && Goals.Peek().Finished())
        {
            Goals.Pop();
        }

        if (Goals.Count > 0)
        {
            Goals.Peek().TakeAction();
        }
        GameManager.Instance.CurrentState = GameManager.GameState.EndTurn;
    }

    public void PushGoal(Goal goal)
    {
        goal.Push(this);
    }

    public bool IsMobile()
    {
        return Self.Mobile;
    }
}
