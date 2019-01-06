using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SaveGameData : MonoBehaviour
{
    [Serializable]
    public class SaveData
    {
        public string StartingSeed;
        public Random.State SeedState;

        public class SerializableMapDictionary : SerializableDictionary<string, CellSdo> { }

        public SerializableMapDictionary Map;

        public class SerializableEntitiesDictionary : SerializableDictionary<Guid, EntitySdo> { }

        public SerializableEntitiesDictionary Entities;
        public Guid PlayerId;

        public class SerializableItemDictionary : SerializableDictionary<Guid, ItemSdo> { }

        public SerializableItemDictionary Items;

        public string CurrentCellId;
        public string CurrentAreaId;
        public string CurrentTileId;

        public GameManager.GameState CurrentState;
        public string CurrentSceneName;

        public List<string> Messages;

        public class SerializableOrdersDictionary : SerializableDictionary<string, PizzaOrderSdo> { }

        public SerializableOrdersDictionary ActiveOrders;
    }

    [Serializable]
    public class SaveGameFileNames
    {
        public Dictionary<string, string> FileNames;
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

        Serializer = new SaveGameJsonSerializer(); 

        LoadSavedGamesFileInfo();
    }

    public void Save()
    {
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
                CurrentSceneName = GameManager.Instance.CurrentScene.ToString(),
                Messages = GameManager.Instance.Messages,
                ActiveOrders = ConvertActiveOrdersForSaving(GameManager.Instance.ActiveOrders)
            };

            var saveGameFileNames = new SaveGameFileNames();

            var fileName = $@"{WorldData.Instance.SaveGameId}";

            SaveFileNames.Add(fileName, GameManager.Instance.Player.Fluff.Name);

            saveGameFileNames.FileNames = SaveFileNames;

            SaveGame.Save(FileNamesIdentifier, saveGameFileNames, Serializer);

            SaveGame.Save(fileName, data, Serializer);
        }
        catch (Exception e)
        {
            Debug.Log("Error saving! " + e.Message);
            throw;
        }
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

        LoadGameButton.GetComponent<Button>().interactable = true;
    }

    private static SaveData.SerializableMapDictionary ConvertMapForSaving(Cell[,] map)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        var convertedCells = new SaveData.SerializableMapDictionary();

        for (var currentRow = 0; currentRow < height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < width; currentColumn++)
            {
                var currentCell = map[currentColumn, currentRow];

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
}
