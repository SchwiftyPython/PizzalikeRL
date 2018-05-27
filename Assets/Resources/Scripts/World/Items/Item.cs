using UnityEngine;

public class Item
{
    protected GameObject Prefab;
    protected GameObject Sprite;

    protected string BodyPartCategory;

    protected ItemRarity Rarity;

    public Item()
    { }

    public Item(ItemTemplate template, ItemRarity rarity)
    {
		//todo get prefab if we go that route. May end up getting prefab based on item characteristics and/or modifying appearance at runtime.
        BodyPartCategory = template.BodyPartCategory;
        Rarity = rarity;
    }
}
