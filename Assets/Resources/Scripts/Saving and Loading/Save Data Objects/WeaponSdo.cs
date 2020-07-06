using System;

[Serializable]
public class WeaponSdo : ItemSdo
{
    public string Type;

    public int Range;

    public AoeType? AOEType;

    public static WeaponSdo ConvertToWeaponSdo(Weapon weapon)
    {
        return new WeaponSdo
        {
            Type = weapon.Type,
            Range = weapon.Range,
            Rarity = weapon.Rarity,
            EquipmentSlotType = weapon.EquipmentSlotType,
            EquipmentSlots = weapon.EquipmentSlots,
            ItemType = weapon.ItemType,
            ItemCategory = weapon.ItemCategory,
            ItemName = weapon.ItemName,
            Id = weapon.Id,
            ItemDice = weapon.ItemDice,
            AOEType = weapon.AOEType,
            Properties = weapon.Properties
        };
    }

    public static Weapon ConvertToWeapon(WeaponSdo sdo)
    {
        var weapon = new Weapon();
        weapon.Type = sdo.Type;
        weapon.Range = sdo.Range;
        weapon.Rarity = sdo.Rarity;
        weapon.EquipmentSlotType = sdo.EquipmentSlotType;
        weapon.EquipmentSlots = sdo.EquipmentSlots;
        weapon.ItemType = sdo.ItemType;
        weapon.ItemCategory = sdo.ItemCategory;
        weapon.Id = sdo.Id;
        weapon.ItemDice = sdo.ItemDice;
        weapon.ItemName = sdo.ItemName;
        weapon.Properties = sdo.Properties;

        if (sdo.AOEType != null)
        {
            weapon.AOE = AOEStore.GetAOEByType((AoeType) sdo.AOEType);
        }

        return weapon;
    }
}
