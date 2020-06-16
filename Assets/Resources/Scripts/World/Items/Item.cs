using System;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentSlotType
{
    Head,
    Body,
    Hand,
    Missile,
    Thrown,
    Special,
    Consumable
}

public class Item
{
    public GameObject WorldPrefab;
    public GameObject WorldSprite;

    public GameObject InventoryPrefab;
    public GameObject InventorySprite;

    public ItemRarity Rarity;

    public EquipmentSlotType EquipmentSlotType;
    public List<Entity.EquipmentSlot> EquipmentSlots;
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
        EquipmentSlots = ItemStore.Instance.GetEquipmentSlotsForSlotType(EquipmentSlotType);
        Rarity = rarity;
        Properties = template.Properties;
        Id = Guid.NewGuid();

        ItemName = GlobalHelper.SplitStringByCapitalLetters(template.Type);

        //may end up getting prefab based on item characteristics and/or modifying appearance at runtime.
        WorldPrefab = ItemStore.Instance.GetWorldPrefabForItemByType(ItemType);

        //todo send new item event
        WorldData.Instance.Items.Add(Id, this);
    }
}
