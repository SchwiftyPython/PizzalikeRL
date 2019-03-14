using System;
using System.Collections.Generic;

[Serializable]
public class PizzaOrderSdo
{
    public List<Pizza> Pizzas;

    public Guid CustomerId;

    public static PizzaOrderSdo ConvertToPizzaOrderSdo(PizzaOrder order)
    {
        var tempSdo = new PizzaOrderSdo
        {
            CustomerId = order.Customer.Id,
            Pizzas = order.Pizzas
        };

        return tempSdo;
    }

    public static PizzaOrder ConvertToPizzaOrder(PizzaOrderSdo sdo)
    {
        var order = new PizzaOrder
        {
            Customer = WorldData.Instance.Entities[sdo.CustomerId],
            Pizzas = sdo.Pizzas
        };

        order.CustomerLocation = order.Customer.CurrentCell.Settlement.Name;

        return order;
    }
}
