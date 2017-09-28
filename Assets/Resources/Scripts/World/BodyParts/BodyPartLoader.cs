using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartLoader : MonoBehaviour {

    public const string path = "bodyparts";
    private static string[] bodyPartNames;
    private static BodyPartContainer bc;

    void Start() {
        bc = BodyPartContainer.Load(path);

        bodyPartNames = new string[bc.bodyParts.Count];

        int index = 0;

        foreach (BodyPart bp in bc.bodyParts) {
            bodyPartNames[index] = bp.name;
            
            //GetBodyPart(bp.name); //testing

            index++;
        }
    }

    public static string[] GetBodyPartNames() {
        return bodyPartNames;
    }

    public static string  GetBodyPartNameAt(int index) {
        return bodyPartNames[index];
    }

    public static int GetBCLength() {
        return bodyPartNames.Length;
    }

    public static BodyPart GetBodyPart(string bodyPartName) {
        int index = bc.bodyParts.FindIndex(item => item.name.Equals(bodyPartName.ToLower()));
        BodyPart bp = bc.bodyParts[index];

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