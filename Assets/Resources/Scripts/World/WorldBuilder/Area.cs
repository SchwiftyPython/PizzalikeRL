using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Area
{
    private Dictionary<GameObject, Rarities> _biomeTypeTiles;

    private Dictionary<string, GameObject> _waterTiles;

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

        if (Settlement != null)
        {
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

                    AreaTiles[currentRow, currentColumn] =
                        new Tile(tile, new Vector2(currentRow, currentColumn), false, false);
                }
            }
        }

        UpdateNeighbors();

        if (ParentCell.Rivers.Count > 0)
        {
            PlaceWaterTiles();
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
        const int maxTries = 3;

        _waterTiles = GetWaterTiles();

        var foundStartingPoint = false;
        var numTries = 0;
        Tile startTile = null;
        while (!foundStartingPoint && numTries < maxTries)
        {
            var x = Random.Range(0, Width);
            var y = Random.Range(0, Height);
            
            startTile = AreaTiles[x, y];

            if (CanPlaceWaterTile(startTile))
            {
                foundStartingPoint = true;
            }
            else
            {
                numTries++;
            }
        }

        if (numTries > maxTries || startTile == null)
        {
            return;
        }

        var waterWidth = Random.Range(0, Width);
        var waterHeight = Random.Range(0, Height);

        var tempMap = new Tile[Height, Width];

        for (var currentRow = (int) startTile.GridPosition.y; currentRow < waterHeight; currentRow++)
        {
            for (var currentColumn = (int) startTile.GridPosition.x; currentColumn < waterWidth; currentColumn++)
            {
                var currentTile = tempMap[currentRow, currentColumn];

                if (CanPlaceWaterTile(currentTile))
                {
                    var waterTilePrefab = GetCorrectWaterTilePrefab(currentTile);
                }
            }
        }


    }

    private bool CanPlaceWaterTile(Tile tile)
    {
        return Settlement == null ||
               Settlement.Lots.All(lot => !lot.IsPartOfLot(new Vector2(tile.GridPosition.x, tile.GridPosition.y)));
    }

    private Dictionary<string, GameObject> GetWaterTiles()
    {
        switch (BiomeType)
        {
            case BiomeType.Grassland:
                return
                    PopulateWaterTileDictionary(WorldData.Instance.GrassWaterTiles);
            case BiomeType.Desert:
                return
                    PopulateWaterTileDictionary(WorldData.Instance.DesertWaterTiles);
            default:
                return
                    PopulateWaterTileDictionary(WorldData.Instance.GrassWaterTiles);
        }
    }

    private GameObject GetCorrectWaterTilePrefab(Tile tile)
    {
        if (tile.Left == null)
        {
            if (tile.Top == null)
            {
                return _waterTiles["upper_left"];
            }
        }
    }

    private Dictionary<string, GameObject> PopulateWaterTileDictionary(IReadOnlyList<GameObject> waterTilePrefabs)
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

    private Tile GetTop(Tile t)
    {
        return AreaTiles[(int) t.GridPosition.x, MathHelper.Mod((int) (t.GridPosition.y - 1), Height)];
    }
    private Tile GetBottom(Tile t)
    {
        return AreaTiles[(int) t.GridPosition.x, MathHelper.Mod((int) (t.GridPosition.y + 1), Height)];
    }
    private Tile GetLeft(Tile t)
    {
        return AreaTiles[MathHelper.Mod((int) (t.GridPosition.x - 1), Width), (int) t.GridPosition.y];
    }
    private Tile GetRight(Tile t)
    {
        return AreaTiles[MathHelper.Mod((int) (t.GridPosition.x + 1), Width), (int) t.GridPosition.y];
    }

    private void UpdateNeighbors()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var c = AreaTiles[x, y];

                c.Top = GetTop(c);
                c.Bottom = GetBottom(c);
                c.Left = GetLeft(c);
                c.Right = GetRight(c);
            }
        }
    }
}
