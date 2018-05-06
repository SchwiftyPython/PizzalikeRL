using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Pizza
{
    #region Enums

    public enum Size
    {
        Personal,
        Medium,
        Large
    }

    public enum Crust
    {
        Pan,
        Stuffed,
        Thin
    }

    public enum Sauce
    {
        Marinara,
        White,
        Barbeque,
        HotSauce
    }

    public enum Toppings
    {
        Cheese,
        Pepperoni,
        Sausage,
        Meatball,
        Ham,
        Bacon,
        Chicken,
        Beef,
        Pork,
        Mushrooms,
        Spinach,
        Onion,
        Olives,
        Pineapple,
        Jalapeno
    }

    #endregion Enums

    public Size PizzaSize { get; }
    public Crust PizzaCrust { get; }
    public Sauce PizzaSauce { get; }
    public readonly List<Toppings> PizzaToppings;

    public Pizza(int numToppings = 1)
    {
        PizzaSize = GetRandomPizzaComponent<Size>();
        PizzaCrust = GetRandomPizzaComponent<Crust>();
        PizzaSauce = GetRandomPizzaComponent<Sauce>();
        PizzaToppings = new List<Toppings>();

        for (var i = 0; i < numToppings; i++)
        {
            var topping = GetRandomPizzaComponent<Toppings>();

            if (!PizzaToppings.Contains(topping))
            {
                PizzaToppings.Add(topping);
            }
        }
    }

    private static T GetRandomPizzaComponent<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(Random.Range(0, values.Length));
    }
}
