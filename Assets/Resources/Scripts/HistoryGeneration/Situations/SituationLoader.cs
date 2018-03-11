using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SituationLoader : MonoBehaviour
{
    public const string Path = "situations";
    private static List<List<string>> _situationTypes;
    private static SituationContainer _sc;
    
    public static void Initialize ()
    {
        _sc = SituationContainer.Load(Path);

        _situationTypes = new List<List<string>>();

        foreach (var s in _sc.Situations)
        {
            _situationTypes.Add(s.Types);
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

    public static List<Situation> GetSituationsOfType(string situationType)
    {
        return (from s in _sc.Situations
                where s.Types.Contains(situationType.ToLower())
                select s).ToList();
    }
}
