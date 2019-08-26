using System;

[Serializable]
public class ItemSdo
{
    public ItemRarity Rarity;

    public Entity.EquipmentSlot EquipmentSlotType;

    public string ItemType;

    public string ItemCategory;

    public Guid Id;

    public Dice ItemDice;

    public static ItemSdo ConvertToItemSdo(Item item)
    {
        return new ItemSdo
        {
            Rarity = item.Rarity,
            EquipmentSlotType = item.EquipmentSlotType,
            ItemType = item.ItemType,
            ItemCategory = item.ItemCategory,
            Id = item.Id,
            ItemDice = item.ItemDice
        };
    }

    public static Item ConvertToItem(ItemSdo sdo)
    {
        var item = new Item
        {
            Rarity = sdo.Rarity,
            EquipmentSlotType = sdo.EquipmentSlotType,
            ItemType = sdo.ItemType,
            ItemCategory = sdo.ItemCategory,
            Id = sdo.Id,
            ItemDice = sdo.ItemDice
        };
        return item;
    }
}
