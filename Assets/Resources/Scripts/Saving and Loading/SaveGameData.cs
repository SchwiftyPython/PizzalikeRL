using System;
using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SaveGameData : MonoBehaviour
{
    [Serializable]
    public class SaveData
    {
        public string StartingSeed;
        public Random.State SeedState;

        [Serializable]
        public class SerializableMapDictionary : SerializableDictionary<string, CellSdo> { }
       
        public SerializableMapDictionary Map;

        [Serializable]
        public class SerializableEntitiesDictionary : SerializableDictionary<Guid, EntitySdo> { }

        public SerializableEntitiesDictionary EntitySdos;
        public Guid PlayerId;

        [Serializable]
        public class SerializableItemDictionary : SerializableDictionary<Guid, ItemSdo> { }

        public SerializableItemDictionary Items;

        public List<FactionSdo> FactionSdos;

        public string CurrentCellId;
        public string CurrentAreaId;
        public string CurrentTileId;

        public GameManager.GameState CurrentState;
        public string CurrentSceneName;

        public List<string> Messages;

        [Serializable]
        public class SerializableOrdersDictionary : SerializableDictionary<string, PizzaOrderSdo> { }

        public SerializableOrdersDictionary ActiveOrders;

        public string PlayerStartingPlaceCellId;

        public CameraPosition CurrentCameraPosition;
        public SerializableVector3 CameraVector;
    }

    [Serializable]
    public class SaveGameFileNames
    {
        [Serializable]
        public class SerializableFileNamesDictionary : SerializableDictionary<string, string> { }

        public SerializableFileNamesDictionary FileNames;
    }
    
    public Dictionary<string, string> SaveFileNames;
    public string FileNamesIdentifier = "savedatafiles";
    public ISaveGameSerializer Serializer;

    public GameObject LoadGameButton;

    public static SaveGameData Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        Serializer = new SaveGameBinarySerializer(); 

        LoadSavedGamesFileInfo();
    }

    //todo make save game file right after world gen
    public void Save()
    {
        Debug.Log("Saving...");

        try
        {
            var data = new SaveData
            {
                StartingSeed = WorldData.Instance.Seed,
                SeedState = Random.state,
                Map = ConvertMapForSaving(WorldData.Instance.Map),
                PlayerId = GameManager.Instance.Player.Id,
                CurrentCellId = GameManager.Instance.CurrentCell.Id,
                CurrentAreaId = GameManager.Instance.CurrentArea.Id,
                CurrentTileId = GameManager.Instance.CurrentTile.Id,
                CurrentState = GameManager.Instance.CurrentState,
                CurrentSceneName = GameManager.Instance.CurrentScene.name,
                Messages = Messenger.Instance.GetAllMessages(),
                ActiveOrders = ConvertActiveOrdersForSaving(GameManager.Instance.ActiveOrders),
                EntitySdos = EntitySdo.ConvertToEntitySdos(WorldData.Instance.Entities.Values.ToList()),
                FactionSdos = FactionSdo.ConvertToFactionSdos(WorldData.Instance.Factions.Values.ToList()),
                Items = ConvertItemsForSaving(WorldData.Instance.Items),
                PlayerStartingPlaceCellId = WorldData.Instance.PlayerStartingPlace.Id,
                CurrentCameraPosition = GameManager.Instance.CurrentCameraPosition,
                CameraVector = new SerializableVector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, Camera.main.transform.localPosition.z)
            };

            Debug.Log($@"Camera enum before save: {data.CurrentCameraPosition}");
            Debug.Log($@"Camera position before save: {data.CameraVector.X}, {data.CameraVector.Y}, {data.CameraVector.Z}");

            var saveGameFileNames =
                new SaveGameFileNames {FileNames = new SaveGameFileNames.SerializableFileNamesDictionary()};

            var fileName = $@"{WorldData.Instance.SaveGameId}";

            SaveFileNames.Add(fileName, GameManager.Instance.Player.Fluff.Name);

            foreach (var saveFileName in SaveFileNames.Keys)
            {
                saveGameFileNames.FileNames.Add(saveFileName, SaveFileNames[saveFileName]);
            }

            SaveGame.Save(FileNamesIdentifier, saveGameFileNames, Serializer);

            SaveGame.Save(fileName, data, Serializer);
        }
        catch (Exception e)
        {
            Debug.Log("Error saving! " + e.Message);
            throw;
        }
    }

    public void Load(string fileName)
    {
        Debug.Log("Loading...");

        var saveData = SaveGame.Load<SaveData>(fileName, Serializer);

        SceneManager.LoadScene(saveData.CurrentSceneName);

        InitializeMap();

        WorldData.Instance.Items = ConvertItemsForPlaying(saveData.Items);

        WorldData.Instance.Entities = ConvertEntitiesForPlaying(saveData.EntitySdos);

        WorldData.Instance.Factions = ConvertFactionsForPlaying(saveData.FactionSdos);

        WorldData.Instance.Map = ConvertMapForPlaying(saveData.Map);

        WorldData.Instance.Seed = saveData.StartingSeed;
        Random.state = saveData.SeedState;

        WorldData.Instance.SaveGameId = fileName;

        WorldData.Instance.PlayerStartingPlace = WorldData.Instance.MapDictionary[saveData.PlayerStartingPlaceCellId];

        GameManager.Instance.Player = WorldData.Instance.Entities[saveData.PlayerId];

        GameManager.Instance.CurrentCell = GameManager.Instance.Player.CurrentCell;

        GameManager.Instance.CurrentArea = GameManager.Instance.Player.CurrentArea;

        GameManager.Instance.CurrentTile = GameManager.Instance.Player.CurrentTile;

        GameManager.Instance.ActiveOrders = ConvertActiveOrdersForPlaying(saveData.ActiveOrders);

        GameManager.Instance.CurrentState = saveData.CurrentSceneName == GameManager.AreaMapSceneName
            ? GameManager.GameState.EnterArea
            : saveData.CurrentState;

        Messenger.Instance.LoadMessages(saveData.Messages);

        // GameManager.Instance.CurrentCameraPosition = saveData.CurrentCameraPosition;
        //
        // Debug.Log($@"Camera enum after load: {GameManager.Instance.CurrentCameraPosition}");
        //
        // Camera.main.transform.localPosition = new Vector3(saveData.CameraVector.X, saveData.CameraVector.Y, saveData.CameraVector.Z);
        //
        // Debug.Log($@"Camera position after load: {saveData.CameraVector.X}, {saveData.CameraVector.Y}, {saveData.CameraVector.Z}");
    }

    private void LoadSavedGamesFileInfo()
    {
        if (SaveFileNames == null)
        {
            SaveFileNames = new Dictionary<string, string>();
        }

        var fileNames = SaveGame.Load<SaveGameFileNames>(FileNamesIdentifier, Serializer);

        if (fileNames == null || fileNames.FileNames.Count < 1)
        {
            LoadGameButton.GetComponent<Button>().interactable = false;
            return;
        }

        foreach (var fileName in fileNames.FileNames.Keys)
        {
            SaveFileNames.Add(fileName, fileNames.FileNames[fileName]);
        }

        LoadGameButton.GetComponent<Button>().interactable = true;
    }

    public static void InitializeMap()
    {
        var map = new Cell[WorldData.Instance.Height, WorldData.Instance.Width];

        for (var currentRow = 0; currentRow < WorldData.Instance.Height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < WorldData.Instance.Width; currentColumn++)
            {
                map[currentRow, currentColumn] = new Cell
                {
                    X = currentRow,
                    Y = currentColumn,
                    Id = currentRow + " " + currentColumn
                };

//                WorldData.Instance.MapDictionary.Add(WorldData.Instance.Map[currentRow, currentColumn].Id,
//                    WorldData.Instance.Map[currentRow, currentColumn]);
            }
        }

        WorldData.Instance.Map = map;
    }

    private static Cell[,] ConvertMapForPlaying(SaveData.SerializableMapDictionary savedMap)
    {
        var tempMap = WorldData.Instance.Map;

        foreach (var cellSdo in savedMap)
        {
            var x = cellSdo.Value.X;
            var y = cellSdo.Value.Y;

            tempMap[x, y] = CellSdo.ConvertToCell(cellSdo.Value);

            if (WorldData.Instance.MapDictionary.ContainsKey(cellSdo.Key))
            {
                WorldData.Instance.MapDictionary[cellSdo.Key] = tempMap[x, y];
            }
            else
            {
                WorldData.Instance.MapDictionary.Add(cellSdo.Key, tempMap[x, y]);
            }
        }

        return tempMap;
    }

    private static SaveData.SerializableMapDictionary ConvertMapForSaving(Cell[,] map)
    {
        var height = map.GetLength(0);
        var width = map.GetLength(1);

        var convertedCells = new SaveData.SerializableMapDictionary();

        for (var currentRow = 0; currentRow < height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < width; currentColumn++)
            {
                var currentCell = map[currentRow, currentColumn];

                var tempSdo = CellSdo.ConvertToCellSdo(currentCell);

                convertedCells.Add(currentCell.Id, tempSdo); 
            }
        }

        return convertedCells;
    }

    private static SaveData.SerializableOrdersDictionary ConvertActiveOrdersForSaving(
        Dictionary<string, PizzaOrder> activeOrders)
    {
        var convertedOrders = new SaveData.SerializableOrdersDictionary();

        foreach (var order in activeOrders.Keys)
        {
            if (convertedOrders.ContainsKey(order))
            {
                Debug.Log("Order " + order + " already exists in converted orders!");
                continue;
            }

            convertedOrders.Add(order, PizzaOrderSdo.ConvertToPizzaOrderSdo(activeOrders[order]));
        }

        return convertedOrders;
    }

    private static Dictionary<string, PizzaOrder> ConvertActiveOrdersForPlaying(
        SaveData.SerializableOrdersDictionary orderSdos)
    {
        var activeOrders = new Dictionary<string, PizzaOrder>();

        foreach (var sdo in orderSdos)
        {
            if (activeOrders.ContainsKey(sdo.Key))
            {
                continue;
            }

            var order = PizzaOrderSdo.ConvertToPizzaOrder(sdo.Value);

            activeOrders.Add(sdo.Key, order);
        }

        return activeOrders;
    }

    private static Dictionary<Guid, Entity> ConvertEntitiesForPlaying(SaveData.SerializableEntitiesDictionary entitySdos)
    {
        return EntitySdo.ConvertToEntities(entitySdos);
    }

    private static Dictionary<Guid, Item> ConvertItemsForPlaying(SaveData.SerializableItemDictionary itemSdos)
    {
        var items = new Dictionary<Guid, Item>();
        foreach (var sdo in itemSdos)
        {
            Item item;

            switch (sdo.Value.ItemCategory.ToLower())
            {
                case "armor":
                    item = ArmorSdo.ConvertToArmor((ArmorSdo) sdo.Value);
                    break;
                case "weapon":
                    item = WeaponSdo.ConvertToWeapon((WeaponSdo) sdo.Value);
                    break;
                case "consumable":
                    throw new NotImplementedException();
                default:
                    Debug.Log($@"Item Category {sdo.Value.ItemCategory.ToLower()} not found. Converting to Item.");
                    item = ItemSdo.ConvertToItem(sdo.Value);
                    break;
            }

            items.Add(item.Id, item);
        }

        return items;
    }

    private static SaveData.SerializableItemDictionary ConvertItemsForSaving(Dictionary<Guid, Item> items)
    {
        if (items == null)
        {
            return null;
        }

        var sdos = new SaveData.SerializableItemDictionary();

        foreach (var item in items)
        {
            try
            {
                ItemSdo sdo;
                switch (item.Value.ItemCategory.ToLower())
                {
                    case "armor":
                        sdo = ArmorSdo.ConvertToArmorSdo((Armor) item.Value);
                        break;
                    case "weapon":
                        sdo = WeaponSdo.ConvertToWeaponSdo((Weapon) item.Value);
                        break;
                    case "consumable":
                        throw new NotImplementedException();
                    default:
                        Debug.Log($@"Item Category {item.Value.ItemCategory.ToLower()} not found. Converting to ItemSdo.");
                        sdo = ItemSdo.ConvertToItemSdo(item.Value);
                        break;
                }

                sdos.Add(sdo.Id, sdo);
            }
            catch (Exception e)
            {
                Debug.Log($"Error converting {item.Value.ItemName}: {e.Message}");
            }
        }

        return sdos;
    }

    private Dictionary<string, Faction> ConvertFactionsForPlaying(IEnumerable<FactionSdo> factionSdos)
    {
        var factions = new Dictionary<string, Faction>();

        foreach (var sdo in factionSdos)
        {
            if (factions.ContainsKey(sdo.Name))
            {
                continue;
            }

            var faction  = new Faction(sdo);

            factions.Add(faction.Name, faction);
        }

        return factions;
    }
}
