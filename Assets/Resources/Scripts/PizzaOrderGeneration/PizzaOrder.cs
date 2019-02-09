using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PizzaOrder
{
    public enum OrderDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public List<Pizza> Pizzas;
    public Entity Customer;
    
    public Cell CustomerLocation;

    public PizzaOrder() { }

    public PizzaOrder(OrderDifficulty difficulty = OrderDifficulty.Easy)
    {
        Pizzas = new List<Pizza>();
        Pizzas.AddRange(GenerateOrder(difficulty));

        ChooseCustomer();
        CustomerLocation = Customer.CurrentCell; //todo Use name of region cell is in when implemented
    }

    private static IEnumerable<Pizza> GenerateOrder(OrderDifficulty difficulty)
    {
        int maxToppings;
        switch (difficulty)
        {
            case OrderDifficulty.Easy:
                return new List<Pizza>{ new Pizza() };

            case OrderDifficulty.Medium:
                maxToppings = 2;
                return new List<Pizza>
                {
                    new Pizza(Random.Range(0, maxToppings + 1)),
                    new Pizza(Random.Range(0, maxToppings + 1))
                };
                
            case OrderDifficulty.Hard:
                maxToppings = 3;
                return new List<Pizza>
                {
                    new Pizza(Random.Range(0, maxToppings + 1)),
                    new Pizza(Random.Range(0, maxToppings + 1)),
                    new Pizza(Random.Range(0, maxToppings + 1))
                };
                default:
                    return new List<Pizza> { new Pizza() };
        }
    }

    private void ChooseCustomer()
    {
        var allNamedNpcs = new List<Entity>(WorldData.Instance.FactionLeaders);
        allNamedNpcs.AddRange(WorldData.Instance.OtherNamedNpcs);

        Customer = allNamedNpcs[Random.Range(0, allNamedNpcs.Count)];
    }
}
