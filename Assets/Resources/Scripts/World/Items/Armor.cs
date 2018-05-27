using System;
using System.Collections.Generic;

public class Armor : Item
{
    //todo might make value a struct once we define more enchancements and attributes.
    private readonly Dictionary<ItemRarity, Tuple<int, int>> _armorDefenseValues = new Dictionary<ItemRarity, Tuple<int, int>>
    {
        {ItemRarity.Common, new Tuple<int, int>(1, 3) },
        {ItemRarity.Uncommon, new Tuple<int, int>(1, 6) },
        {ItemRarity.Rare, new Tuple<int, int>(2, 6) },
        {ItemRarity.Legendary, new Tuple<int, int>(3, 6) }
    };

    public string Type;
    public int Defense;

    public Armor(ItemTemplate template, ItemRarity rarity) : base(template, rarity)
    {
        Type = template.Type;
        GenDefense();
    }

    private void GenDefense()
    {
        var diceInfo = _armorDefenseValues[Rarity];
        Defense = DiceRoller.Instance.RollDice(diceInfo.Item1, diceInfo.Item2);
    }
}
