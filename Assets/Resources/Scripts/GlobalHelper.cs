using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalHelper : MonoBehaviour
{
    public const string DoubleSpace = "\n\n";
    public const string SingleSpace = "\n";

    #region EventNames

    public const string ToppingDroppedEventName = "ToppingDropped";
    public const string ToppingNotDroppedEventName = "ToppingNotDropped";
    public const string DeliveredEventName = "Delivered";
    public const string NewActiveWindowEventName = "NewActiveWindow";
    public const string InteractEventName = "Interact";
    public const string InspectEntityEventName = "InspectEntity";
    public const string LoadAbilityBarEventName = "LoadAbilityBar";
    public const string EndTurnEventName = "EndTurn";
    public const string UnderAttackEventName = "UnderAttack";
    public const string EffectDoneEventName = "EffectDone";
    public const string DirectionalAbilityEventName = "DirectionalAbility";
    public const string AbilityTileSelectedEventName = "AbilityTileSelected";
    public const string EntityEnteredTileEventName = "EntityEnteredTile";
    public const string ActionTakenEventName = "ActionTaken";
    public const string ItemEquippedEventName = "ItemEquipped";
    public const string ItemUnequippedEventName = "ItemUnequipped";
    public const string AbilityButtonActionPopupEventName = "AbilityButtonActionPopup";
    public const string AbilitySelectPopupEventName = "AbilitySelectPopup";
    public const string AbilitySelectedEventName = "AbilitySelected";
    public const string SendMessageToConsoleEventName = "SendMessageToConsole";
    public const string InspectItemEventName = "InspectItemEvent";
    public const string ItemSelectedEventName = "ItemSelected";
    public const string DroppedItemSelectedEventName = "DroppedItemSelected";
    public const string DroppedItemPopupEventName = "DroppedItemPopup";
    public const string GetItemEventName = "GetItem";
    public const string TakeAllEventName = "TakeAll";
    public const string EquipmentSlotSelectedEventName = "EquipmentSlotSelected";
    public const string KillPlayerEventName = "KillPlayer";
    public const string PlayerEnterAreaEventName = "PlayerEnterArea";
    public const string PlayerEnterWorldMapEventName = "PlayerEnterWorldMap";

    #endregion EventNames

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

    public enum RangedAttackType
    {
        Missile,
        Thrown
    }

    public enum AttributeCheckWinner
    {
        Attacker,
        Defender
    }

    public enum AttributeCheck
    {
        Strength,
        Agility,
        Constitution,
        Intelligence
    }

    public static AttributeCheckWinner AttackerDefenderAttributeCheck(Entity attacker, Entity defender,
        AttributeCheck attribute)
    {
        var intCheckDice = new Dice(1, 20);

        var attackerTotal = DiceRoller.Instance.RollDice(intCheckDice);

        var defenderTotal = DiceRoller.Instance.RollDice(intCheckDice);

        switch (attribute)
        {
            case AttributeCheck.Strength:
                attackerTotal = attacker.Strength + attackerTotal;
                defenderTotal = defender.Strength + defenderTotal;
                break;
            case AttributeCheck.Agility:
                attackerTotal = attacker.Agility + attackerTotal;
                defenderTotal = defender.Agility + defenderTotal;
                break;
            case AttributeCheck.Constitution:
                attackerTotal = attacker.Constitution + attackerTotal;
                defenderTotal = defender.Constitution + defenderTotal;
                break;
            case AttributeCheck.Intelligence:
                attackerTotal = attacker.Intelligence + attackerTotal;
                defenderTotal = defender.Intelligence + defenderTotal;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null);
        }

        return attackerTotal > defenderTotal ? AttributeCheckWinner.Attacker : AttributeCheckWinner.Defender;
    }

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

    public static void DestroyObject(GameObject go)
    {
        Destroy(go);
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

    public static string SplitStringByCapitalLetters(string s)
    {
        var splitString = "";

        foreach (var letter in s)
        {
            if (char.IsUpper(letter) && splitString.Length > 0)
            {
                splitString += " " + letter;
            }
            else
            {
                splitString += letter;
            }
        }

        return splitString;
    }

    //<Summary>
    // Returns a random enum value of type T
    //</Summary>
    public static T GetRandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(Random.Range(0, values.Length));
    }

    public static string GetEnumDescription(Enum value)
    {
        return
            value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description
            ?? value.ToString();
    }

    public static T GetEnumValueFromDescription<T>(string description)
    {
        var type = typeof(T);

        if (!type.IsEnum)
        {
            throw new InvalidOperationException();
        }

        foreach (var field in type.GetFields())
        {
            if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            else
            {
                if (field.Name == description)
                {
                    return (T)field.GetValue(null);
                }
            }
        }
        throw new ArgumentException($@"Not found: {description}", nameof(description));
    }

    public static Vector2 GetVectorForDirection(GoalDirection direction)
    {
        return DirectionVectorDictionary[direction];
    }

    public static Dictionary<GoalDirection, Vector2> GetVectorDictionary()
    {
        return DirectionVectorDictionary;
    }

    public static Dice GetDiceFromString(string dice)
    {
        var splitDice = dice.Split('d');

        var numDice = int.Parse(splitDice[0]);
        var numSides = int.Parse(splitDice[1]);

        return new Dice(numDice, numSides);
    }

    public static T[] Convert2dArrayTo1dArray<T>(T[,] array2d)
    {
        var height = array2d.GetLength(0);
        var width = array2d.GetLength(1);

        var index = 0;
        var single = new T[height * width];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                single[index] = array2d[row, column];
                index++;
            }
        }
        return single;
    }

    public static T[,] Convert1dArrayTo2dArray<T>(int height, int width, T[] array1d)
    {
        var index = 0;
        var multi = new T[height, width];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                multi[row, column] = array1d[index];
                index++;
            }
        }
        return multi;
    }

    public static T[,] Convert1dArrayTo2dArraySquare<T>(T[] array1d)
    {
        var index = 0;
        var sqrt = (int)Math.Sqrt(array1d.Length);
        var multi = new T[sqrt, sqrt];
        for (var row = 0; row < sqrt; row++)
        {
            for (var column = 0; column < sqrt; column++)
            {
                multi[row, column] = array1d[index];
                index++;
            }
        }
        return multi;
    }

    public static T NextEnum<T>(T src) where T : struct
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
        }

        var arr = (T[])Enum.GetValues(src.GetType());
        var j = Array.IndexOf(arr, src) + 1;
        return arr.Length == j ? arr[0] : arr[j];
    }
}
