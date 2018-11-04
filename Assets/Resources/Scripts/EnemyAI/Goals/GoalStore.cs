using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GoalStore
{
    private readonly Dictionary<string, Goal> _allGoals = new Dictionary<string, Goal>
    {
        {"bored", new Bored() },
        {"wander", new Wander() }
    };

    private const string FilePath = "\\Assets\\Resources\\Scripts\\EnemyAI\\Goals";
    private const string GoalsFile = "goals.csv";

    public List<string> GoalKeys { get; private set; }

    public void Initialize()
    {
        var basePath = Environment.CurrentDirectory;

        var fullPath = Path.Combine(basePath, FilePath.TrimStart('\\', '/'), GoalsFile);

        GoalKeys = GetGoalsFromFile(fullPath);
    }

    public Goal GetGoal(string goal)
    {
        if (!_allGoals.ContainsKey(goal))
        {
            return null;
        }

        try
        {
            return _allGoals[goal];
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(
                $"Error retrieving goal: {goal}\n {e.Message}");
            throw;
        }
    }

    private static List<string> GetGoalsFromFile(string file)
    {
        var fullPath = Path.Combine(FilePath, file);
        var goals = new List<string>();
        try
        {
            using (var reader = new StreamReader(fullPath))
            {
                string line;
                while (null != (line = reader.ReadLine()?.Trim()))
                {
                    goals.AddRange(line.Split(','));
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error processing file: " + fullPath + " " + e.Message);
        }
        return goals;
    }
}
