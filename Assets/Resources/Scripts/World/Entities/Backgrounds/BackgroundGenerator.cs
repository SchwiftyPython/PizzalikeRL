using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    private const string StartSymbol = "#origin#";

    public static TextAsset ChildhoodFile;
    public static TextAsset ParentStatusFile;
    public static TextAsset LifeEventsFile;

    public static List<string> GenerateBackground()
    {
       return new List<string> {GenerateChildhood(), GenerateParentStatus()};
    }

    public static string GenerateLifeEvent()
    {
        return GenerateText(LifeEventsFile);
    }

    private static string GenerateChildhood()
    {
        return GenerateText(ChildhoodFile);
    }

    private static string GenerateParentStatus()
    {
        return GenerateText(ParentStatusFile);
    }

    private static string GenerateText(TextAsset file)
    {
        try
        {
            var grammar = new TraceryNet.Grammar(new FileInfo(file.text));
            return grammar.Flatten(StartSymbol);
        }
        catch (Exception e)
        {
            Debug.Log("Error opening " + file + "! " + e);
            throw;
        }
    }
}
