using UnityEngine;

public enum AoeType
{
    Blast
}

//todo enum for aoe size with corresponding radius

public class AOEStore
{
    public static IAreaOfEffect GetAOEByType(AoeType type)
    {
        switch (type)
        {
            case AoeType.Blast:
                return new BlastAOE(Vector2.zero, 2);
            default:
                Debug.Log($"AoeType {type} not found. Using BlastAOE as default.");
                return new BlastAOE();
        }
    }

    public static IAreaOfEffect GetRandomAOE()
    {
        var type = GlobalHelper.GetRandomEnumValue<AoeType>();

        return GetAOEByType(type);
    }
}
