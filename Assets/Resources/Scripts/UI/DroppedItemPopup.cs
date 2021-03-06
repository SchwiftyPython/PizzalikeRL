﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItemPopup : MonoBehaviour, ISubscriber
{
    private readonly IList<string> _subscribedEvents = new List<string>
    {
        GlobalHelper.DroppedItemPopupEventName,
        GlobalHelper.GetItemEventName,
        GlobalHelper.TakeAllEventName
    };

    private bool _processingInput;
    public IDictionary<char, GameObject> Buttons { get; private set; }
    private char _keyMapLetter;

    private List<Item> _items;

    public GameObject ActionBar;

    public GameObject CloseButton;
    public GameObject DroppedItemButtonPrefab;
    public GameObject TakeAllButton;

    public Tile TargetTile;

    private void Start()
    {
        SubscribeToEvents();
        Hide();
    }

    private void Update()
    {
        if (Input.anyKeyDown && !_processingInput)
        {
            _processingInput = true;
            char.TryParse(Input.inputString, out var keyPressed);
            if (Buttons.ContainsKey(keyPressed))
            {
                var pressedButton = Buttons[keyPressed].transform.GetComponent<Button>();
                pressedButton.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
            
            _processingInput = false;
        }
    }

    private void SubscribeToEvents()
    {
        foreach (var eventName in _subscribedEvents)
        {
            EventMediator.Instance.SubscribeToEvent(eventName, this);
        }
    }

    private void UnsubscribeFromEvents()
    {
        EventMediator.Instance.UnsubscribeFromAllEvents(this);
    }

    private void Hide()
    {
        DestroyOldItemButtons();
        gameObject.SetActive(false);
        ActionBar.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        ActionBar.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    public void Refresh()
    {
        DestroyOldItemButtons();
        DisplayItemsInTargetTile(TargetTile);
    }

    public void DisplayDroppedItems()
    {
        if (GameManager.Instance.CurrentTile.PresentItems == null ||
            GameManager.Instance.CurrentTile.PresentItems.Count < 1)
        {
            return;
        }

        Buttons = new Dictionary<char, GameObject>();
        _items = new List<Item>();

        var itemParent = transform;

        _keyMapLetter = 'a';
        foreach (var item in GameManager.Instance.CurrentTile.PresentItems)
        {
            var itemButton = Instantiate(DroppedItemButtonPrefab, new Vector3(0, 0), Quaternion.identity);
            itemButton.transform.SetParent(itemParent);
            Buttons.Add(_keyMapLetter, itemButton);

            var textFields = itemButton.GetComponentsInChildren<TextMeshProUGUI>(true);

            //todo come up with some kind of naming system based on material or legend
            if (item.ItemCategory.Equals("weapon"))
            {
                textFields[1].text = "-  " + item.ItemType + "     [ " + item.ItemDice.NumDice + "d" + item.ItemDice.NumSides + " ]"; //todo add a sword icon
                textFields[0].text = _keyMapLetter.ToString();
            }
            else if (item.ItemCategory.Equals("armor"))
            {
                var defense = ((Armor)item).Defense;
                textFields[1].text = "-  " + item.ItemType + "     [ " + defense + " def ]"; //todo replace def with a shield icon
                textFields[0].text = _keyMapLetter.ToString();
            }
            textFields[2].text = item.Id.ToString();
            NextKeyMapLetter();
        }

        Show();
    }

    public void DisplayItemsInTargetTile(Tile target)
    {
        if ((target.PresentItems == null ||
            target.PresentItems.Count < 1) &&
            (target.PresentProp == null ||
            !target.PresentProp.IsContainer))
        {
            return;
        }

        TargetTile = target;

        Buttons = new Dictionary<char, GameObject>();
        _items = new List<Item>();

        var container = (Chest)TargetTile.PresentProp;

        if (container != null)
        {
            _items.AddRange(container.GetContents());
        }

        if (TargetTile.PresentItems != null)
        {
            _items.AddRange(TargetTile.PresentItems);
        }

        var itemParent = transform;

        _keyMapLetter = 'a';
        foreach (var item in _items)
        {
            var itemButton = Instantiate(DroppedItemButtonPrefab, new Vector3(0, 0), Quaternion.identity);
            itemButton.transform.SetParent(itemParent);
            Buttons.Add(_keyMapLetter, itemButton);

            var textFields = itemButton.GetComponentsInChildren<TextMeshProUGUI>(true);

            //todo come up with some kind of naming system based on material or legend
            if (item.ItemCategory.Equals("weapon"))
            {
                textFields[1].text = "-  " + item.ItemType + "     [ " + item.ItemDice.NumDice + "d" + item.ItemDice.NumSides + " ]"; //todo add a sword icon
                textFields[0].text = _keyMapLetter.ToString();
            }
            else if (item.ItemCategory.Equals("armor"))
            {
                var defense = ((Armor)item).Defense;
                textFields[1].text = "-  " + item.ItemType + "     [ " + defense + " def ]"; //todo replace def with a shield icon
                textFields[0].text = _keyMapLetter.ToString();
            }
            textFields[2].text = item.Id.ToString();
            NextKeyMapLetter();
        }

        Show();
    }

    private void NextKeyMapLetter()
    {
        if (_keyMapLetter == 'z')
        {
            _keyMapLetter = 'a';
        }
        else if (_keyMapLetter == 'Z')
        {
            _keyMapLetter = 'A';
        }
        else
        {
            _keyMapLetter = (char)(_keyMapLetter + 1);
        }
    }

    private void DestroyOldItemButtons()
    {
        if (Buttons == null)
        {
            return;
        }

        foreach (var button in Buttons.Values.ToArray())
        {
            Destroy(button);
        }
        Buttons.Clear();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.DroppedItemPopupEventName))
        {
            if (parameter == null || !(parameter is Tile tile))
            {
                DisplayDroppedItems();
                return;
            }

            DisplayItemsInTargetTile(tile);
        }
        else if (eventName.Equals(GlobalHelper.GetItemEventName)) 
        {
            if (!gameObject.activeSelf || parameter == null || !(parameter is Item item))
            {
                return;
            }

            TargetTile.RemoveItemFromTile(item);

            _items.Remove(item);

            GameManager.Instance.Player.Inventory.Add(item.Id, item);

            var message = $"Picked up {item.ItemType}"; //todo change to item name

            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, message);

            if (_items.Count <= 0) 
            {
                Hide();
            }
            else
            {
                Refresh();
            }
        }
        else if (eventName.Equals(GlobalHelper.TakeAllEventName))
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            var player = GameManager.Instance.Player;

            foreach (var itemButton in Buttons.Values)
            {
                Guid.TryParse(itemButton.transform.GetComponentsInChildren<TextMeshProUGUI>(true)[2].text, out var itemId);

                var item = WorldData.Instance.Items[itemId];

                player.Inventory.Add(itemId, item);

                TargetTile.RemoveItemFromTile(item);

                var message = $"Picked up {item.ItemType}"; //todo change to item name

                EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, message);

                var itemSprite = item.WorldSprite;

                Destroy(itemSprite);
            }

            //Destroy(itemSprite);
            Hide();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }
}
