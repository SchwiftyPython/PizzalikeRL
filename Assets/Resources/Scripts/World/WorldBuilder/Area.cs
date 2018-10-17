using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    private Dictionary<GameObject, Rarities> _biomeTypeTiles;

    public List<Entity> PresentEntities { get; set; }
    public List<Faction> PresentFactions { get; set; }

    public int Width = 80;
    public int Height = 25;

    //TODO: Could probably just reference parent cell for BiomeType
    public BiomeType BiomeType { get; set; }

    public Cell ParentCell { get; set; }
    public Tile[,] AreaTiles { get; set; }
    public Queue<Entity> TurnOrder { get; set; }

    public Settlement Settlement;

    private int _x;

    public int X
    {
        get { return _x; }

        set { _x = value; }
    }

    private int _y;

    public int Y
    {
        get { return _y; }

        set { _y = value; }
    }

    public void BuildArea()
    {
        if (AreaTiles != null)
        {
            return;
        }
        const int maxNpcsPlacedAtOnce = 9;
        PresentEntities = new List<Entity>();
        //PresentFactions = new List<Faction>();
        AreaTiles = new Tile[Width, Height];

        var tileDeck = new AreaTileDeck(BiomeType);

        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var texture = tileDeck.Draw();
                if (texture.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), true, true);
                }
                else
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), false, false);
                }
                AreaTiles[i, j].Visibility = Tile.Visibilities.Invisible;
            }
        }
        
        if (Settlement == null)
        {
            return;
        }

        var settlementPrefab = SettlementPrefabStore.GetSettlementPrefab(Settlement.Size);

        SettlementPrefabStore.AssignBuildingToLots(settlementPrefab);

        Settlement.Lots = settlementPrefab.Lots;

//        var settlementBluePrint = SettlementPrefabStore.Rotate180(settlementPrefab.Blueprint);

        var settlementBluePrint = settlementPrefab.Blueprint;

        for (var currentRow = 0; currentRow < settlementBluePrint.GetLength(1); currentRow++)
        {
            for (var currentColumn = 0; currentColumn < settlementBluePrint.GetLength(0); currentColumn++)
            {
                var tileCode = settlementBluePrint[currentColumn, currentRow];

                if (tileCode == 'x')
                {
                    continue;
                }
                if (tileCode == SettlementPrefabStore.LotKey)
                {
                    continue;
                }

                //Debug.Log($"1: {settlementBluePrint.GetLength(1)}  0: {settlementBluePrint.GetLength(0)}");
                //Debug.Log($"x: {currentRow}  y: {currentColumn}");
                //Debug.Log($"tilecode: {tileCode}");

                var tile = GetTilePrefab(tileCode);
              
                AreaTiles[currentRow, currentColumn] = new Tile(tile, new Vector2(currentRow, currentColumn), false, false);
            }
        }

        if (PresentFactions == null)
        {
            return;
        }

        foreach (var faction in PresentFactions)
        {
            faction.Leader.CurrentArea = this;
            faction.Leader.CurrentCell = ParentCell;

            var numNpcsToPlace = Random.Range(1, maxNpcsPlacedAtOnce + 1);

            for (var k = 0; k < numNpcsToPlace; k++)
            {
                PresentEntities.Add(k != numNpcsToPlace - 1
                    ? new Entity(faction.EntityType, faction.Type) {CurrentArea = this, CurrentCell = ParentCell}
                    : faction.Leader);
            }
        }
    }

    public bool EntitiesPresent()
    {
        return PresentEntities.Count > 0;
    }

    public bool AreaBuilt()
    {
        return AreaTiles != null;
    }

    public Tile GetTileAt(Vector3 position)
    {
        return AreaTiles[(int) position.x, (int) position.y];
    }

    private GameObject GetTilePrefab(char tileCode)
    {
        var tileType = GetTileType(tileCode);

        switch (BiomeType)
        {
            case BiomeType.Grassland:
                return SettlementPrefabStore.GrassDirtPathTiles[tileType];
            case BiomeType.Desert:
                return SettlementPrefabStore.DesertAsphaltRoadTiles[tileType];
            default:
                return SettlementPrefabStore.GrassDirtPathTiles[tileType];
        }
    }

    private string GetTileType(char tileCode)
    {
        switch (BiomeType)
        {
            case BiomeType.Grassland:
                return SettlementPrefabStore.GrassDirtPathTileKeys[tileCode];
            case BiomeType.Desert:
                return SettlementPrefabStore.DesertAsphaltRoadTileKeys[tileCode];
            default:
                return SettlementPrefabStore.GrassDirtPathTileKeys[tileCode];
        }
    }

    private void PlaceWaterTiles()
    {
        var waterTiles = GetWaterTiles();


    }

    private IDictionary<string, GameObject> GetWaterTiles()
    {
        switch (BiomeType)
        {
            case BiomeType.Grassland:
                return
                    (Dictionary<string, GameObject>) PopulateWaterTileDictionary(WorldData.Instance.GrassWaterTiles);
            case BiomeType.Desert:
                return
                    (Dictionary<string, GameObject>) PopulateWaterTileDictionary(WorldData.Instance.DesertWaterTiles);
            default:
                return
                    (Dictionary<string, GameObject>) PopulateWaterTileDictionary(WorldData.Instance.GrassWaterTiles);
        }
    }

    private IDictionary<string, GameObject> PopulateWaterTileDictionary(IReadOnlyList<GameObject> waterTilePrefabs)
    {
        var waterTiles = new Dictionary<string, GameObject>
        {
            { "center", null },
            { "lower_left", null },
            { "lower_right", null },
            { "upper_left", null },
            { "upper_right", null },
            { "horizontal_bottom", null },
            { "horizontal_top", null },
            { "vertical_left", null },
            { "vertical_right", null }
        };

        var waterTileKeys = new List<string>
        {
            "center",
            "lower_left",
            "lower_right",
            "upper_left",
            "upper_right",
            "horizontal_bottom",
            "horizontal_top",
            "vertical_left",
            "vertical_right"
        };

        for (var i = 0; i < waterTiles.Count; i++)
        {
            waterTiles[waterTileKeys[i]] = waterTilePrefabs[i];
        }

        return waterTiles;
    }
}
