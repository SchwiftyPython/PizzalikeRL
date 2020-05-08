using System;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public GameObject WorldPrefab;
    public GameObject WorldSprite;

    public GameObject InventoryPrefab;
    public GameObject InventorySprite;

    public ItemRarity Rarity;

    public Entity.EquipmentSlot EquipmentSlotType;
    public string ItemType;
    public string ItemCategory;
    public bool MultiSlot;

    public Guid Id;

    public Dice ItemDice;

    public List<string> Properties;

    public string ItemName;

    public Item(){ }

    public Item(ItemTemplate template, ItemRarity rarity)
    {
        ItemType = template.Type;
        ItemCategory = template.Category;
        EquipmentSlotType = template.EquipmentSlotType;
        Rarity = rarity;
        Properties = template.Properties;
        Id = Guid.NewGuid();

        ItemName = GlobalHelper.SplitStringByCapitalLetters(template.Type);

        //May end up getting prefab based on item characteristics and/or modifying appearance at runtime.
        WorldPrefab = ItemStore.Instance.GetWorldPrefabForItemByType(ItemType);

        //todo send new item event
        WorldData.Instance.Items.Add(Id, this);
    }
}
