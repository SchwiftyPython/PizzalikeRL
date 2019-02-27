using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PizzaOrderJournalWindow : MonoBehaviour
{
    public GameObject PizzaOrderPrefab;
    public GameObject OrderButtonParent;
    public GameObject OrderDescription;

    private IDictionary<string, PizzaOrder> _activeOrders;

    public static PizzaOrderJournalWindow Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _activeOrders = GameManager.Instance.ActiveOrders;
        PopulateWindow();
    }

    private void PopulateWindow()
    {
        foreach (var order in _activeOrders.Values)
        {
            var orderButton = Instantiate(PizzaOrderPrefab, new Vector3(0, 0), Quaternion.identity);
            orderButton.transform.SetParent(OrderButtonParent.transform);
            
            var orderTitle = orderButton.GetComponentInChildren<Toggle>().GetComponentInChildren<Text>();
            orderTitle.text = $"{order.Customer.Fluff.Name}";
        }
    }

    public void DisplayOrderDetails(string customerName)
    {
        var order = _activeOrders[customerName];

        var message = $"{order.Customer.Fluff.Name} has ordered {order.Pizzas.Count} pizzas. \n Location: {order.CustomerLocation.X}, {order.CustomerLocation.Y} ";

        var currentPizzaNumber = 0;
        foreach (var pizza in order.Pizzas)
        {
            currentPizzaNumber++;
            message += "\nPizza #" + currentPizzaNumber + "\n\n";

            var pizzaOrderDetails = string.Empty;
            pizzaOrderDetails += $"Size: {pizza.PizzaSize}" +
                                 $" \nCrust: {pizza.PizzaCrust}" +
                                 $" \nSauce: {pizza.PizzaSauce}" +
                                 " \nToppings: ";

            pizzaOrderDetails =
                pizza.PizzaToppings.Aggregate(pizzaOrderDetails, (current, topping) => current + $" {topping}\n ");
            message += pizzaOrderDetails;
        }
        OrderDescription.transform.GetComponent<Text>().text = message;
        OrderDescription.SetActive(true);
    }
}
