using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
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

    #endregion Enums

    private readonly Dictionary<Toppings, int> _baseIngredientCount = new Dictionary<Toppings, int>
    {
        { Toppings.Bacon, 1 },
        { Toppings.BellPepper, 3 },
        { Toppings.Cheese, 3 },
        { Toppings.Jalapeno, 2 },
        { Toppings.Mushrooms, 2 },
        { Toppings.Olives, 1 },
        { Toppings.Onion, 1 },
        { Toppings.Pepperoni, 3 },
        { Toppings.Pineapple, 2 },
        { Toppings.Sausage, 2 },
        { Toppings.Tomato, 2 },
        { Toppings.Wheat, 1 }
    };

    private readonly Dictionary<Size, int> _ingredientSizeMultiplier = new Dictionary<Size, int>
    {
        { Size.Personal, 1 },
        { Size.Medium, 2 },
        { Size.Large, 3 }
    };

    public Size PizzaSize { get; }
    public Crust PizzaCrust { get; }
    public readonly Dictionary<Toppings, int> PizzaToppings;

    public Pizza(int numToppings = 1)
    {
        PizzaSize = GetRandomPizzaComponent<Size>();
        PizzaCrust = GetRandomPizzaComponent<Crust>(); //todo work this out and see if worth doing -- modifies cheese and wheat amounts
        PizzaToppings = new Dictionary<Toppings, int>();

        for (var i = 0; i < numToppings; i++)
        {
            var topping = GetRandomPizzaComponent<Toppings>();
            
            if (!PizzaToppings.ContainsKey(topping) && topping != Toppings.Wheat && topping != Toppings.Cheese)
            {
                PizzaToppings.Add(topping, GetNumIngredientsRequired(topping));
            }
        }

        // Tomato can be a topping, but is always required for sauce. 
        if (!PizzaToppings.ContainsKey(Toppings.Tomato))
        {
            PizzaToppings.Add(Toppings.Tomato, GetNumIngredientsRequired(Toppings.Tomato));
        }
        else
        {
            PizzaToppings[Toppings.Tomato] += GetNumIngredientsRequired(Toppings.Tomato);
        }

        PizzaToppings.Add(Toppings.Wheat, GetNumIngredientsRequired(Toppings.Wheat));

        PizzaToppings.Add(Toppings.Cheese, GetNumIngredientsRequired(Toppings.Cheese));
    }

    private int GetNumIngredientsRequired(Toppings topping)
    {
        return _baseIngredientCount[topping] * _ingredientSizeMultiplier[PizzaSize];
    }

    private static T GetRandomPizzaComponent<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(Random.Range(0, values.Length));
    }
}
