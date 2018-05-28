using UnityEngine;

public class EntityTemplateLoader : MonoBehaviour {

    public const string Path = "entitytemplates";
    private static string[] _entityTemplateTypes;
    private static EntityTemplatesContainer _ec;

    private void Start() {
        BodyPartLoader.LoadBodyParts();

        _ec = EntityTemplatesContainer.Load(Path);

        _entityTemplateTypes = new string[_ec.EntityTemplates.Count];

        var index = 0;

        foreach (var e in _ec.EntityTemplates) {
            _entityTemplateTypes[index] = e.Type;

            //GetEntityTemplate(e.type); //testing

            index++;
        }

    }

    public static string[] GetEntityTemplateTypes() {
        return _entityTemplateTypes;
    }

    public static string GetEntityTemplateTypeAt(int index) {
        return _entityTemplateTypes[index];
    }

    public static int GetEcLength() {
        return _entityTemplateTypes.Length;
    }

    public static EntityTemplate GetEntityTemplate(string entityTemplateType) {
        if (_ec == null)
        {
            BodyPartLoader.LoadBodyParts();

            _ec = EntityTemplatesContainer.Load(Path);

            _entityTemplateTypes = new string[_ec.EntityTemplates.Count];

            var i = 0;

            foreach (var e in _ec.EntityTemplates)
            {
                _entityTemplateTypes[i] = e.Type;

                i++;
            }
        }
        var index = _ec.EntityTemplates.FindIndex(item => item.Type.Equals(entityTemplateType.ToLower()));
        var et = _ec.EntityTemplates[index];

        /*
        Debug.Log("Entity Template Type: " + et.type);
        foreach (var p in et.parts) {
            Debug.Log("Part: " + p);
        }
        */
        
        return et;

    }
}
