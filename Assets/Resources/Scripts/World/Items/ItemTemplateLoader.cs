using System.Collections.Generic;
using UnityEngine;

public class ItemTemplateLoader : MonoBehaviour {

    public const string Path = "\\Scripts\\World\\Items\\itemtemplates";
    private static List<string> _itemTemplateTypes;
    private static ItemTemplateContainer _ic;

    private void Awake()
    {
        LoadTemplatesFromFile();
    }

    public static ItemTemplate GetEntityTemplate(string entityTemplateType)
    {
        if (_ic == null)
        {
            LoadTemplatesFromFile();
        }
        if (_ic == null)
        {
            return new ItemTemplate();
        }

        var index = _ic.ItemTemplates.FindIndex(item => item.Type.Equals(entityTemplateType.ToLower()));
        var it = _ic.ItemTemplates[index];

        return it;
    }

    public static List<string> GetEntityTemplateTypes()
    {
        if (_itemTemplateTypes == null)
        {
            LoadTemplatesFromFile();
        }

        return _itemTemplateTypes != null ? new List<string>(_itemTemplateTypes) : null;
    }

    private static void LoadTemplatesFromFile()
    {
        _ic = ItemTemplateContainer.Load(Path.TrimStart('\\', '/'));

        _itemTemplateTypes = new List<string>();

        foreach (var i in _ic.ItemTemplates)
        {
            _itemTemplateTypes.Add(i.Type);
        }
    }
}
