using System;
using System.Collections.Generic;

[Serializable]
public class ItemSdo
{
    public ItemRarity Rarity;

    public EquipmentSlotType EquipmentSlotType;

    public List<Entity.EquipmentSlot> EquipmentSlots;

    public string ItemType;

    public string ItemCategory;

    public string ItemName;

    public Guid Id;

    public Dice ItemDice;

    public List<string> Properties;

    public static ItemSdo ConvertToItemSdo(Item item)
    {
        return new ItemSdo
        {
            Rarity = item.Rarity,
            EquipmentSlotType = item.EquipmentSlotType,
            EquipmentSlots = item.EquipmentSlots,
            ItemType = item.ItemType,
            ItemCategory = item.ItemCategory,
            Id = item.Id,
            ItemDice = item.ItemDice,
            ItemName = item.ItemName,
            Properties = item.Properties
        };
    }

    public static Item ConvertToItem(ItemSdo sdo)
    {
        var item = new Item
        {
            Rarity = sdo.Rarity,
            EquipmentSlotType = sdo.EquipmentSlotType,
            EquipmentSlots = sdo.EquipmentSlots,
            ItemType = sdo.ItemType,
            ItemCategory = sdo.ItemCategory,
            Id = sdo.Id,
            ItemDice = sdo.ItemDice,
            WorldPrefab = ItemStore.Instance.GetWorldPrefabForItemByType(sdo.ItemType),
            ItemName = sdo.ItemName,
            Properties = sdo.Properties
    };
        return item;
    }
}
