using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemActionWindow : MonoBehaviour, ISubscriber
{
    private readonly Color _equipmentSlotPanelSolid = new Color(107, 107, 107, 255);
    private readonly Color _equipmentSlotPanelInvisible = new Color(107, 107, 107, 0);

    private Item _selectedItem;

    private List<GameObject> _allButtons;
    private List<GameObject> _droppedItemButtons;
    private List<GameObject> _menuItemButtons;
    
    private enum PopupContext
    {
        DroppedItem,
        MenuItem
    }

    public GameObject GetButton;
    public GameObject EquipButton; 
    public GameObject LookButton;
    public GameObject ReadButton;

    public GameObject EquipmentSlotPanel;
    public GameObject EquipmentSlotPrefab;

    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.DroppedItemSelectedEventName, this);
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemSelectedEventName, this);

        _allButtons = new List<GameObject>
        {
            GetButton,
            EquipButton,
            LookButton,
            ReadButton
        };

        _droppedItemButtons = new List<GameObject>
        {
            GetButton,
            LookButton
        };

        //todo need to make distinction between equipable items and other types once they are implemented
        _menuItemButtons = new List<GameObject>
        {
            EquipButton
        };

        gameObject.SetActive(false);
        EquipmentSlotPanel.SetActive(false);
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
        EquipmentSlotPanel.SetActive(false);
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnGetButtonClicked()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.GetItemEventName, this, _selectedItem);
        Hide();
    }

    public void OnLookButtonClicked()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.InspectItemEventName, this, _selectedItem);
        Hide();
    }

    public void OnEquipButtonClicked()
    {
        PopulateEquipmentSlotPanel();
        EquipmentSlotPanel.SetActive(true);
    }

    private void PopulateEquipmentSlotPanel()
    {
        GlobalHelper.DestroyAllChildren(EquipmentSlotPanel);

        foreach (var slot in _selectedItem.EquipmentSlots)
        {
            var equipmentSlotButton = Instantiate(EquipmentSlotPrefab, new Vector3(0, 0), Quaternion.identity);
            equipmentSlotButton.transform.SetParent(EquipmentSlotPanel.transform);

            var script = equipmentSlotButton.GetComponent<EquipmentSlotButton>();

            script.EquipmentSlot = slot;
            script.ItemToEquip = _selectedItem;

            equipmentSlotButton.GetComponentInChildren<TextMeshProUGUI>().text = GlobalHelper.GetEnumDescription(slot);
        }

        EquipmentSlotPanel.GetComponentInChildren<Image>().color = _selectedItem.EquipmentSlots.Count > 1
            ? _equipmentSlotPanelSolid
            : _equipmentSlotPanelInvisible;

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EquipmentSlotSelectedEventName, this);
    }

    private void OnItemSelected(Item item, PopupContext context)
    {
        _selectedItem = item;

        var pos = Input.mousePosition;

        //todo check if window is near edge of game area
        //todo possibly make window draggable so player can adjust if needed
        gameObject.transform.position = new Vector2(pos.x + 90f, pos.y + 80f);

        LoadButtons(context == PopupContext.DroppedItem ? _droppedItemButtons : _menuItemButtons);

        gameObject.SetActive(true);
        EquipmentSlotPanel.SetActive(false);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void LoadButtons(ICollection<GameObject> buttonsToLoad)
    {
        foreach (var button in _allButtons)
        {
            button.SetActive(buttonsToLoad.Contains(button));
        }
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromAllEvents(this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.DroppedItemSelectedEventName, StringComparison.OrdinalIgnoreCase))
        {
            if (!(parameter is Item item))
            {
                return;
            }

            OnItemSelected(item, PopupContext.DroppedItem);
        }

        if (eventName.Equals(GlobalHelper.ItemSelectedEventName, StringComparison.OrdinalIgnoreCase))
        {
            if (!(parameter is Item item))
            {
                return;
            }

            OnItemSelected(item, PopupContext.MenuItem);
        }

        if (eventName.Equals(GlobalHelper.EquipmentSlotSelectedEventName, StringComparison.OrdinalIgnoreCase))
        {
            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EquipmentSlotSelectedEventName, this);
            Hide();
        }
    }
}
