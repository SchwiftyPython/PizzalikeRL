using UnityEngine;

public class BodyPartLoader : MonoBehaviour {

    public const string Path = "bodyparts";
    public static string[] BodyPartNames;
    public static string[] BodyPartTypes;
    private static BodyPartContainer _bc;

    public static void LoadBodyParts() {
        _bc = BodyPartContainer.Load(Path);

        BodyPartNames = new string[_bc.BodyParts.Count];
        BodyPartTypes = new string[_bc.BodyParts.Count];

        var index = 0;

        foreach (var bp in _bc.BodyParts)
        {
            BodyPartNames[index] = bp.Name;
            BodyPartTypes[index] = bp.Type;
            index++;
        }
    }

    public static string[] GetBodyPartNames() {
        return BodyPartNames;
    }

    public static string[] GetBodyPartTypes()
    {
        return BodyPartTypes;
    }

    public static string  GetBodyPartNameAt(int index) {
        return BodyPartNames[index];
    }

    public static int GetBcLength() {
        return BodyPartNames.Length;
    }

    public static BodyPart GetBodyPart(string bodyPartName) {
        var index = _bc.BodyParts.FindIndex(item => item.Name.Equals(bodyPartName.ToLower()));
        var bp = _bc.BodyParts[index];

        /*
        Debug.Log("Body Part Name: " + bp.name);
        Debug.Log("Body Part Type: " + bp.Type);
        Debug.Log("Body Part Max Hp: " + bp.maxHP);
        Debug.Log("Body Part Can Equip Weapon: " + bp.canEquipWeapon);
        Debug.Log("Body Part Can Equip Armor: " + bp.canEquipArmor);
        Debug.Log("Body Part Needs Part: " + bp.needsPart);
        Debug.Log("Body Part Attack Verb: " + bp.attackVerb);
        Debug.Log("Body Part Coverage: " + bp.coverage);
        */

        return bp;
    }

}