using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ItemPrefabKeys
{
    [Description("Cowboy Hat")]
    CowboyHat,
    [Description("Energy Armor")]
    EnergyArmor,
    [Description("Flak Armor")]
    FlakArmor,
    [Description("Ballistic Helmet")]
    BallisticHelmet,
    [Description("Leather Armor")]
    LeatherArmor,
    [Description("Plate Armor")]
    PlateArmor,
    [Description("Robes")]
    Robes,
    [Description("Trench Coat")]
    TrenchCoat,
    [Description("Tuque")]
    Tuque,
    [Description("Assault Rifle")]
    AssaultRifle,
    [Description("Battle Axe")]
    BattleAxe,
    [Description("Bolt Action Rifle")]
    BoltActionRifle,
    [Description("Club")]
    Club,
    [Description("Folding Chair")]
    FoldingChair,
    [Description("Frag Grenade")]
    FragGrenade,
    [Description("Great Bow")]
    GreatBow,
    [Description("Great Sword")]
    GreatSword,
    [Description("Emp Grenade")]
    EmpGrenade,
    [Description("Hand Axe")]
    HandAxe,
    [Description("Knife")]
    Knife,
    [Description("Laser Rifle")]
    LaserRifle,
    [Description("Rocket Launcher")]
    RocketLauncher,
    [Description("Light Machine Gun")]
    LightMachineGun,
    [Description("Revolver")]
    Revolver,
    [Description("Semi Auto Handgun")]
    SemiAutoHandgun,
    [Description("Short Sword")]
    ShortSword,
    [Description("Shotgun")]
    Shotgun,
    [Description("Sniper Rifle")]
    SniperRifle,
    [Description("Spear")]
    Spear,
    [Description("Submachine Gun")]
    Submachinegun,
    [Description("Warhammer")]
    Warhammer
}

public class ItemStore : MonoBehaviour
{
    private Dictionary<ItemPrefabKeys, GameObject> _itemPrefabsWorldView;
    private List<string> _baseItemTemplateTypes;

    public static ItemStore Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        _baseItemTemplateTypes = ItemTemplateLoader.GetItemTemplateTypes();
        PopulatePrefabDictionaries();
    }

    //<Summary>
    //    Returns a random item of any rarity 
    //</Summary>
    public Item GetRandomItem()
    {
        var rarity = GetRandomRarity();

        var itemTemplate = GetRandomBaseItemTemplate();
        
        switch (itemTemplate.Category.ToLower())
        {
            case "armor":
                return new Armor(itemTemplate, rarity);
            case "weapon":
                return new Weapon(itemTemplate, rarity);
            case "consumable":
                return GetRandomItem();
                //todo need sprite 
                //return new Item(itemTemplate, rarity);
            default:
                return new Item(itemTemplate, rarity);
        }
    }

    public List<Item> GetRandomItems(int numItems = 0)
    {
        if (numItems < 1)
        {
            numItems = Random.Range(1, 6);
        }

        var items = new List<Item>();

        for (var i = 0; i < numItems; i++)
        {
            items.Add(GetRandomItem());
        }

        return items;
    }

    //<Summary>
    //    Returns a random item of specified rarity 
    //</Summary>
    public Item GetRandomItemForRarity(ItemRarity rarity)
    {
        var itemTemplate = GetRandomBaseItemTemplate();

        switch (itemTemplate.Category.ToLower())
        {
            case "armor":
                return new Armor(itemTemplate, rarity);
            case "weapon":
                return new Weapon(itemTemplate, rarity);
            case "consumable":
                return GetRandomItemForRarity(rarity);
            //todo need sprite 
            //return new Item(itemTemplate, rarity);
            default:
                return new Item(itemTemplate, rarity);
        }
    }

    //<Summary>
    //    Gets prefab for item based on type 
    //</Summary>
    public GameObject GetWorldPrefabForItemByType(string type)
    {
        //todo update this when weapon components added

        Enum.TryParse(type, true, out ItemPrefabKeys prefabKey);

        return _itemPrefabsWorldView[prefabKey];
    }

    private Item CreateItemOfType(string itemType, ItemRarity rarity = ItemRarity.Common)
    {
        var itemTemplate = ItemTemplateLoader.GetItemTemplate(itemType);

        switch (itemTemplate.Category.ToLower())
        {
            case "armor":
                return new Armor(itemTemplate, rarity);
            case "weapon":
                return new Weapon(itemTemplate, rarity);
            case "consumable":
                return GetRandomItem();
            //todo need sprite 
            //return new Item(itemTemplate, rarity);
            default:
                return new Item(itemTemplate, rarity);
        }
    }

    //<Summary>
    //    Returns a random rarity 
    //</Summary>
    private static ItemRarity GetRandomRarity()
    {
        return GlobalHelper.GetRandomEnumValue<ItemRarity>();
    }

    //<Summary>
    //    Returns a random base item template 
    //</Summary>
    private ItemTemplate GetRandomBaseItemTemplate()
    {
        var itemType = _baseItemTemplateTypes[Random.Range(0, _baseItemTemplateTypes.Count)];

        return ItemTemplateLoader.GetItemTemplate(itemType);
    }

    //<Summary>
    //    Populates item prefab dictionaries 
    //</Summary>
    private void PopulatePrefabDictionaries()
    {
        //todo load inventory view prefabs
        _itemPrefabsWorldView = new Dictionary<ItemPrefabKeys, GameObject>();

        var index = 0;
        foreach (ItemPrefabKeys prefabKey in Enum.GetValues(typeof(ItemPrefabKeys)))
        {
            _itemPrefabsWorldView[prefabKey] = WorldData.Instance.ItemPrefabsWorldView[index];
            index++;
        }
    }
}
