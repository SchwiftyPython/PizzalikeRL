using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StoryGenerator : MonoBehaviour {
    private const string StartSymbol = "#origin#";

    public GameObject StoryBoxHolder;
    
    public void DisplayText(string output)
    {
        StoryBoxHolder.transform.GetChild(0).GetComponent<Text>().text = output;
    }

    public void GenerateText() {
        const string scifiFileName = "scifi.json";
        try
        {
            var grammar = new TraceryNet.Grammar(new FileInfo(scifiFileName));
            var output = grammar.Flatten(StartSymbol);
            DisplayText(output);
        }
        catch (Exception e)
        {
            Debug.Log("Error opening " + scifiFileName +"! " + e);
            throw;
        }
    }
}
