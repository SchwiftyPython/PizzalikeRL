using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionWindow : MonoBehaviour, ISubscriber
{
    private Entity _player;
    private Tile _selectedTile;

    public GameObject MoveHereButton;
    public GameObject RangedAttackButton;
    public GameObject MeleeAttackButton;
    public GameObject DeliverButton;
    
    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent("ActionPopup", this);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
        InputController.Instance.ClearHighlights();
    }

    //todo make buttons prefab and display only relevant actions
    public void OnTileSelected(Tile tile)
    {
        _selectedTile = tile;
        _player = GameManager.Instance.Player;

        var presentEntity = tile.GetPresentEntity();

        MoveHereButton.GetComponent<Button>().interactable = !tile.GetBlocksMovement();

        if (presentEntity != null)
        {
            if (_player.HasRangedWeaponEquipped() &&
                _player.EquippedWeaponInRangeOfTarget(presentEntity))
            {
                RangedAttackButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                RangedAttackButton.GetComponent<Button>().interactable = false;
            }

            MeleeAttackButton.GetComponent<Button>().interactable = _player.CalculateDistanceToTarget(presentEntity) < 2;

            if (presentEntity.IsCustomer && OrderReadyForDelivery(presentEntity))
            {
                DeliverButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                DeliverButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            RangedAttackButton.GetComponent<Button>().interactable = false;
            MeleeAttackButton.GetComponent<Button>().interactable = false;
            DeliverButton.GetComponent<Button>().interactable = false;
        }

        var pos = Input.mousePosition;

        gameObject.transform.position = new Vector2(pos.x + 60f, pos.y + 50f);

        gameObject.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    public void OnMoveHereButtonClicked()
    {
        //todo auto move
        AfterActionCleanup();
    }

    public void OnRangedAttackButtonClicked()
    {
        _player.RangedAttack(_selectedTile.GetPresentEntity());
        AfterActionCleanup();
    }

    public void OnMeleeAttackButtonClicked()
    {
        _player.MeleeAttack(_selectedTile.GetPresentEntity());
        AfterActionCleanup();
    }

    public void OnDeliverButtonClicked()
    {
        var presentEntity = _selectedTile.GetPresentEntity();

        GameManager.Instance.ActiveOrders.Remove(presentEntity.Fluff.Name);

        //Remove marker from customer
        GlobalHelper.DestroyAllChildren(presentEntity.GetSprite());

        //todo popup sucking off player for delivering pizza
        //todo award whatever skill point currency
        //todo Generate some fluff about order, create landmark, etc

        AfterActionCleanup();
    }

    private void AfterActionCleanup()
    {
        GameManager.Instance.CurrentState = GameManager.GameState.EndTurn;
        Hide();
    }

    private static bool OrderReadyForDelivery(Entity presentEntity)
    {
        var order = (from customerName in GameManager.Instance.ActiveOrders.Keys
            where presentEntity.Fluff.Name.Equals(customerName, StringComparison.OrdinalIgnoreCase)
            select GameManager.Instance.ActiveOrders[customerName]).FirstOrDefault();

        if (order == null)
        {
            return false;
        }

        var requiredToppingCounts = new Dictionary<Toppings, int>();

        foreach (var pizza in order.Pizzas)
        {
            foreach (var topping in pizza.PizzaToppings)
            {
                if (requiredToppingCounts.ContainsKey(topping.Key))
                {
                    requiredToppingCounts[topping.Key] += topping.Value;
                }
                else
                {
                    requiredToppingCounts.Add(topping.Key, topping.Value);
                }
            }
        }

        var currentToppingCounts = GameManager.Instance.Player.ToppingCounts;

        return requiredToppingCounts.All(topping => currentToppingCounts[topping.Key] >= topping.Value);
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent("ActionPopup", this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (GameManager.Instance.AnyActiveWindows())
        {
            return;
        }

        if (!(parameter is Tile tile))
        {
            return;
        }

        OnTileSelected(tile);
    }
}
