using UnityEngine;

public class BodyPartLoader : MonoBehaviour
{
    public const string Path = "bodyparts";
    public static string[] BodyPartNames;
    public static string[] BodyPartTypes;
    private static BodyPartContainer _bc;

    public static void LoadBodyParts()
    {
        _bc = BodyPartContainer.Load(Path);

        BodyPartNames = new string[_bc.BodyParts.Count];
        BodyPartTypes = new string[_bc.BodyParts.Count];

        var index = 0;

        foreach (var bp in _bc.BodyParts)
        {
            BodyPartNames[index] = bp.Name;
            BodyPartTypes[index] = bp.Type;
            bp.CurrentHp = bp.MaxHp;
            index++;
        }
    }

    public static string[] GetBodyPartNames()
    {
        return BodyPartNames;
    }

    public static string[] GetBodyPartTypes()
    {
        return BodyPartTypes;
    }

    public static string GetBodyPartNameAt(int index)
    {
        return BodyPartNames[index];
    }

    public static int GetBcLength()
    {
        return BodyPartNames.Length;
    }

    public static BodyPart GetBodyPart(string bodyPartName)
    {
        var index = _bc.BodyParts.FindIndex(item => item.Name.Equals(bodyPartName.ToLower()));
        return _bc.BodyParts[index];
    }

}