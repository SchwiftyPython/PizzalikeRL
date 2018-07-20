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
            }
        }
        
        if (Settlement == null)
        {
            return;
        }

        var settlementPrefab = SettlementPrefabStore.GetSettlementPrefab(Settlement.Size);

        var settlementBluePrint = SettlementPrefabStore.Rotate180(settlementPrefab.Blueprint);

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
                    //get lot dimensions
                    //pick building
                    continue;
                }

                //Debug.Log($"1: {settlementBluePrint.GetLength(1)}  0: {settlementBluePrint.GetLength(0)}");
                Debug.Log($"x: {currentRow}  y: {currentColumn}");
                Debug.Log($"tilecode: {tileCode}");
                var tileType = SettlementPrefabStore.PathTileKeys[tileCode];

                //testing
                var tile = SettlementPrefabStore.GrassDirtPathTiles[tileType];
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
}
