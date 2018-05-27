using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTemplateLoader : MonoBehaviour {

    public const string Path = "\\Scripts\\World\\Items\\itemtemplates";
    private static List<string> _itemTemplateTypes;
    private static ItemTemplateContainer _ic;

    private void Start()
    {
        _ic = ItemTemplateContainer.Load(Path.TrimStart('\\', '/'));

        _itemTemplateTypes = new List<string>();

        foreach (var i in _ic.ItemTemplates)
        {
            _itemTemplateTypes.Add(i.Type);
        }

    }

    public static ItemTemplate GetEntityTemplate(string entityTemplateType)
    {
        if (_ic == null)
        {
//            var basePath = Environment.CurrentDirectory;
//
//            var fullPath = System.IO.Path.Combine(basePath, Path.TrimStart('\\', '/'));
//
//            fullPath = System.IO.Path.Combine(Path, fullPath);

            _ic = ItemTemplateContainer.Load(Path);

            _itemTemplateTypes = new List<string>();

            foreach (var i in _ic.ItemTemplates)
            {
                _itemTemplateTypes.Add(i.Type);
            }
        }
        var index = _ic.ItemTemplates.FindIndex(item => item.Type.Equals(entityTemplateType.ToLower()));
        var it = _ic.ItemTemplates[index];

        return it;

    }
}
