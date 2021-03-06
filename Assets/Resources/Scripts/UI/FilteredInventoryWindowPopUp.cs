﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilteredInventoryWindowPopUp : MonoBehaviour
{
    public GameObject SectionPrefab;
    public GameObject ButtonPrefab;
    public GameObject UnequipButton;

    private IDictionary<string, List<Item>> _sortedItems;
    private IList<GameObject> _itemSections;
    private IDictionary<char, GameObject> _buttons;

    private Transform _sectionParent;

    private IDictionary<Guid, Item> _playerInventory;

    private char _keyMapLetter;

    private bool _processingInput;

    public GameObject FilteredInventoryWindow;
    public GameObject TitleBar;
    public GameObject ActionBar;

    public Entity.EquipmentSlot EquipmentSlotFilter;
    public Guid BodyPartFilterId;

    public static FilteredInventoryWindowPopUp Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        FilteredInventoryWindow.SetActive(false);
        TitleBar.SetActive(false);
        ActionBar.SetActive(false);
        gameObject.SetActive(false);
        _sectionParent = FilteredInventoryWindow.transform;
    }

    private void Update()
    {
        if (Input.anyKeyDown && !_processingInput)
        {
            _processingInput = true;
            char keyPressed;
            char.TryParse(Input.inputString, out keyPressed);
            if (_buttons.ContainsKey(keyPressed))
            {
                var pressedButton = _buttons[keyPressed].transform.GetComponent<Button>();
                pressedButton.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
            
            _processingInput = false;
        }
    }

    public void DisplayAvailableEquipmentForSelectedEquipmentSlot(Entity.EquipmentSlot slot)
    {
        _itemSections = new List<GameObject>();
        _buttons = new Dictionary<char, GameObject>();

        PopulateSectionDictionary(slot);

        _keyMapLetter = 'a';
        foreach (var section in _sortedItems.Keys)
        {
            var sectionHeader = Instantiate(SectionPrefab, new Vector3(0, 0), Quaternion.identity);
            sectionHeader.transform.SetParent(_sectionParent);
            _itemSections.Add(sectionHeader);

            var sectionHeaderText = sectionHeader.GetComponent<TextMeshProUGUI>();
            sectionHeaderText.text = FirstCharToUpper(section) + "s";

            var itemButtonsParent = sectionHeader.transform;

            foreach (var item in _sortedItems[section])
            {
                var itemButton = Instantiate(ButtonPrefab, new Vector3(0, 0), Quaternion.identity);
                itemButton.transform.SetParent(itemButtonsParent);
                _buttons.Add(_keyMapLetter, itemButton);

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
        }

        UnequipButton.SetActive(GameManager.Instance.Player.Equipped[slot] != null);

        FilteredInventoryWindow.SetActive(true);
        TitleBar.SetActive(true);
        ActionBar.SetActive(true);
        gameObject.SetActive(true);
    }

    private void PopulateSectionDictionary(Entity.EquipmentSlot slot)
    {
        EquipmentSlotFilter = slot;
        _playerInventory = GameManager.Instance.Player.Inventory;
        _sortedItems = new Dictionary<string, List<Item>>();

        foreach (var item in _playerInventory.Values)
        {
            if (!item.EquipmentSlots.Contains(EquipmentSlotFilter)) 
            {
                continue;
            }

            if (_sortedItems.ContainsKey(item.ItemType))
            {
                _sortedItems[item.ItemType].Add(item);
            }
            if (!_sortedItems.ContainsKey(item.ItemType))
            {
                _sortedItems.Add(item.ItemType, new List<Item> {item});
            }
        }
    }

    //todo a number of these methods could be moved to global helper
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

    private static string FirstCharToUpper(string input)
    {
        switch (input)
        {
            case null: throw new ArgumentNullException(nameof(input));
            case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default: return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }

    private void DestroyOldItemButtons()
    {
        foreach (var button in _buttons.Values.ToArray())
        {
            Destroy(button);
        }
        _buttons.Clear();
    }

    private void DestroyOldItemSections()
    {
        foreach (var section in _itemSections.ToArray())
        {
            Destroy(section);
        }
        _itemSections.Clear();
    }

    public void Hide()
    {
        DestroyOldItemSections();
        DestroyOldItemButtons();
        FilteredInventoryWindow.SetActive(false);
        TitleBar.SetActive(false);
        ActionBar.SetActive(false);
        gameObject.SetActive(false);
    }
}
