using System;

[Serializable]
public class ToppingSdo
{
    public Toppings ToppingType;

    public static ToppingSdo ConvertToToppingSdo(Topping topping)
    {
        if (topping == null)
        {
            return null;
        }

        var sdo = new ToppingSdo {ToppingType = topping.Type};
        return sdo;
    }

    public static Topping ConvertToTopping(ToppingSdo sdo)
    {
        if (sdo == null)
        {
            return null;
        }

        var topping = new Topping(sdo);
        return topping;
    }
}
