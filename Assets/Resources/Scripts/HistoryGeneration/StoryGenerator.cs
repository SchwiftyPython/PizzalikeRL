using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StoryGenerator : MonoBehaviour
{
    public GameObject StoryBoxHolder;

//    private void Update()
//    {
//        DisplayText();
//    }
    
    public void DisplayText(string output)
    {
        StoryBoxHolder.transform.GetChild(0).GetComponent<Text>().text = output;
    }

    public void GenerateText()
    {
        const string scifiFileName = "scifi.json";
        try
        {
            var grammar = new TraceryNet.Grammar(new FileInfo(scifiFileName));
            var output = grammar.Flatten("#origin#");
            DisplayText(output);
        }
        catch (Exception e)
        {
            Debug.Log("Error opening " + scifiFileName +"! " + e);
            throw;
        }
    }
}
