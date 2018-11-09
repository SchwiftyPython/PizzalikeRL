using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log(Self + " is bored.");
            new Bored().Push(this);
            Goals.Peek().TakeAction();
        }

        if (Goals.Count > 0)
        {
            Goals.Peek().TakeAction();
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

    public IEnumerator MakeDecision()
    {
        var enemy = GameManager.Instance.CurrentArea.TurnOrder.Peek();
        FindPathToTarget(enemy, GameManager.Instance.Player.CurrentPosition); //move toward player test
        yield return new WaitForSeconds(0.1f);
        //Debug.Log(Path);
        //Debug.Log("Present Entity: " + GameManager.Instance.CurrentArea.GetTileAt(Path.vectorPath[1]).GetPresentEntity());
        if (GameManager.Instance.CurrentArea.GetTileAt(Path.vectorPath[1]).GetPresentEntity() == null)
        {
            var nextTilePosition =
                new Vector2(Path.vectorPath[1].x, Path.vectorPath[1].y); 
            enemy.AreaMove(nextTilePosition);
            TurnStarted = false;
            ActionTaken = true;
        }
        else
        {
            TurnStarted = false;
            ActionTaken = true;
        }
    }
}
