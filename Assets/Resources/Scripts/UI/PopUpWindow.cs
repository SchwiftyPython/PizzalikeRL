using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow : MonoBehaviour
{
    public GameObject Window;
    public Text MessageField;
    
    public void Show(PizzaOrder order)
    {
        var message = $"{order.Customer.Fluff.Name} has ordered {order.Pizzas.Count} pizzas. \n Location: {order.CustomerLocation}";

        var currentPizzaNumber = 0;
        foreach (var pizza in order.Pizzas)
        {
            currentPizzaNumber++;
            message += "\nPizza #" + currentPizzaNumber + "\n\n";

            var pizzaOrderDetails = string.Empty;
            pizzaOrderDetails += $" Size: {pizza.PizzaSize}" +
                                 " \nToppings: ";

            pizzaOrderDetails =
                pizza.PizzaToppings.Aggregate(pizzaOrderDetails, (current, topping) => current + $" {topping}\n ");
            message += pizzaOrderDetails;
        }
        MessageField.text = message;
        Window.SetActive(true);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }
}
