using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryWindow : MonoBehaviour
{
    public GameObject PlayerInventoryWindow;

    public GameObject SectionPrefab;
    public GameObject ButtonPrefab;

    private Dictionary<string, List<Item>> _sortedItems;
    private List<GameObject> _itemSections;
    private List<GameObject> _buttons;

    private Transform _sectionParent;

    private IDictionary<Guid, Item> _playerInventory;

    private char _keyMapLetter;

    public bool InventoryChanged;

    public static InventoryWindow Instance;

    private void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        _sortedItems = new Dictionary<string, List<Item>>();
        _buttons = new List<GameObject>();
        _sectionParent = transform;

        _itemSections = new List<GameObject>();
        _buttons = new List<GameObject>();

        PlayerInventoryWindow.SetActive(false);
    }

    private void Start()
    {
        PopulateSectionDictionary();
        PopulateWindow();
    }

    private void Update()
    {
        if (isActiveAndEnabled && InventoryChanged)
        {
            InventoryChanged = false;
            DestroyOldItemSections();
            DestroyOldItemButtons();
            PopulateSectionDictionary();
            PopulateWindow();
        }
    }

    private void PopulateSectionDictionary()  //todo sections should be melee, missile armor -- correspond to slots
    {
        _playerInventory = GameManager.Instance.Player.Inventory;
        _sortedItems = new Dictionary<string, List<Item>>();

        foreach (var item in _playerInventory.Values)
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
        _itemSections = new List<GameObject>();
        _buttons = new List<GameObject>();

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
                _buttons.Add(itemButton);

                var textFields = itemButton.GetComponentsInChildren<TextMeshProUGUI>(true);

                //todo come up with some kind of naming system based on material or legend
                if (item.ItemCategory.Equals("weapon"))
                {
                    textFields[1].text = "-  " + item.ItemType + "     [ " + item.ItemDice.NumDice + "d" + item.ItemDice.NumSides + " ]"; //todo add a sword icon
                    textFields[0].text = _keyMapLetter.ToString();
                }
                else if (item.ItemCategory.Equals("armor"))
                {
                    var defense = ((Armor) item).Defense;
                    textFields[1].text = "-  " + item.ItemType + "     [ " + defense + " def ]" ; //todo replace def with a shield icon
                    textFields[0].text = _keyMapLetter.ToString();
                }
                textFields[2].text = item.Id.ToString();
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
        foreach (var button in _buttons)
        {
            Destroy(button);
        }
    }

    private void DestroyOldItemSections()
    {
        foreach (var section in _itemSections)
        {
            Destroy(section);
        }
    }
}
