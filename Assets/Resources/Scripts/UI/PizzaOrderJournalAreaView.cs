using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PizzaOrderJournalAreaView : MonoBehaviour
{
    private IDictionary<string, PizzaOrder> _activeOrders;
    private IDictionary<Toppings, int> _requiredToppingCounts;

    public GameObject PizzaOrderTabPrefab;
    public GameObject OrderTabParent;
    public GameObject OrderDescription;
    public GameObject IngredientPrefab;
    public GameObject IngredientPrefabParent;
    public GameObject OneOrderBarPrefab;

    public static PizzaOrderJournalAreaView Instance;

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
        PopulateWindow();
    }

    //todo register for active order change event
    private void Update()
    {
        if (_activeOrders == null || _activeOrders.Count != GameManager.Instance.ActiveOrders.Count ||
            _activeOrders.Count != OrderTabParent.transform.childCount)
        {
            RefreshOrderDisplay();
        }
    }

    private void PopulateWindow()
    {
        if (_activeOrders == null || _activeOrders != GameManager.Instance.ActiveOrders)
        {
            _activeOrders = GameManager.Instance.ActiveOrders;
        }

        var orderNumber = 1;

        foreach (var order in _activeOrders.Values)
        {
            if (_activeOrders.Count > 1)
            {
                var orderTab = Instantiate(PizzaOrderTabPrefab, new Vector3(0, 0), Quaternion.identity);
                orderTab.transform.SetParent(OrderTabParent.transform);

                var orderTitle = orderTab.GetComponentInChildren<TextMeshProUGUI>();
                orderTitle.text = $"Order {orderNumber}";
            }
            else
            {
                var oneOrderBar = Instantiate(OneOrderBarPrefab, new Vector3(0, 0), Quaternion.identity);
                oneOrderBar.transform.SetParent(OrderTabParent.transform);
            }

            _requiredToppingCounts = new Dictionary<Toppings, int>();

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
            orderNumber++;
        }

        if (_activeOrders.Count == 1)
        {
            DisplayOrderDetails(0);
        }
    }

    public void DisplayOrderDetails(int orderTabNumber)
    {
        _requiredToppingCounts = new Dictionary<Toppings, int>();

        GlobalHelper.DestroyAllChildren(IngredientPrefabParent);

        var order = _activeOrders.ElementAt(orderTabNumber).Value;

        var message = $"Customer: {order.Customer.Fluff.Name}";

        message += $"\nLocation: {order.CustomerLocation}";

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

        OrderDescription.transform.GetComponent<TextMeshProUGUI>().text = message;
        OrderDescription.SetActive(true);

        var currentToppingCounts = GameManager.Instance.Player.ToppingCounts;

        foreach (var topping in _requiredToppingCounts)
        {
            var ingredient = Instantiate(IngredientPrefab, Vector3.zero, Quaternion.identity);
            ingredient.transform.SetParent(IngredientPrefabParent.transform);

            var ingredientSpritePrefab = WorldData.Instance.WorldViewToppingsDictionary[topping.Key];

            ingredient.GetComponentsInChildren<Image>()[1].sprite = ingredientSpritePrefab.GetComponent<SpriteRenderer>().sprite;

            var textFields = ingredient.GetComponentInChildren<Image>().GetComponentsInChildren<TextMeshProUGUI>();

            textFields[0].text = topping.Key.ToString();
            textFields[1].text = $"{currentToppingCounts[topping.Key]}/{topping.Value}";
        }
    }

    //todo register for topping picked up event, new order event, completed order event
    //Not confident kill and fill is good idea
    public void RefreshOrderDisplay()
    {
        Clear();
        PopulateWindow();
    }

    private void Clear()
    {
        GlobalHelper.DestroyAllChildren(OrderTabParent);
        GlobalHelper.DestroyAllChildren(IngredientPrefabParent);
        OrderDescription.transform.GetComponent<TextMeshProUGUI>().text = string.Empty;
    }
}
