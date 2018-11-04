using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bored : Goal
{
    private List<string> _goals = new List<string>
    {
        //Wander 
        //WanderRandomly
        //Travel
        //Wait
        //MoveTo
        //Dormant
    };

    public override bool Finished()
    {
        return false;
    }

    public override void TakeAction()
    {
        //if(FireEvent( "IAmBored")) return; todo when something can sign up for IAmBored AIEvent
        //if(StartKillingSomething()) return;
        IdleActivity();
    }

    private void IdleActivity()
    {
        var index = Random.Range(0, _goals.Count);

        var goalKey = _goals[index];

        var goal = GoalStore.GetGoal(goalKey);

        if (goal == null)
        {
            FailToParent();
        }
        goal?.Push(ParentController);
    }
}
