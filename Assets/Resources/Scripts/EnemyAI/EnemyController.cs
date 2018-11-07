using System;
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

        if (Goals == null)
        {
            Goals = new Stack<Goal>();
        }

        if (Goals.Count == 0)
        {
            Debug.Log(Self + " is bored.");
            var bored = new Bored();
            bored.Push(this);
        }

        while (Goals.Peek().Finished())
        {
            Goals.Pop();
        }
        Goals.Peek().TakeAction();
        ActionTaken = true;
    }

    public void PushGoal(Goal goal)
    {
        goal.Push(this);
    }

    public bool IsMobile()
    {
        return Mobile;
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
