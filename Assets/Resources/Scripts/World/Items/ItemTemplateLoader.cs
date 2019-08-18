using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemTemplateLoader : MonoBehaviour {

    public const string Path = "\\Scripts\\World\\Items\\itemtemplates";
    private static List<string> _itemTemplateTypes;
    private static ItemTemplateContainer _ic;

    public static Dictionary<ItemPrefabKeys, ItemTemplate> ItemTemplatesDictionary;

    private void Awake()
    {
        LoadTemplatesFromFile();
        PopulateItemTemplatesDictionary();
    }

    public static ItemTemplate GetItemTemplate(string itemTemplateType)
    {
        if (_ic == null)
        {
            LoadTemplatesFromFile();
        }
        if (_ic == null)
        {
            return new ItemTemplate();
        }

        Enum.TryParse(itemTemplateType, true, out ItemPrefabKeys itemKey);

        var it = ItemTemplatesDictionary[itemKey];
        
        return it;
    }

    public static List<string> GetItemTemplateTypes()
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

        foreach (var i in _ic.ItemTemplatesList)
        {
            _itemTemplateTypes.Add(i.Type);
        }
    }

    private void PopulateItemTemplatesDictionary()
    {
        ItemTemplatesDictionary = new Dictionary<ItemPrefabKeys, ItemTemplate>();

        foreach (var template in _ic.ItemTemplatesList)
        {
            Enum.TryParse(template.Type, true, out ItemPrefabKeys itemKey);

            if (ItemTemplatesDictionary.ContainsKey(itemKey))
            {
                continue;
            }

            ItemTemplatesDictionary.Add(itemKey, template);
        }
    }
}
