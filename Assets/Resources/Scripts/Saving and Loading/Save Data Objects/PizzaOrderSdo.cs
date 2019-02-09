using System;
using System.Collections.Generic;

[Serializable]
public class PizzaOrderSdo
{
    public List<Pizza> Pizzas;

    public Guid CustomerId;

    public string CustomerLocationId;

    public static PizzaOrderSdo ConvertToPizzaOrderSdo(PizzaOrder order)
    {
        var tempSdo = new PizzaOrderSdo
        {
            CustomerId = order.Customer.Id,
            CustomerLocationId = order.CustomerLocation.Id,
            Pizzas = order.Pizzas
        };

        return tempSdo;
    }

    public static PizzaOrder ConvertToPizzaOrder(PizzaOrderSdo sdo)
    {
        var order = new PizzaOrder
        {
            Customer = WorldData.Instance.Entities[sdo.CustomerId],
            CustomerLocation = WorldData.Instance.MapDictionary[sdo.CustomerLocationId],
            Pizzas = sdo.Pizzas
        };

        return order;
    }
}
