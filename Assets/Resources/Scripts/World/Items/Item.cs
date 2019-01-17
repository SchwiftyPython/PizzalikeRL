using System;
using UnityEngine;

public class Item
{
    protected GameObject Prefab;
    protected GameObject Sprite;

    public ItemRarity Rarity;

    public string BodyPartCategory;
    public string ItemType;
    public string ItemCategory;

    public Guid Id;

    public Dice ItemDice;

    public Item(){ }

    public Item(ItemTemplate template, ItemRarity rarity)
    {
        //todo get prefab if we go that route. 
        //May end up getting prefab based on item characteristics and/or modifying appearance at runtime.
        ItemType = template.Type;
        ItemCategory = template.Category;
        BodyPartCategory = template.BodyPartCategory;
        Rarity = rarity;
        Id = Guid.NewGuid();
    }
}
