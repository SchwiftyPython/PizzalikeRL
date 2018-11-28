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

        BodyPartNames = new string[_bc.BodyPartTemplates.Count];
        BodyPartTypes = new string[_bc.BodyPartTemplates.Count];

        var index = 0;

        foreach (var bp in _bc.BodyPartTemplates)
        {
            BodyPartNames[index] = bp.Name;
            BodyPartTypes[index] = bp.Type;
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

    public static BodyPartTemplate GetBodyPartTemplate(string bodyPartName)
    {
        var index = _bc.BodyPartTemplates.FindIndex(item => item.Name.Equals(bodyPartName.ToLower()));
        return _bc.BodyPartTemplates[index];
    }

}