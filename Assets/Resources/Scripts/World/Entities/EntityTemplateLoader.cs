﻿using UnityEngine;

namespace Resources.Scripts.World.Entities {

    public class EntityTemplateLoader : MonoBehaviour {

        public const string Path = "entitytemplates";

        private static string[] _entityTemplateTypes;
        private static EntityTemplatesContainer _ec;

        private void Start() {

            _ec = EntityTemplatesContainer.Load(Path);

            _entityTemplateTypes = new string[_ec.entityTemplates.Count];

            var index = 0;

            foreach (var e in _ec.entityTemplates) {
                _entityTemplateTypes[index] = e.type;

                //GetEntityTemplate(ec.type); //testing

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
            var index = _ec.entityTemplates.FindIndex(item => item.type.Equals(entityTemplateType.ToLower()));
            var et = _ec.entityTemplates[index];

            //Debug.Log("Entity Template Type: " + et.type);

            return et;

        }
    }
}
