using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

        for (var row = 0; row < Height; row++)
        {
            for (var column = 0; column < Width; column++)
            {
                var texture = tileDeck.Draw();
                if (texture.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    AreaTiles[column, row] = new Tile(texture, new Vector2(column, row), true, true);
                }
                else
                {
                    AreaTiles[column , row] = new Tile(texture, new Vector2(column, row), false, false);
                }
                AreaTiles[column, row].Visibility = Tile.Visibilities.Invisible;
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

        if (ParentCell.Rivers.Count > 0 || BiomeType == BiomeType.Swamp)
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
            case BiomeType.Ice:
                return SettlementPrefabStore.IceAsphaltRoadTiles[tileType];
            case BiomeType.Swamp:
                return SettlementPrefabStore.SwampDirtPathTiles[tileType];
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
            case BiomeType.Ice:
                return SettlementPrefabStore.IceAsphaltRoadTileKeys[tileCode];
            case BiomeType.Swamp:
                return SettlementPrefabStore.SwampDirtPathTileKeys[tileCode];
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

        var maxWaterHeight = Random.Range(3, Height / 2);
        var maxWaterWidth = Random.Range(3, Width / 2);

        var tempMap = new Tile[Width, Height];
        var currentHeight = 0;
        var success = true;
        for (var currentRow = (int) startTile.GridPosition.y; currentHeight < maxWaterHeight; currentRow++)
        {
            var currentWidth = 0;
            for (var currentColumn = (int) startTile.GridPosition.x; currentWidth < maxWaterWidth; currentColumn++)
            {
                if (currentRow >= Height || currentColumn >= Width)
                {
                    success = false;
                    break;
                }

                var currentTile = tempMap[currentColumn, currentRow] ?? new Tile(null, new Vector2(currentColumn, currentRow), false, false);

                UpdateNeighborsForTempTile(currentTile, tempMap);

                if (CanPlaceWaterTile(currentTile))
                {
                    GameObject waterTilePrefab;
                    try
                    {
                        waterTilePrefab = GetCorrectWaterTilePrefab(currentTile, currentWidth, currentHeight,
                            maxWaterWidth, maxWaterHeight);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        success = false;
                        break;
                    }
                    if (waterTilePrefab == null)
                    {
                        success = false;
                        break;
                    }

                    tempMap[currentColumn, currentRow] = new Tile(waterTilePrefab, new Vector2(currentColumn, currentRow), false, false);
                }
                currentWidth++;
            }
            currentHeight++;
        }

        if (!success)
        {
            return;
        }

        //tempMap = Rotate180(tempMap);

        currentHeight = 0;
        for (var currentRow = (int) startTile.GridPosition.y; currentHeight < maxWaterHeight; currentRow++)
        {
            var currentWidth = 0;
            for (var currentColumn = (int) startTile.GridPosition.x; currentWidth < maxWaterWidth; currentColumn++)
            {
                AreaTiles[currentColumn, currentRow] = tempMap[currentColumn, currentRow];
                currentWidth++;
            }
            currentHeight++;
        }
        
        Debug.Log($"Water placed in cell {ParentCell.X}, {ParentCell.Y}");
    }

    private bool CanPlaceWaterTile(Tile tile)
    {
        return !tile.GetBlocksMovement() || Settlement == null ||
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
            case BiomeType.Swamp:
                return
                    PopulateWaterTileDictionary(WorldData.Instance.SwampWaterTiles);
            case BiomeType.Ice:
                return
                    PopulateWaterTileDictionary(WorldData.Instance.IceWaterTiles);
            case BiomeType.Wasteland:
                return
                PopulateWaterTileDictionary(WorldData.Instance.WastelandWaterTiles);
            default:
                return
                    PopulateWaterTileDictionary(WorldData.Instance.GrassWaterTiles);
        }
    }

    private GameObject GetCorrectWaterTilePrefab(Tile tile, int currentWidth, int currentHeight, int maxWaterWidth, int maxWaterHeight)
    {
        if (tile.Left == null)
        {
            if (tile.Top == null)
            {
                return _waterTiles["lower_left"];
            }
            if (currentHeight == maxWaterHeight - 1)
            {
                return _waterTiles["upper_left"];
            }

            return  _waterTiles["vertical_left"];
        }
        if (tile.Top == null)
        {
            if (currentWidth == maxWaterWidth - 1)
            {
                return _waterTiles["lower_right"];
            }

            var bottomRightNeighbor = new Tile(null, new Vector2(tile.GridPosition.x + 1, tile.GridPosition.y + 1), false, false);

            if (!CanPlaceWaterTile(bottomRightNeighbor))
            {
                return _waterTiles["inside_upper_right"];
            }

            return _waterTiles["horizontal_bottom"];
        }
        if (currentHeight == maxWaterHeight - 1)
        {
            if (currentWidth == maxWaterWidth - 1)
            {
                return _waterTiles["upper_right"];
            }
            return _waterTiles["horizontal_top"];
        }
        if (currentWidth == maxWaterWidth - 1)
        {
            return _waterTiles["vertical_right"];
        }

        var upperLeftNeighbor = new Tile(null, new Vector2(tile.GridPosition.x - 1, tile.GridPosition.y - 1), false, false);

        if (!CanPlaceWaterTile(upperLeftNeighbor))
        {
            return _waterTiles["inside_lower_left"];
        }

        var bottomLeftNeighbor = new Tile(null, new Vector2(tile.GridPosition.x - 1, tile.GridPosition.y + 1), false, false);

        if (!CanPlaceWaterTile(bottomLeftNeighbor))
        {
            return _waterTiles["inside_upper_left"];
        }

        var upperRightNeighbor = new Tile(null, new Vector2(tile.GridPosition.x + 1, tile.GridPosition.y - 1), false, false);

        if (!CanPlaceWaterTile(upperRightNeighbor))
        {
            return _waterTiles["inside_lower_right"];
        }

        var rightNeighbor = new Tile(null, new Vector2(tile.GridPosition.x + 1, tile.GridPosition.y), false, false);

        if (!CanPlaceWaterTile(rightNeighbor))
        {
            return _waterTiles["vertical_right"];
        }

        var bottomNeighbor = new Tile(null, new Vector2(tile.GridPosition.x, tile.GridPosition.y + 1), false, false);

        if (!CanPlaceWaterTile(bottomNeighbor))
        {
            return _waterTiles["horizontal_top"];
        }

        return _waterTiles["center"];
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
            { "vertical_right", null },
            { "inside_lower_left", null },
            { "inside_lower_right", null },
            { "inside_upper_left", null },
            { "inside_upper_right", null }
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
            "vertical_right",
            "inside_lower_left",
            "inside_lower_right",
            "inside_upper_left",
            "inside_upper_right"
        };

        for (var i = 0; i < (waterTilePrefabs.Count > waterTiles.Count ? waterTiles.Count : waterTilePrefabs.Count); i++)
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
                var t = AreaTiles[x, y];

                t.Top = GetTop(t);
                t.Bottom = GetBottom(t);
                t.Left = GetLeft(t);
                t.Right = GetRight(t);
            }
        }
    }

    private void UpdateNeighborsForTempTile(Tile tile, Tile[,] tempMap)
    {
        tile.Top = GetTempTop(tile, tempMap);
        tile.Bottom = GetTempBottom(tile, tempMap);
        tile.Left = GetTempLeft(tile, tempMap);
        tile.Right = GetTempRight(tile, tempMap);
    }

    private Tile GetTempTop(Tile t, Tile[,] tempMap)
    {
        return tempMap[(int)t.GridPosition.x, MathHelper.Mod((int)(t.GridPosition.y - 1), Height)];
    }
    private Tile GetTempBottom(Tile t, Tile[,] tempMap)
    {
        return tempMap[(int)t.GridPosition.x, MathHelper.Mod((int)(t.GridPosition.y + 1), Height)];
    }
    private Tile GetTempLeft(Tile t, Tile[,] tempMap)
    {
        return tempMap[MathHelper.Mod((int)(t.GridPosition.x - 1), Width), (int)t.GridPosition.y];
    }
    private Tile GetTempRight(Tile t, Tile[,] tempMap)
    {
        return tempMap[MathHelper.Mod((int)(t.GridPosition.x + 1), Width), (int)t.GridPosition.y];
    }

    public static Tile[,] Rotate180(Tile[,] blueprint)
    {

        var height = blueprint.GetLength(0);
        var width = blueprint.GetLength(1);
        var answer = new Tile[height, width];

        for (var y = 0; y < height / 2; y++)
        {
            var topY = y;
            var bottomY = height - 1 - y;
            for (var topX = 0; topX < width; topX++)
            {
                var bottomX = width - topX - 1;
                answer[topY, topX] = blueprint[bottomY, bottomX];
                answer[bottomY, bottomX] = blueprint[topY, topX];
            }
        }

        if (height % 2 == 0)
        {
            return answer;
        }

        var centerY = height / 2;
        for (var leftX = 0; leftX < Mathf.CeilToInt(width / 2f); leftX++)
        {
            var rightX = width - 1 - leftX;
            answer[centerY, leftX] = blueprint[centerY, rightX];
            answer[centerY, rightX] = blueprint[centerY, leftX];
        }

        return answer;
    }
}
