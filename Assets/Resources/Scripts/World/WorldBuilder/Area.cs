using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Area
{
    private Dictionary<GameObject, Rarities> _biomeTypeTiles;

    private Dictionary<string, GameObject> _waterTiles;

    private List<string> _waterEndTiles = new List<string>
    {
        "lower_right",
        "vertical_right",
        "upper_right"
    };

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

    public string Id;

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

    public void Build()
    {
        if (AreaTiles != null)
        {
            return;
        }
        
        PresentEntities = new List<Entity>();
        
        AreaTiles = new Tile[Height, Width];

        var tileDeck = new AreaTileDeck(BiomeType);

        for (var row = 0; row < Height; row++)
        {
            for (var column = 0; column < Width; column++)
            {
                var texture = tileDeck.Draw();
                if (texture.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    AreaTiles[row, column] = new Tile(texture, new Vector2(row, column), true, true);
                }
                else
                {
                    AreaTiles[row, column] = new Tile(texture, new Vector2(row, column), false, false);
                }
                AreaTiles[row, column].Visibility = Visibilities.Invisible;
            }
        }

        PrepareSettlement();

        UpdateNeighbors();

        if (ParentCell.Rivers.Count > 0 || BiomeType == BiomeType.Swamp)
        {
            PlaceWaterTiles();
        }
        
        GenerateWildlife();

        AssignFactionCitizensToArea();
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

    private void PrepareSettlement()
    {
        if (Settlement == null)
        {
            return;
        }

        var settlementPrefab = SettlementPrefabStore.GetSettlementPrefab(Settlement.Size);

        SettlementPrefabStore.AssignBuildingToLots(settlementPrefab);

        Settlement.Lots = settlementPrefab.Lots;

        var settlementBluePrint = settlementPrefab.Blueprint;

        for (var currentRow = 0; currentRow < settlementBluePrint.GetLength(0); currentRow++)
        {
            for (var currentColumn = 0; currentColumn < settlementBluePrint.GetLength(1); currentColumn++)
            {
                var tileCode = settlementBluePrint[currentRow, currentColumn];

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

    private void AssignFactionCitizensToArea()
    {
        if (PresentFactions == null)
        {
            return;
        }

        foreach (var faction in PresentFactions)
        {
            if (faction.RemainingCitizensToPlace.Count < 1)
            {
                continue;
            }

            int maxNpcs;

            if (Settlement != null && Settlement.Faction.Name.Equals(faction.Name))
            {
                maxNpcs = Math.Min(faction.RemainingCitizensToPlace.Count,
                              SettlementPrefabStore.SettlementSizePopulationCaps[Settlement.Size]) + 1;
            }
            else
            {
                maxNpcs = faction.RemainingCitizensToPlace.Count + 1;
            }

            var numNpcsToPlace = Random.Range(1, maxNpcs);

            for (var i = 0; i < numNpcsToPlace; i++)
            {
                var citizenToPlace = faction.RemainingCitizensToPlace.First();
                faction.RemainingCitizensToPlace.RemoveAt(0);

                citizenToPlace.CurrentArea = this;
                citizenToPlace.CurrentCell = ParentCell;

                PresentEntities.Add(citizenToPlace);
            }
        }
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
            case BiomeType.Wasteland:
                return SettlementPrefabStore.WastelandDirtPathTiles[tileType];
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
            case BiomeType.Wasteland:
                return SettlementPrefabStore.WastelandDirtPathTileKeys[tileCode];
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
            var row = Random.Range(0, Height);
            var column = Random.Range(0, Width);
            
            startTile = AreaTiles[row, column];

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

        var maxWaterHeight = BiomeType == BiomeType.Swamp ? Random.Range(Height / 6, Height - 5) : Random.Range(3, Height / 2);
        var maxWaterWidth = BiomeType == BiomeType.Swamp ? Random.Range(Width / 6, Width - 5) : Random.Range(3, Width / 2);

        var tempMap = new Tile[Height, Width];
        var currentHeight = 0;
        var success = true;
        for (var currentRow = (int) startTile.GridPosition.x; currentHeight < maxWaterHeight; currentRow++)
        {
            var currentWidth = 0;
            for (var currentColumn = (int) startTile.GridPosition.y; currentWidth < maxWaterWidth; currentColumn++)
            {
                if (currentRow >= Height || currentColumn >= Width)
                {
                    success = false;
                    break;
                }

                var currentTile = AreaTiles[currentRow, currentColumn];

                if (CanPlaceWaterTile(currentTile))
                {
                    GameObject waterTilePrefab;
                    try
                    {
                        var tempTile = tempMap[currentRow, currentColumn] ?? new Tile(null, new Vector2(currentRow, currentColumn), false, false);

                        UpdateNeighborsForTempTile(tempTile, tempMap);

                        waterTilePrefab = GetCorrectWaterTilePrefab(currentTile, currentWidth, currentHeight,
                            maxWaterWidth, maxWaterHeight);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        return;
                    }
                    if (waterTilePrefab == null)
                    {
                        return;
                    }

                    tempMap[currentRow, currentColumn] = new Tile(waterTilePrefab, new Vector2(currentRow, currentColumn), false, false);

                    if (_waterEndTiles.Contains(waterTilePrefab.name))
                    {
                        break;
                    }
                }
                currentWidth++;
            }
            if (!success)
            {
                return;
            }
            currentHeight++;
        }

        for (var currentRow = 0; currentRow < Height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < Width; currentColumn++)
            {
                if (tempMap[currentRow, currentColumn] == null)
                {
                    continue;
                }

                AreaTiles[currentRow, currentColumn] = tempMap[currentRow, currentColumn];
            }
        }

        
        Debug.Log($"Water placed in cell {ParentCell.X}, {ParentCell.Y}");
    }

    private bool CanPlaceWaterTile(Tile tile)
    {
        return !tile.GetBlocksMovement() && (Settlement == null ||
               Settlement.Lots.All(lot => !lot.IsPartOfLot(new Vector2(tile.GridPosition.x, tile.GridPosition.y))));
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

            var bottomRightNeighbor = AreaTiles[(int) (tile.GridPosition.x + 1), (int) tile.GridPosition.y + 1];

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

        var rightNeighbor = AreaTiles[(int) (tile.GridPosition.x + 1), (int) tile.GridPosition.y];

        if (!CanPlaceWaterTile(rightNeighbor))
        {
            return _waterTiles["vertical_right"];
        }

        var bottomNeighbor = AreaTiles[(int)(tile.GridPosition.x), (int)tile.GridPosition.y + 1];

        if (!CanPlaceWaterTile(bottomNeighbor))
        {
            return _waterTiles["horizontal_top"];
        }

        var upperLeftNeighbor = AreaTiles[(int)(tile.GridPosition.x - 1), (int)tile.GridPosition.y - 1];

        if (!CanPlaceWaterTile(upperLeftNeighbor))
        {
            return _waterTiles["inside_lower_left"];
        }

        var bottomLeftNeighbor = AreaTiles[(int)(tile.GridPosition.x - 1), (int)tile.GridPosition.y + 1];

        if (!CanPlaceWaterTile(bottomLeftNeighbor))
        {
            return _waterTiles["inside_upper_left"];
        }

        var upperRightNeighbor = AreaTiles[(int)(tile.GridPosition.x + 1), (int)tile.GridPosition.y - 1];

        if (!CanPlaceWaterTile(upperRightNeighbor))
        {
            return _waterTiles["inside_lower_right"];
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
        return AreaTiles[MathHelper.Mod(t.X + 1, Height), t.Y];
    }
    private Tile GetBottom(Tile t)
    {
        return AreaTiles[MathHelper.Mod(t.X - 1, Height), t.Y];
    }
    private Tile GetLeft(Tile t)
    {
        return AreaTiles[t.X, MathHelper.Mod(t.Y - 1, Width)];
    }
    private Tile GetRight(Tile t)
    {
        return AreaTiles[t.X, MathHelper.Mod(t.Y + 1, Width)];
    }

    private void UpdateNeighbors()
    {
        for (var row = 0; row < Height; row++)
        {
            for (var column = 0; column < Width; column++)
            {
                var t = AreaTiles[row, column];

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
        return tempMap[(int)t.GridPosition.x, MathHelper.Mod((int)(t.GridPosition.y + 1), Height)];
    }
    private Tile GetTempBottom(Tile t, Tile[,] tempMap)
    {
        return tempMap[(int)t.GridPosition.x, MathHelper.Mod((int)(t.GridPosition.y - 1), Height)];
    }
    private Tile GetTempLeft(Tile t, Tile[,] tempMap)
    {
        return tempMap[MathHelper.Mod((int)(t.GridPosition.x - 1), Width), (int)t.GridPosition.y];
    }
    private Tile GetTempRight(Tile t, Tile[,] tempMap)
    {
        return tempMap[MathHelper.Mod((int)(t.GridPosition.x + 1), Width), (int)t.GridPosition.y];
    }

    private void GenerateWildlife()
    {
        var numWildlife = Random.Range(0, 6);

        if (numWildlife < 1)
        {
            return;
        }

        var creatureDeck = new CreatureDeck(BiomeType, numWildlife);

        for (var i = 0; i < numWildlife; i++)
        {
            var creature = creatureDeck.Draw();
            creature.CurrentArea = this;
            creature.CurrentCell = ParentCell;
            PresentEntities.Add(creature);
            WorldData.Instance.Entities.Add(creature.Id, creature);
        }
    }
}
