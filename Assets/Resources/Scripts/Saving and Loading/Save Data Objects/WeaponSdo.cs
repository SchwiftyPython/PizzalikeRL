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
            ItemType = weapon.ItemType,
            ItemCategory = weapon.ItemCategory,
            ItemName = weapon.ItemName,
            Id = weapon.Id,
            ItemDice = weapon.ItemDice,
            AOEType = weapon.AOEType
        };
    }

    public static Weapon ConvertToWeapon(WeaponSdo sdo)
    {
        var weapon = new Weapon();
        weapon.Type = sdo.Type;
        weapon.Range = sdo.Range;
        weapon.Rarity = sdo.Rarity;
        weapon.EquipmentSlotType = sdo.EquipmentSlotType;
        weapon.ItemType = sdo.ItemType;
        weapon.ItemCategory = sdo.ItemCategory;
        weapon.Id = sdo.Id;
        weapon.ItemDice = sdo.ItemDice;
        weapon.ItemName = sdo.ItemName;

        if (sdo.AOEType != null)
        {
            weapon.AOE = AOEStore.GetAOEByType((AoeType) sdo.AOEType);
        }

        return weapon;
    }
}
