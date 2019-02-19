using System;

[Serializable]
public class ToppingSdo
{
    public Toppings ToppingType;

    public static ToppingSdo ConvertToToppingSdo(Topping topping)
    {
        var sdo = new ToppingSdo {ToppingType = topping.Type};
        return sdo;
    }

    public static Topping ConvertToTopping(ToppingSdo sdo)
    {
        var topping = new Topping(sdo);
        return topping;
    }
}
