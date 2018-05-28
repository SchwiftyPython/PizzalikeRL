using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    public GameObject SectionPrefab;
    public GameObject ButtonPrefab;

    private Dictionary<string, List<Item>> _sortedItems;
    private List<GameObject> _buttons;

    private Transform _sectionParent;

    private List<Item> _playerInventory;

    private char _keyMapLetter;

    private void Start ()
    {
        _playerInventory = GameManager.Instance.Player.Inventory;
        _sortedItems = new Dictionary<string, List<Item>>();
        _buttons = new List<GameObject>();
        _sectionParent = transform;

        PopulateSectionDictionary();
        PopulateWindow();
    }

    private void PopulateSectionDictionary()
    {
        foreach (var item in _playerInventory)
        {
            if (_sortedItems.ContainsKey(item.ItemType))
            {
                _sortedItems[item.ItemType].Add(item);
            }
            if (!_sortedItems.ContainsKey(item.ItemType))
            {
                _sortedItems.Add(item.ItemType, new List<Item>{ item }); 
            }
        }
    }

    private void PopulateWindow()
    {
        _keyMapLetter = 'a';
        foreach (var section in _sortedItems.Keys)
        {
            var sectionHeader = Instantiate(SectionPrefab, new Vector3(0, 0), Quaternion.identity);
            sectionHeader.transform.SetParent(_sectionParent);
            var sectionHeaderText = sectionHeader.GetComponent<Text>();
            sectionHeaderText.text = FirstCharToUpper(section) + "s";

            var itemButtonsParent = sectionHeader.transform;

            foreach (var item in _sortedItems[section])
            {
                var itemButton = Instantiate(ButtonPrefab, new Vector3(0, 0), Quaternion.identity);
                itemButton.transform.SetParent(itemButtonsParent);
                var textFields = itemButton.GetComponentsInChildren<Text>();

                //todo come up with some kind of naming system based on material or legend
                if (item.ItemCategory.Equals("weapon"))
                {
                    textFields[0].text = "-  " + item.ItemType + "     [ " + item.ItemDice.NumDice + "d" + item.ItemDice.NumSides + " ]"; //todo add a sword icon
                    textFields[1].text = _keyMapLetter.ToString();
                }
                else if (item.ItemCategory.Equals("armor"))
                {
                    var defense = ((Armor) item).Defense;
                    textFields[0].text = "-  " + item.ItemType + "     [ " + defense + " def ]" ; //todo replace def with a shield icon
                    textFields[1].text = _keyMapLetter.ToString();
                }
                NextKeyMapLetter();
            }
        }
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

    public static string FirstCharToUpper(string input)
    {
        switch (input)
        {
            case null: throw new ArgumentNullException(nameof(input));
            case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default: return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
