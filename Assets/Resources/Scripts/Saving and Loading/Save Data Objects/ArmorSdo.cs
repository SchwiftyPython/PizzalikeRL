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
            BodyPartCategory = armor.BodyPartCategory,
            ItemType = armor.ItemType,
            ItemCategory = armor.ItemCategory,
            Id = armor.Id,
            ItemDice = armor.ItemDice
        };
    }
}
