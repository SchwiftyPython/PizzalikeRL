using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public GameObject ThrownButton;
    public GameObject RangedHitChancePanel;
    public TextMeshProUGUI RangedHitChanceText;
    public GameObject MeleeHitChancePanel;
    public TextMeshProUGUI MeleeHitChanceText;
    public GameObject ThrownHitChancePanel;
    public TextMeshProUGUI ThrownHitChanceText;

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

    public void Hide()
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
            if (_player.HasMissileWeaponsEquipped() &&
                _player.EquippedMissileWeaponsInRangeOfTarget(presentEntity)
                && presentEntity.CurrentTile.Visibility == Visibilities.Visible)
            {
                RangedAttackButton.SetActive(true);
                RangedAttackButton.GetComponent<Button>().interactable = true;
                RangedHitChancePanel.SetActive(true);
                RangedHitChanceText.text = GetChanceToHitRanged(presentEntity);
            }
            else
            {
                RangedAttackButton.SetActive(false);
                RangedAttackButton.GetComponent<Button>().interactable = false;
                RangedHitChancePanel.SetActive(false);
            }

            if (_player.HasThrownWeaponEquipped() &&
                _player.ThrownWeaponInRangeOfTarget(presentEntity)
                && presentEntity.CurrentTile.Visibility == Visibilities.Visible)
            {
                RangedAttackButton.SetActive(true);
                ThrownButton.GetComponent<Button>().interactable = true;
                ThrownHitChancePanel.SetActive(true);
                ThrownHitChanceText.text = GetChanceToHitRanged(presentEntity);
            }
            else
            {
                RangedAttackButton.SetActive(false);
                ThrownButton.GetComponent<Button>().interactable = false;
                ThrownHitChancePanel.SetActive(false);
            }

            MeleeAttackButton.GetComponent<Button>().interactable = _player.CalculateDistanceToTarget(presentEntity) < 2;
            MeleeAttackButton.SetActive(MeleeAttackButton.GetComponent<Button>().interactable);
            MeleeHitChancePanel.SetActive(MeleeAttackButton.GetComponent<Button>().interactable);
            MeleeHitChanceText.text = GetChanceToHitMelee(presentEntity);

            if (presentEntity.IsCustomer && OrderReadyForDelivery(presentEntity))
            {
                DeliverButton.GetComponent<Button>().interactable = true;
                DeliverButton.SetActive(true);
            }
            else
            {
                DeliverButton.GetComponent<Button>().interactable = false;
                DeliverButton.SetActive(false);
            }
        }
        else
        {
            RangedAttackButton.GetComponent<Button>().interactable = false;
            RangedHitChancePanel.SetActive(false);
            RangedAttackButton.SetActive(false);
            MeleeAttackButton.GetComponent<Button>().interactable = false;
            MeleeHitChancePanel.SetActive(false);
            MeleeAttackButton.SetActive(false);
            DeliverButton.GetComponent<Button>().interactable = false;
            DeliverButton.SetActive(false);
            ThrownButton.GetComponent<Button>().interactable = false;
            ThrownButton.SetActive(false);
            ThrownHitChancePanel.SetActive(false);
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

    public void OnThrowWeaponButtonClicked()
    {
        _player.RangedAttack(_selectedTile.GetPresentEntity(), GlobalHelper.RangedAttackType.Thrown);
        AfterActionCleanup();
    }

    public void OnDeliverButtonClicked()
    {
        var presentEntity = _selectedTile.GetPresentEntity();

        EventMediator.Instance.Broadcast("Delivered", this, presentEntity);

        //todo popup sucking off player for delivering pizza
        //todo award whatever skill point currency
        //todo Generate some fluff about order, create landmark, etc

        AfterActionCleanup();
    }

    private string GetChanceToHitRanged(Entity presentEntity)
    {
        return _player.GetChanceToHitRangedTarget(presentEntity) + "%";
    }

    private string GetChanceToHitMelee(Entity presentEntity)
    {
        return _player.GetChanceToHitMeleeTarget(presentEntity) + "%";
    }

    private void AfterActionCleanup()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.ActionTakenEventName, this);
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
