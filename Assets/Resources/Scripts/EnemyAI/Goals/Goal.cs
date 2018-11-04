using System.Collections.Generic;

public class Goal
{
    private List<Goal> _possibleGoals;

    public GoalStore GoalStore;

    public Goal ParentGoal;

    public EnemyController ParentController;

    public Entity Self => ParentController.Self;

    public Goal()
    {
        GoalStore = new GoalStore();
        GoalStore.Initialize();
    }

    public virtual void Create()
    {
    }

    public virtual bool Finished()
    {
        return true;
    }

    public virtual void TakeAction()
    {
    }

    public void Pop()
    {
        if (ParentController.Goals.Count > 0)
        {
            ParentController.Goals.Pop();
        }
    }

    public virtual void PushGoal(Goal goal)
    {
        goal.Push(ParentController);
    }

    public virtual void PushChildGoal(Goal child)
    {
        child.ParentGoal = this;
        child.Push(ParentController);
    }

    public void PushChildGoal(Goal child, Goal parent)
    {
        child.ParentGoal = parent;
        child.Push(ParentController);
    }

    public virtual void Push(EnemyController parent)
    {
        ParentController = parent;
        parent.Goals.Push(this);
        Create();
    }

    public virtual void Failed()
    {
    }

    public void FailToParent()
    {
        while (ParentController.Goals.Count > 0 && ParentController.Goals.Peek() != ParentGoal)
        {
            ParentController.Goals.Pop();
        }
        if (ParentController.Goals.Count > 0)
        {
            ParentController.Goals.Peek().Failed();
        }
    }
}
