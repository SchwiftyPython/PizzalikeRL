using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using UnityEngine;
using Random = UnityEngine.Random;

public class SaveGameData : MonoBehaviour
{
    [Serializable]
    public class SaveData
    {
        public string StartingSeed;
        public Random.State SeedState;

        public CellSdo[] Map;
        public Guid PlayerId;

        public string CurrentCellId;
        public string CurrentAreaId;
        public string CurrentTileId;

        public GameManager.GameState CurrentState;
        public string CurrentSceneName;

        public List<string> Messages;

        public Dictionary<string, PizzaOrder> ActiveOrders;
    }

    public SaveData Data;
    public string Identifier = "savedata";
    public ISaveGameSerializer Serializer;

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

        //todo load game files
    }

    public void Save()
    {
        try
        {
            Data = new SaveData
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
                //Messages = GameManager.Instance.Messages,
                //ActiveOrders = GameManager.Instance.ActiveOrders
            };

            SaveGame.Save(Identifier, Data, Serializer);
        }
        catch (Exception e)
        {
            Debug.Log("Error saving! " + e.Message);
            throw;
        }
    }

    private static CellSdo[] ConvertMapForSaving(Cell[,] map)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        var convertedCells = new CellSdo[width, height];

        for (var currentRow = 0; currentRow < height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < width; currentColumn++)
            {
                var currentCell = map[currentColumn, currentRow];

                var tempSdo = CellSdo.ConvertToCellSdo(currentCell);

                convertedCells[currentColumn, currentRow] = tempSdo;
            }
        }

        Debug.Log("Map cells converted to cellSdos.");

        var index = 0;
        var single = new CellSdo[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                single[index] = convertedCells[x, y];
                index++;
            }
        }
        return single;
    }
}
