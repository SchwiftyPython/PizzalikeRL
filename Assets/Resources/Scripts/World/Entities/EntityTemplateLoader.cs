using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTemplateLoader : MonoBehaviour {

    public const string path = "entitytemplates";
    private static string[] entityTemplateTypes;
    private static EntityTemplatesContainer ec;

    void Start() {

        ec = EntityTemplatesContainer.Load(path);

        entityTemplateTypes = new string[ec.entityTemplates.Count];

        int index = 0;

        foreach (EntityTemplate ec in ec.entityTemplates) {
            entityTemplateTypes[index] = ec.type;

            //GetEntityTemplate(ec.type); //testing

            index++;
        }

    }

    public static string[] GetEntityTemplateTypes() {
        return entityTemplateTypes;
    }

    public static string GetEntityTemplateTypeAt(int index) {
        return entityTemplateTypes[index];
    }

    public static int GetECLength() {
        return entityTemplateTypes.Length;
    }

    public static EntityTemplate GetEntityTemplate(string entityTemplateType) {
        int index = ec.entityTemplates.FindIndex(item => item.type.Equals(entityTemplateType.ToLower()));
        EntityTemplate et = ec.entityTemplates[index];

        Debug.Log("Entity Template Type: " + et.type);

        return et;

    }
}
