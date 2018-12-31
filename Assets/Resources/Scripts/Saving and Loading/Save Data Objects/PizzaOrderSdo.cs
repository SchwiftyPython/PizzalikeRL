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
}
