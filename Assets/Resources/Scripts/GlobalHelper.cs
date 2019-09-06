using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalHelper : MonoBehaviour
{
    public const string DoubleSpace = "\n\n";
    public const string SingleSpace = "\n";

    public const string ToppingDroppedEventName = "ToppingDropped";
    public const string ToppingNotDroppedEventName = "ToppingNotDropped";
    public const string DeliveredEventName = "Delivered";
    public const string NewActiveWindowEventName = "NewActiveWindow";
    public const string InteractEventName = "Interact";
    public const string InspectEntityEventName = "InspectEntity";

    private static readonly Dictionary<GoalDirection, Vector2> DirectionVectorDictionary = new Dictionary<GoalDirection, Vector2>
    {
        {GoalDirection.North, new Vector2(1, 0)},
        {GoalDirection.NorthEast, new Vector2(1, 1)},
        {GoalDirection.East, new Vector2(0, 1)},
        {GoalDirection.SouthEast, new Vector2(-1, 1)},
        {GoalDirection.South, new Vector2(-1, 0)},
        {GoalDirection.SouthWest, new Vector2(-1, -1)},
        {GoalDirection.West, new Vector2(0, -1)},
        {GoalDirection.NorthWest, new Vector2(1, -1)}
    };

    public static void DestroyAllChildren(GameObject parent)
    {
        for (var i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }

    public static void DestroyAllChildren(RectTransform parent)
    {
        for (var i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }

    public static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static string CapitalizeAllWords(string s)
    {
        return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
    }

    //<Summary>
    // Returns a random enum value of type T
    //</Summary>
    public static T GetRandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(Random.Range(0, values.Length));
    }

    public static Vector2 GetVectorForDirection(GoalDirection direction)
    {
        return DirectionVectorDictionary[direction];
    }

    public static Dictionary<GoalDirection, Vector2> GetVectorDictionary()
    {
        return DirectionVectorDictionary;
    }
}
