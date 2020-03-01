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
    private Weapon _selectedWeapon;
    private List<Tile> _aoeTiles;

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
                ThrownButton.SetActive(true);
                ThrownButton.GetComponent<Button>().interactable = true;
                ThrownHitChancePanel.SetActive(true);
                ThrownHitChanceText.text = GetChanceToHitRanged(presentEntity);
            }
            else
            {
                ThrownButton.SetActive(false);
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

        //todo check if window overlaps highlighted tiles
        //todo check if window is near edge of game area
        //todo possibly make window draggable so player can adjust if needed
        gameObject.transform.position = new Vector2(pos.x + 90f, pos.y + 80f);

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
        _player.RangedAttack(_selectedTile.GetPresentEntity(), _selectedWeapon);
        AfterActionCleanup();
    }

    public void OnHoverRangedAttackButton()
    {
        var equippedMissileWeapon = _player.GetEquippedMissileWeapon();

        if (equippedMissileWeapon == null)
        {
            return;
        }

        if (equippedMissileWeapon.Properties.Contains("aoe"))
        {
            OnHoverWithAoE(equippedMissileWeapon);
        }
    }

    public void OnMeleeAttackButtonClicked()
    {
        _player.MeleeAttack(_selectedTile.GetPresentEntity());
        AfterActionCleanup();
    }

    public void OnThrowWeaponButtonClicked()
    {
        if (_aoeTiles != null && _aoeTiles.Count > 0)
        {
            _player.RangedAttackAOE(_aoeTiles, _selectedWeapon);
        }
        else
        {
            _player.RangedAttack(_selectedTile.GetPresentEntity(), _selectedWeapon);
        }
        AfterActionCleanup();
    }

    public void OnHoverThrowWeaponButton()
    {
        var equippedThrownWeapon = _player.GetEquippedThrownWeapon();

        if (equippedThrownWeapon == null)
        {
            return;
        }

        if (equippedThrownWeapon.Properties.Contains("aoe"))
        {
            OnHoverWithAoE(equippedThrownWeapon);
        }
    }

    public void OnHoverWithAoE(Weapon aoeWeapon)
    {
        //todo calc hit chance for each entity in aoe? Maybe avg of all instead of a bunch of windows

        var aoe = aoeWeapon.AOE;

        List<Tile> tilesToHighlight = null;

        if (aoe.GetType() == typeof(BlastAOE))
        {
            tilesToHighlight =
                new List<Tile>(AreaMap.Instance.GetCellsInCircle(_selectedTile.X, _selectedTile.Y,
                    ((BlastAOE) aoeWeapon.AOE).GetRadius()));
        }

        _aoeTiles = tilesToHighlight;
        _selectedWeapon = aoeWeapon;

        InputController.Instance.HighlightTiles(tilesToHighlight);
    }

    public void OnButtonExit()
    {
        _aoeTiles = null;
        InputController.Instance.ClearHighlights();
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
