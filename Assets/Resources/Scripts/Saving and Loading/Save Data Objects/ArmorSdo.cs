using System;

[Serializable]
public class ArmorSdo : ItemSdo
{
    public string Type;

    public int Defense;

    public static ArmorSdo ConvertToArmorSdo(Armor armor)
    {
        return new ArmorSdo
        {
            Type = armor.Type,
            Defense = armor.Defense,
            Rarity = armor.Rarity,
            EquipmentSlotType = armor.EquipmentSlotType,
            ItemType = armor.ItemType,
            ItemCategory = armor.ItemCategory,
            Id = armor.Id,
            ItemDice = armor.ItemDice,
            ItemName = armor.ItemName,
            Properties = armor.Properties
        };
    }

    public static Armor ConvertToArmor(ArmorSdo sdo)
    {
        var armor = new Armor();
        armor.Type = sdo.Type;
        armor.Defense = sdo.Defense;
        armor.Rarity = sdo.Rarity;
        armor.EquipmentSlotType = sdo.EquipmentSlotType;
        armor.ItemType = sdo.ItemType;
        armor.ItemCategory = sdo.ItemCategory;
        armor.Id = sdo.Id;
        armor.ItemDice = sdo.ItemDice;
        armor.ItemName = sdo.ItemName;
        armor.Properties = sdo.Properties;

        return armor;
    }
}
