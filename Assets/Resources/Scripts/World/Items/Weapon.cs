using System.Collections.Generic;

public class Weapon : Item
{
    //todo might make value a struct once we define more enchancements and attributes.
    private readonly Dictionary<ItemRarity, Dice> _weaponBaseDamageValues = new Dictionary<ItemRarity, Dice>
    {
        { ItemRarity.Common, new Dice(1, 3) },
        { ItemRarity.Uncommon, new Dice(1, 6) },
        { ItemRarity.Rare, new Dice(2, 6) },
        { ItemRarity.Legendary, new Dice(3, 6) }
    };

    private readonly Dictionary<string, int> _weaponTypeBaseRanges = new Dictionary<string, int>
    {
        { "sword", 1 },
        { "bow", 20 }
    };

    public string Type;
    public int Range;

    public Weapon(ItemTemplate template, ItemRarity rarity) : base(template, rarity)
    {
        Type = template.Type;
        ItemDice = _weaponBaseDamageValues[rarity];
        Range = _weaponTypeBaseRanges[template.Type];
    }
}
