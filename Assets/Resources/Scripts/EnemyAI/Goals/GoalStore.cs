using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalStore
{
    private readonly Dictionary<string, Func<Goal>> _allGoals = new Dictionary<string, Func<Goal>>
    {
        {"bored", () => new Bored()},
        {"wander", () => new Wander()},
        {"wait", () => new Wait()},
        {"kill", ()=> new Attack()}
    };

    public List<string> GoalKeys { get; set; }

    public GoalStore()
    {
        GoalKeys = new List<string>();
        foreach (var goalKey in _allGoals.Keys)
        {
            GoalKeys.Add(goalKey);
        }
    }

    public Goal GetGoal(string goal)
    {
        if (!_allGoals.ContainsKey(goal))
        {
            return null;
        }

        try
        {
            return _allGoals[goal]();
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(
                $"Error retrieving goal: {goal}\n {e.Message}");
            throw;
        }
    }
}
