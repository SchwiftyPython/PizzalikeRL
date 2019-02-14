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
        var rarity = GetRandomRarity<ItemRarity>();

        var itemTemplate = GetRandomBaseItemTemplate();
        
        switch (itemTemplate.Category.ToLower())
        {
            case "armor":
                return new Armor(itemTemplate, rarity);
            case "weapon":
                return new Weapon(itemTemplate, rarity);
            case "consumable":
                return new Item(itemTemplate, rarity);
            default:
                return new Item(itemTemplate, rarity);
        }
    }

    //<Summary>
    //    Returns a random rarity 
    //</Summary>
    private static T GetRandomRarity<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(Random.Range(0, values.Length));
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
