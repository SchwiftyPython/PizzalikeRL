using System;
using UnityEngine;

public class Topping
{
    public Toppings Type { get; }

    public GameObject WorldSpritePrefab { get; private set; }
    public GameObject InventorySpritePrefab { get; }

    public GameObject WorldSprite { get; set; }
    public GameObject InventorySprite { get; set; }

    public Topping(string topping)
    {
       Type = GetToppingType(topping);
       GetToppingSprites();
    }

    public Topping(ToppingSdo sdo)
    {
        Type = sdo.ToppingType;
        GetToppingSprites();
    }

    public Topping(Topping topping)
    {
        Type = topping.Type;
        WorldSpritePrefab = topping.WorldSpritePrefab;
        InventorySpritePrefab = topping.InventorySpritePrefab;
    }

    private static Toppings GetToppingType(string topping)
    {
        var enumTopping = (Toppings)Enum.Parse(typeof(Toppings), topping, true);
        return  enumTopping;
    }

    private void GetToppingSprites()
    {
        WorldSpritePrefab = WorldData.Instance.GetToppingWorldViewSpriteByType(Type);
        //todo get inventory view
    }

    //todo should be able to script topping picked up event here
}
