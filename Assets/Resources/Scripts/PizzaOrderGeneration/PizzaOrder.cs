using System.Collections.Generic;
using System.Linq;
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
    
    public string CustomerLocation;

    public PizzaOrder() { }

    public PizzaOrder(OrderDifficulty difficulty = OrderDifficulty.Easy)
    {
        Pizzas = new List<Pizza>();
        Pizzas.AddRange(GenerateOrder(difficulty));

        ChooseCustomer();
        CustomerLocation = Customer.CurrentCell.Settlement.Name; 
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
        var allNpcs = new List<Entity>(WorldData.Instance.Entities.Values)
            .Where(e => e.IsPlayer() == false && e.Fluff != null).ToList();

        Customer = allNpcs[Random.Range(0, allNpcs.Count)];
        Customer.IsCustomer = true;
    }
}
