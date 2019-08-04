using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ItemPrefabKeys
{
    CowboyHat,
    EnergyArmor,
    FlakArmor,
    BallisticHelmet,
    LeatherArmor,
    PlateArmor,
    Robes,
    TrenchCoat,
    Tuque,
    AssaultRifle,
    BattleAxe,
    BoltActionRifle,
    Club,
    FoldingChair,
    FragGrenade,
    GreatBow,
    GreatSword,
    EmpGrenade,
    HandAxe,
    Knife,
    LaserRifle,
    RocketLauncher,
    LightMachineGun,
    Revolver,
    SemiAutoHandgun,
    ShortSword,
    Shotgun,
    SniperRifle,
    Spear,
    Submachinegun,
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
        _baseItemTemplateTypes = ItemTemplateLoader.GetEntityTemplateTypes();
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

        switch (type.ToLower())
        {
            case "helmet":
                return _itemPrefabsWorldView[ItemPrefabKeys.BallisticHelmet];
            case "sword":
                return _itemPrefabsWorldView[ItemPrefabKeys.ShortSword];
            case "bow":
                return _itemPrefabsWorldView[ItemPrefabKeys.GreatBow];
            default:
                return null;
        }
    }

    //<Summary>
    //    Returns a random rarity 
    //</Summary>
    private static ItemRarity GetRandomRarity()
    {
        return DebugHelper.Instance.GetRandomEnumValue<ItemRarity>();
    }

    //<Summary>
    //    Returns a random base item template 
    //</Summary>
    private ItemTemplate GetRandomBaseItemTemplate()
    {
        var itemType = _baseItemTemplateTypes[Random.Range(0, _baseItemTemplateTypes.Count)];

        return ItemTemplateLoader.GetEntityTemplate(itemType);
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
