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
    public GameObject IngredientPrefab;
    public GameObject IngredientPrefabParent;

    private IDictionary<string, PizzaOrder> _activeOrders;
    private IDictionary<Toppings, int> _requiredToppingCounts;

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
        _requiredToppingCounts = new Dictionary<Toppings, int>();

        var order = _activeOrders[customerName];

        var message = $"{order.Customer.Fluff.Name} has ordered {order.Pizzas.Count} pizzas. \n Location: {order.CustomerLocation}";

        var currentPizzaNumber = 0;
        foreach (var pizza in order.Pizzas)
        {
            foreach (var topping in pizza.PizzaToppings) 
            {
                if (_requiredToppingCounts.ContainsKey(topping.Key))
                {
                    _requiredToppingCounts[topping.Key] += topping.Value;
                }
                else
                {
                    _requiredToppingCounts.Add(topping.Key, topping.Value);
                }
            }

            currentPizzaNumber++;
            message += "\nPizza #" + currentPizzaNumber + "\n\n";

            var pizzaOrderDetails = string.Empty;
            pizzaOrderDetails += $"Size: {pizza.PizzaSize}" +
                                 " \nToppings: ";

            pizzaOrderDetails =
                pizza.PizzaToppings.Aggregate(pizzaOrderDetails, (current, topping) => current + $" {topping}\n ");
            message += pizzaOrderDetails;
        }

        OrderDescription.transform.GetComponent<Text>().text = message;
        OrderDescription.SetActive(true);

        foreach (var topping in _requiredToppingCounts)
        {
            var ingredient = Instantiate(IngredientPrefab, Vector3.zero, Quaternion.identity);
            ingredient.transform.SetParent(IngredientPrefabParent.transform);

            var ingredientSpritePrefab = WorldData.Instance.WorldViewToppingsDictionary[topping.Key];
            
            ingredient.GetComponentInChildren<Image>().sprite = ingredientSpritePrefab.GetComponent<SpriteRenderer>().sprite;
            ingredientSpritePrefab.transform.SetParent(ingredient.transform);

            var textFields = ingredient.GetComponentInChildren<Image>().GetComponentsInChildren<Text>();

            textFields[0].text = topping.Key.ToString();
            textFields[1].text = topping.Value.ToString();
        }
    }
}
