using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSdo : ItemSdo
{
    public string Type;

    public int Range;

    public static WeaponSdo ConvertToWeaponSdo(Weapon weapon)
    {
        return new WeaponSdo
        {
            Type = weapon.Type,
            Range = weapon.Range,
            Rarity = weapon.Rarity,
            BodyPartCategory = weapon.BodyPartCategory,
            ItemType = weapon.ItemType,
            ItemCategory = weapon.ItemCategory,
            Id = weapon.Id,
            ItemDice = weapon.ItemDice
        };
    }
}
