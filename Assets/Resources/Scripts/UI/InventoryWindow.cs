using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryWindow : MonoBehaviour, ISubscriber
{
    public GameObject PlayerInventoryWindow;

    public GameObject SectionPrefab;
    public GameObject ButtonPrefab;

    private Dictionary<string, List<Item>> _sortedItems;
    private List<GameObject> _itemSections;
    private List<GameObject> _buttons;

    public Transform SectionParent;
    public GameObject ItemInformation;

    private IDictionary<Guid, Item> _playerInventory;

    private char _keyMapLetter;

    public bool InventoryChanged;

    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    public TextMeshProUGUI ItemModifiers;
    public TextMeshProUGUI ItemRarity;

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

        _itemSections = new List<GameObject>();
        _buttons = new List<GameObject>();

        PlayerInventoryWindow.SetActive(false);
    }

    private void Start()
    {
        PopulateSectionDictionary();
        PopulateWindow();

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemSelectedEventName, this);
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

    public void ShowItemDescription(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (item.ItemCategory.Equals("weapon"))
        {
            ItemName.text =
                $"{ItemStore.Instance.GetDisplayNameForItemType(item.ItemType)}     [ {item.ItemDice.NumDice}d{item.ItemDice.NumSides} ]"; //todo add a sword icon
        }
        else if (item.ItemCategory.Equals("armor"))
        {
            var defense = ((Armor)item).Defense;
            ItemName.text =
                $"{ItemStore.Instance.GetDisplayNameForItemType(item.ItemType)}     [ {defense} def ]"; //todo replace def with a shield icon
        }
        else
        {
            ItemName.text =
                $"{ItemStore.Instance.GetDisplayNameForItemType(item.ItemType)}";
        }

        ItemDescription.text = string.Empty;

        ItemModifiers.text = string.Empty;

        ItemRarity.text = item.Rarity.ToString();

        ItemInformation.SetActive(true);
    }

    //todo sections should be melee, missile, armor -- correspond to slots. Could also offer group by option. Find by property.
    private void PopulateSectionDictionary()  
    {
        _playerInventory = GameManager.Instance.Player.Inventory;
        _sortedItems = new Dictionary<string, List<Item>>();

        foreach (var item in _playerInventory.Values)
        {
            string sectionName;

            var weapon = item as Weapon;

            if (item.ItemCategory.Equals("weapon", StringComparison.OrdinalIgnoreCase))
            {
                if (item.ItemType.IndexOf("grenade", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    sectionName = "Grenades";
                }
                else if (((Weapon) item).IsRanged)
                {
                    sectionName = "Ranged";
                }
                else
                {
                    sectionName = "Melee";
                }
            }
            else
            {
                sectionName = GlobalHelper.Capitalize(item.ItemCategory);
            }

            if (_sortedItems.ContainsKey(sectionName))
            {
                _sortedItems[sectionName].Add(item);
            }
            else
            {
                _sortedItems.Add(sectionName, new List<Item> { item });
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
            sectionHeader.transform.SetParent(SectionParent);
            _itemSections.Add(sectionHeader);

            var sectionHeaderText = sectionHeader.GetComponent<TextMeshProUGUI>();
            sectionHeaderText.text = section;

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
                    textFields[1].text = $"-  {item.ItemName}     [ {item.ItemDice.NumDice}d{item.ItemDice.NumSides} ]"; //todo add a sword icon
                    textFields[0].text = _keyMapLetter.ToString();
                }
                else if (item.ItemCategory.Equals("armor"))
                {
                    var defense = ((Armor) item).Defense;
                    textFields[1].text = $"-  {item.ItemName}     [ {defense} def ]"; //todo replace def with a shield icon
                    textFields[0].text = _keyMapLetter.ToString();
                }
                else
                {
                    textFields[1].text = $"-  {item.ItemName}";
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

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.ItemSelectedEventName, StringComparison.OrdinalIgnoreCase))
        {
            if (!(parameter is Item item))
            {
                return;
            }

            ShowItemDescription(item);
        }
    }
}
