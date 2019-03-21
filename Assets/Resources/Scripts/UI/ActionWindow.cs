using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionWindow : MonoBehaviour
{
    private Entity _player;
    private Tile _selectedTile;

    public GameObject Window;

    public GameObject MoveHereButton;
    public GameObject RangedAttackButton;
    public GameObject MeleeAttackButton;
    public GameObject DeliverButton;

    public static ActionWindow Instance;
    
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
        Window.SetActive(false);
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

        Window.transform.position = new Vector2(pos.x + 60f, pos.y + 50f);

        Window.SetActive(true);
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
        //todo complete order
        AfterActionCleanup();
    }

    private void AfterActionCleanup()
    {
        InputController.Instance.ClearHighlights();
        Window.SetActive(false);
        GameManager.Instance.CurrentState = GameManager.GameState.EndTurn;
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
}
