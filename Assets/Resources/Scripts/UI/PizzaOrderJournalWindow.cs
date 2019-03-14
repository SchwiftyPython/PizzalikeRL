using System.Collections.Generic;
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

        for (var i = 0; i < IngredientPrefabParent.transform.childCount; i++)
        {
            Destroy(IngredientPrefabParent.transform.GetChild(i).gameObject);
        }

        var order = _activeOrders[customerName];
        
        var message = $"  {order.Customer.Fluff.Name} has ordered {order.Pizzas.Count} pizza";

        if (order.Pizzas.Count > 1)
        {
            message += "s";
        }

        message += $". \n  Location: {order.CustomerLocation}";

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
        }

        OrderDescription.transform.GetComponent<Text>().text = message;
        OrderDescription.SetActive(true);

        var currentToppingCounts = GameManager.Instance.Player.ToppingCounts;

        foreach (var topping in _requiredToppingCounts)
        {
            var ingredient = Instantiate(IngredientPrefab, Vector3.zero, Quaternion.identity);
            ingredient.transform.SetParent(IngredientPrefabParent.transform);

            var ingredientSpritePrefab = WorldData.Instance.WorldViewToppingsDictionary[topping.Key];
            
            ingredient.GetComponentsInChildren<Image>()[1].sprite = ingredientSpritePrefab.GetComponent<SpriteRenderer>().sprite;

            var textFields = ingredient.GetComponentInChildren<Image>().GetComponentsInChildren<Text>();

            textFields[0].text = topping.Key.ToString();
            textFields[1].text = $"{currentToppingCounts[topping.Key]}/{topping.Value}";
        }
    }
}
