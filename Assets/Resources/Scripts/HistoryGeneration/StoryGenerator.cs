using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StoryGenerator : MonoBehaviour
{
    private const string StartSymbol = "#origin#";

    private const string StoryPath = "\\Assets\\Resources\\Scripts\\HistoryGeneration\\Story Files";

    private readonly Dictionary<string, Action<SituationContainer>> _stories = new Dictionary<string, Action<SituationContainer>>
    {
        {"faction leader assassination", FactionLeaderAssassination }
    };

    public static GameObject StoryBoxHolder;

    public static StoryGenerator Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        StoryBoxHolder = GameObject.Find("StoryBoxHolder");
    }

    public void Write(string storyTitle, SituationContainer details)
    {
        _stories[storyTitle].Invoke(details);
    }

    private static void DisplayText(string output)
    {
        StoryBoxHolder.transform.GetChild(0).GetComponent<Text>().text = output;
    }

    private static string GenerateText(string file)
    {
        var basePath = Environment.CurrentDirectory;

        var fullPath = Path.Combine(basePath, StoryPath.TrimStart('\\', '/'), file);

        fullPath = Path.Combine(StoryPath, fullPath);

        try
        {
            var grammar = new TraceryNet.Grammar(new FileInfo(fullPath));
            return grammar.Flatten(StartSymbol);
        }
        catch (Exception e)
        {
            Debug.Log("Error opening " + file + "! " + e);
            throw;
        }
    }

    private static void FactionLeaderAssassination(SituationContainer details)
    {
        const string file = "faction_leader_assassination.json";
        const string oldFactionLeader = "oldfactionleader";
        const string newFactionLeader = "newfactionleader";

        var story = GenerateText(file);

        story = story.Replace(oldFactionLeader, details.NamedCharacters[0].Fluff.Name);

        story = story.Replace(newFactionLeader, details.NamedCharacters[1].Fluff.Name);

        DisplayText(story);
    }
}
