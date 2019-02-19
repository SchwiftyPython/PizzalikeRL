using System;
using UnityEngine;

public class Item
{
    public GameObject WorldPrefab;
    public GameObject WorldSprite;

    public GameObject InventoryPrefab;
    public GameObject InventorySprite;

    public ItemRarity Rarity;

    public string BodyPartCategory;
    public string ItemType;
    public string ItemCategory;

    public Guid Id;

    public Dice ItemDice;

    public Item(){ }

    public Item(ItemTemplate template, ItemRarity rarity)
    {
        ItemType = template.Type;
        ItemCategory = template.Category;
        BodyPartCategory = template.BodyPartCategory;
        Rarity = rarity;
        Id = Guid.NewGuid();

        //May end up getting prefab based on item characteristics and/or modifying appearance at runtime.
        WorldPrefab = ItemStore.Instance.GetWorldPrefabForItemByType(ItemType);

        //todo send new item event
        WorldData.Instance.Items.Add(Id, this);
    }
}
