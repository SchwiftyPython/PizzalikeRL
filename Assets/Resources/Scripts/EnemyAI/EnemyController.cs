using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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

    public void GetAngryAt(Entity target)
    {
        if (target.Faction != null)
        {
            Self.EntityReputation.ChangeReputationValue(target.Faction.Name, int.MinValue);
        }
        else
        {
            Self.EntityReputation.ChangeReputationValue(target.EntityType, int.MinValue);
        }

        if (Self.Faction != null)
        {
            target.EntityReputation.ChangeReputationValue(Self.Faction.Name, int.MinValue);
        }
        else
        {
            target.EntityReputation.ChangeReputationValue(Self.EntityType, int.MinValue);
        }

        Self.Goals = new Stack<Goal>();

        new Attack(target).Push(this);
    }

    public void ReactToAttacker(Entity attacker)
    {
        if (attacker == Self)
        {
            return;
        }

        const int fleeChance = 1;

        var roll = Random.Range(0, 100);

        if (roll <= fleeChance)
        {
            new Flee(attacker).Push(this);
        }
        else
        {
            if (Goals == null)
            {
                if (Self.Goals == null)
                {
                    Self.Goals = new Stack<Goal>();
                }
                Goals = Self.Goals;
            }

            if (Goals.Count > 0 && Goals.Peek().GetType() == typeof(Attack))
            {
                return;
            }

            GetAngryAt(attacker);
        }

        EventMediator.Instance.Broadcast("UnderAttack", Self, attacker);
    }
}
