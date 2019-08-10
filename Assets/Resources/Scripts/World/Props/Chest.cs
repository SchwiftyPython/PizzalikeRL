using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chest : Prop
{
    private const int ItemCap = 5;

    private IDictionary<Guid, Item> _contents; 

    public Chest(GameObject prefab) : base(prefab, true)
    {
        _contents = new Dictionary<Guid, Item>();
        FillWithRandomItems();
    }

    public void AddItem(Item item)
    {
        if (_contents.ContainsKey(item.Id))
        {
            Debug.Log($"Chest contents already has this key! {item.ItemType}: {item.Id}");
            return;
        }
        _contents.Add(item.Id, item);
    }

    public void RemoveItem(Item item)
    {
        _contents.Remove(item.Id);
    }

    public IEnumerable<Item> GetContents()
    {
        return _contents.Values;
    }

    private void FillWithRandomItems()
    {
        var numItems = Random.Range(0, ItemCap);

        var items = ItemStore.Instance.GetRandomItems(numItems);

        if (items.Count < 1)
        {
            return;
        }

        for (var i = 0; i < numItems; i++)
        {
            AddItem(items[i]);
        }
    }
}
