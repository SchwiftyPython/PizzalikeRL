using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActionWindow : MonoBehaviour, ISubscriber
{
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
    public GameObject EquipButton; //todo for items that can be equipped in multiple spots (R or L hand for example), make an equip button for each slot?
    public GameObject LookButton;
    public GameObject ReadButton;

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
        var player = GameManager.Instance.Player;

        player.EquipItem(_selectedItem, _selectedItem.EquipmentSlotType);
        Hide();
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
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.DroppedItemSelectedEventName, this);
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.ItemSelectedEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.DroppedItemSelectedEventName))
        {
            if (!(parameter is Item item))
            {
                return;
            }

            OnItemSelected(item, PopupContext.DroppedItem);
        }

        if (eventName.Equals(GlobalHelper.ItemSelectedEventName))
        {
            if (!(parameter is Item item))
            {
                return;
            }

            OnItemSelected(item, PopupContext.MenuItem);
        }
    }
}
