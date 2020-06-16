using System;
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
    // [Description("Turret Barrel")] //todo need to implement
    // TurretBarrel,
    [Description("Warhammer")]
    Warhammer
}

public class ItemStore : MonoBehaviour
{
    private Dictionary<ItemPrefabKeys, GameObject> _itemPrefabsWorldView;
    private List<string> _baseItemTemplateTypes;

    // many of these are 1:1 now, but that can change
    private readonly Dictionary<EquipmentSlotType, List<Entity.EquipmentSlot>> _slotDictionary = new Dictionary<EquipmentSlotType, List<Entity.EquipmentSlot>>
    {
        {EquipmentSlotType.Head, new List<Entity.EquipmentSlot> {Entity.EquipmentSlot.Head}},
        {EquipmentSlotType.Body, new List<Entity.EquipmentSlot> {Entity.EquipmentSlot.Body}},
        {EquipmentSlotType.Hand, new List<Entity.EquipmentSlot> {Entity.EquipmentSlot.LeftHandOne, Entity.EquipmentSlot.RightHandOne}},
        {EquipmentSlotType.Missile, new List<Entity.EquipmentSlot> {Entity.EquipmentSlot.MissileWeaponOne}},
        {EquipmentSlotType.Thrown, new List<Entity.EquipmentSlot> {Entity.EquipmentSlot.Thrown}},
        {EquipmentSlotType.Special, new List<Entity.EquipmentSlot> {Entity.EquipmentSlot.Special}},
        {EquipmentSlotType.Consumable, new List<Entity.EquipmentSlot> {Entity.EquipmentSlot.Consumable}},
    };

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

        return GetItemFromTemplate(itemTemplate);
    }

    public List<Item> GetRandomItems(int numItems = 0, ItemRarity rarityCap = ItemRarity.Common)
    {
        if (numItems < 1)
        {
            numItems = Random.Range(1, 6);
        }

        var items = new List<Item>();

        for (var i = 0; i < numItems; i++)
        {
            if (rarityCap == ItemRarity.Legendary)
            {
                items.Add(GetRandomItem());
            }
            else if(rarityCap == ItemRarity.Common)
            {
                items.Add(GetRandomItemForRarity(ItemRarity.Common));
            }
            else
            {
                const int maxTries = 5;

                var rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();

                var currentTry = 1;
                while (currentTry <= maxTries && rarity > rarityCap)
                {
                    rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();
                    currentTry++;
                }

                items.Add(GetRandomItemForRarity(rarity));
            }
            
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

    public Weapon GetRandomWeaponByProperty(string property, ItemRarity rarityCap = ItemRarity.Common)
    {
        var weaponTemplatesWithProperty = GetWeaponTemplateByProperty(property);

        var randomTemplate = weaponTemplatesWithProperty[Random.Range(0, weaponTemplatesWithProperty.Count)];

        if (rarityCap == ItemRarity.Common)
        {
            return new Weapon(randomTemplate, rarityCap);
        }

        const int maxTries = 6;

        var rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();

        var currentTry = 1;
        while (currentTry <= maxTries && rarity > rarityCap)
        {
            rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();
            currentTry++;
        }

        return new Weapon(randomTemplate, rarity);
    }

    public Item GetRandomItemByEquipmentSlot(Entity.EquipmentSlot slot, ItemRarity rarityCap = ItemRarity.Common)
    {
        var templatesForSlot = new List<ItemTemplate>();

        foreach (var templateType in _baseItemTemplateTypes)
        {
            var currentTemplate = ItemTemplateLoader.GetItemTemplate(templateType);

            var validSlots = GetEquipmentSlotsForSlotType(currentTemplate.EquipmentSlotType);

            if (validSlots.Contains(slot))
            {
                templatesForSlot.Add(currentTemplate);
            }
        }

        if (templatesForSlot.Count < 1)
        {
            return null;
        }

        var itemTemplate = templatesForSlot[Random.Range(0, templatesForSlot.Count)];

        if (rarityCap == ItemRarity.Common)
        {
            return GetItemFromTemplate(itemTemplate, rarityCap);
        }

        const int maxTries = 2;

        var rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();

        var currentTry = 1;
        while (currentTry <= maxTries && rarity > rarityCap)
        {
            rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();
            currentTry++;
        }

        return GetItemFromTemplate(itemTemplate, rarity);
    }

    public Weapon GetRandomRangedWeapon(ItemRarity rarityCap = ItemRarity.Common)
    {
        var rangedTemplates = new List<ItemTemplate>();

        foreach (var templateType in _baseItemTemplateTypes)
        {
            var currentTemplate = ItemTemplateLoader.GetItemTemplate(templateType);

            if (currentTemplate.Category == "weapon" &&
                currentTemplate.EquipmentSlotType == EquipmentSlotType.Missile ||
                currentTemplate.EquipmentSlotType == EquipmentSlotType.Thrown)
            {
                rangedTemplates.Add(currentTemplate);
            }
        }

        var weaponTemplate = rangedTemplates[Random.Range(0, rangedTemplates.Count)];

        if (rarityCap == ItemRarity.Common)
        {
            return new Weapon(weaponTemplate, rarityCap);
        }

        const int maxTries = 3;

        var rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();

        var currentTry = 1;
        while (currentTry <= maxTries && rarity > rarityCap)
        {
            rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();
            currentTry++;
        }

        return new Weapon(weaponTemplate, rarity);
    }

    public Weapon GetRandomThrownWeapon(ItemRarity rarityCap = ItemRarity.Common)
    {
        var thrownTemplates = new List<ItemTemplate>();

        foreach (var templateType in _baseItemTemplateTypes)
        {
            var currentTemplate = ItemTemplateLoader.GetItemTemplate(templateType);

            if (currentTemplate.Category == "weapon" &&
                currentTemplate.EquipmentSlotType == EquipmentSlotType.Thrown)
            {
                thrownTemplates.Add(currentTemplate);
            }
        }

        var weaponTemplate = thrownTemplates[Random.Range(0, thrownTemplates.Count)];

        if (rarityCap == ItemRarity.Common)
        {
            return new Weapon(weaponTemplate, rarityCap);
        }

        const int maxTries = 6;

        var rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();

        var currentTry = 1;
        while (currentTry <= maxTries && rarity > rarityCap)
        {
            rarity = GlobalHelper.GetRandomEnumValue<ItemRarity>();
            currentTry++;
        }

        return new Weapon(weaponTemplate, rarity);
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

    public string GetDisplayNameForItemType(string itemType)
    {
        Enum.TryParse(itemType, true, out ItemPrefabKeys prefabKey);

        return prefabKey.ToString();
    }

    public List<Entity.EquipmentSlot> GetEquipmentSlotsForSlotType(EquipmentSlotType slotType)
    {
        if (!_slotDictionary.ContainsKey(slotType))
        {
            Debug.Log($@"Invalid Equipment Slot Type: {slotType}");
            return null;
        }

        return _slotDictionary[slotType];
    }

    private Item CreateItemOfType(string itemType, ItemRarity rarity = ItemRarity.Common)
    {
        var itemTemplate = ItemTemplateLoader.GetItemTemplate(itemType);

        return GetItemFromTemplate(itemTemplate, rarity);
    }

    private Item GetItemFromTemplate(ItemTemplate template, ItemRarity rarity = ItemRarity.Common)
    {
        switch (template.Category.ToLower())
        {
            case "armor":
                return new Armor(template, rarity);
            case "weapon":
                return new Weapon(template, rarity);
            case "consumable":
                return GetRandomItem();
            //todo need sprite 
            //return new Item(itemTemplate, rarity);
            default:
                return new Item(template, rarity);
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

    private List<ItemTemplate> GetWeaponTemplateByProperty(string property)
    {
        var templatesForProperty = new List<ItemTemplate>();

        foreach (var templateType in _baseItemTemplateTypes)
        {
            var template = ItemTemplateLoader.GetItemTemplate(templateType);

            if (template.Category == "weapon" &&
                template.Properties.Contains(property))
            {
                templatesForProperty.Add(template);
            }
        }

        return templatesForProperty;
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
