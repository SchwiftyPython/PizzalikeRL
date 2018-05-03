using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Pizza
{
    #region Enums

    private enum Size
    {
        Personal,
        Medium,
        Large
    }

    private enum Crust
    {
        HandTossed,
        Stuffed,
        Thin
    }

    private enum Sauce
    {
        Marinara,
        White,
        Barbeque,
        HotSauce
    }

    private enum Toppings
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
        BellPepper,
        Pineapple,
        Jalapeno
    }

    #endregion Enums

    private Size _size;
    private Crust _crust;
    private Sauce _sauce;
    private readonly List<Toppings> _toppings;

    public Pizza(int numToppings = 1)
    {
        _size = GetRandomPizzaComponent<Size>();
        _crust = GetRandomPizzaComponent<Crust>();
        _sauce = GetRandomPizzaComponent<Sauce>();
        _toppings = new List<Toppings>();

        for (var i = 0; i < numToppings; i++)
        {
            var topping = GetRandomPizzaComponent<Toppings>();

            if (!_toppings.Contains(topping))
            {
                _toppings.Add(topping);
            }
        }
    }

    private static T GetRandomPizzaComponent<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(Random.Range(0, values.Length));
    }
}
