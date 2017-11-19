using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartLoader : MonoBehaviour {

    public const string Path = "bodyparts";
    public static string[] _bodyPartNames;
    private static BodyPartContainer _bc;

    public static void LoadBodyParts() {
        _bc = BodyPartContainer.Load(Path);

        _bodyPartNames = new string[_bc.bodyParts.Count];

        var index = 0;

        foreach (var bp in _bc.bodyParts) {
            _bodyPartNames[index] = bp.name;
            
            //GetBodyPart(bp.name); //testing

            index++;
        }
    }

    public static string[] GetBodyPartNames() {
        return _bodyPartNames;
    }

    public static string  GetBodyPartNameAt(int index) {
        return _bodyPartNames[index];
    }

    public static int GetBcLength() {
        return _bodyPartNames.Length;
    }

    public static BodyPart GetBodyPart(string bodyPartName) {
        var index = _bc.bodyParts.FindIndex(item => item.name.Equals(bodyPartName.ToLower()));
        var bp = _bc.bodyParts[index];

        /*
        Debug.Log("Body Part Name: " + bp.name);
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