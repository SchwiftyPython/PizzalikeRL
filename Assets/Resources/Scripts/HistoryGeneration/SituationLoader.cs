using System.Collections.Generic;
using UnityEngine;

public class SituationLoader : MonoBehaviour
{
    public const string Path = "situations";
    private static List<List<string>> _situationTypes;
    private static SituationContainer _sc;
    
    private void Start ()
    {
        _sc = SituationContainer.Load(Path);

        _situationTypes = new List<List<string>>();

        var index = 0;

        foreach (var s in _sc.Situations)
        {
            _situationTypes[index] = s.Types;
            index++;
        }
    }

    public static List<List<string>> GetSituationTypes()
    {
        return _situationTypes;
    }

    public static List<string> GetSituationTypeAt(int index)
    {
        return _situationTypes[index];
    }

    public static int GetScLength()
    {
        return _situationTypes.Count;
    }

    public static Situation GetSituation(string entityTemplateType)
    {
        var index = _sc.Situations.FindIndex(item => item.Types.Contains(entityTemplateType.ToLower()));
        var et = _sc.Situations[index];

        return et;

    }
}
