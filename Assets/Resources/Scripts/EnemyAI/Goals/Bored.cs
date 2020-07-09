using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bored : Goal
{
    //todo we can tweak these weights to get different personalities
    private readonly Dictionary<string, int> _goals = new Dictionary<string, int>
    {
        {"wander", 20},
        {"wait", 110},
        {"attack", 20}
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
        var goal = GetRandomGoal();

        if (goal == null)
        {
            FailToParent();
        }
        PushChildGoal(goal);
    }

    private Goal GetRandomGoal()
    {
        var selection = _goals.First().Key;

        var totalWeight = _goals.Values.Sum();

        var roll = Random.Range(0, totalWeight);

        foreach (var goal in _goals.OrderByDescending(g => g.Value))
        {
            var weightedValue = goal.Value;

            if (roll >= weightedValue)
            {
                roll -= weightedValue;
            }
            else
            {
                selection = goal.Key;
                break;
            }
        }
        return GoalStore.GetGoal(selection);
    }
}
