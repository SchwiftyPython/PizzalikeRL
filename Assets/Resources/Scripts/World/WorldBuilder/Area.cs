using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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

    private enum LoadingSteps
    {
        NewBlueprint,
        Dimensions,
        Template
    }

    private static IDictionary<MiscPropType, List<char[,]>> _miscPropBlueprints;

    private static readonly IDictionary<char, List<GameObject>> PropPrefabs = new Dictionary<char, List<GameObject>>
    {
        { GraveyardKey, null },
        { FieldKey, null },
        { CheeseKey, null}
    };

    private static IDictionary<MiscPropType, int> _weightedPropPrefabKeys = new Dictionary<MiscPropType, int>
    {
        { MiscPropType.Graveyard, 45 },
        { MiscPropType.Field, 55 },
        { MiscPropType.Cheese, 60 }
    };

    public const char GraveyardKey = 'g';
    public const char FieldKey = 'f';
    public const char CheeseKey = 'c';

    public enum MiscPropType
    {
        Field,
        Graveyard,
        Cheese
    }

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
    public SettlementSection SettlementSection;

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

        LoadPropBlueprintsFromFile();
        
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
        PlaceMiscProps();

        UpdateNeighbors();

        if (ParentCell.Rivers?.Count > 0 || BiomeType == BiomeType.Swamp)
        {
            //todo fix
            //PlaceWaterTiles();
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
        if (SettlementSection == null)
        {
            return;
        }

        var settlementPrefab = SettlementPrefabStore.GetSettlementPrefab(Settlement.Size);

        if (PresentFactions == null || PresentFactions.Count < 1 || PresentFactions.First() == null)
        {
            SettlementPrefabStore.AssignBuildingToStartingArea(settlementPrefab);
        }
        else
        {
            SettlementPrefabStore.AssignBuildingToLots(settlementPrefab);
        }

        SettlementSection.Lots = settlementPrefab.Lots;

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

                var tile = GetTilePrefab(tileCode);

                AreaTiles[currentRow, currentColumn] =
                    new Tile(tile, new Vector2(currentRow, currentColumn), false, false);
            }
        }
        PlaceSettlementProps();
    }

    private void PlaceSettlementProps()
    {
        var propChance = 99;

        var roll = Random.Range(1, 101);

        while (roll <= propChance)
        {
            var propType = SettlementPrefabStore.GetRandomPropType();

            var blueprintChance = 17;

            roll = Random.Range(1, 101);

            if (roll <= blueprintChance)
            {

                // todo not implemented
                while (propType == SettlementPrefabStore.SettlementPropType.Fence)
                {
                    propType = SettlementPrefabStore.GetRandomPropType();
                }

                var propBlueprint = SettlementPrefabStore.GetPropBlueprintByType(propType);

                var areaRow = Random.Range(0, Height);
                var startingAreaColumn = Random.Range(0, Width);
                var currentAreaColumn = startingAreaColumn;

                var blueprintHeight = propBlueprint.GetLength(0);
                var blueprintWidth = propBlueprint.GetLength(1);

                var propPrefabs = new Dictionary<char, List<GameObject>>();

                for (var currentRow = 0; currentRow < blueprintHeight; currentRow++)
                {
                    for (var currentColumn = 0; currentColumn < blueprintWidth; currentColumn++)
                    {
                        if (areaRow < 0 || areaRow >= Height || currentAreaColumn < 0 || currentAreaColumn >= Width)
                        {
                            continue;
                        }

                        var currentTile = AreaTiles[areaRow, currentAreaColumn];

                        //todo should probably make a tile type enum because this is trash
                        if (currentTile.PresentWallTile != null ||
                            currentTile.GetPrefabTileTexture().name.Contains("floor") ||
                            currentTile.GetPrefabTileTexture().name.Contains("road") ||
                            currentTile.GetPrefabTileTexture().name.Contains("path") ||
                            currentTile.GetBlocksMovement())
                        {
                            continue;
                        }

                        var currentKey = propBlueprint[currentRow, currentColumn];

                        if (!propPrefabs.ContainsKey(currentKey))
                        {
                            var prefabsForCurrentKey = SettlementPrefabStore.GetPropPrefabsByKey(currentKey);

                            propPrefabs.Add(currentKey, prefabsForCurrentKey);
                        }

                        if (propPrefabs[currentKey] == null || propPrefabs[currentKey].Count < 1)
                        {
                            continue;
                        }

                        GameObject prefab = null;
                        if (currentKey != SettlementPrefabStore.TurretKey)
                        {
                            prefab = propPrefabs[currentKey][Random.Range(0, propPrefabs[currentKey].Count)];
                        }

                        if (currentKey == SettlementPrefabStore.FieldKey)
                        {
                            //todo pick field type
                            currentTile.PresentProp = new Field(FieldType.Wheat, prefab);
                        }
                        else if (currentKey == SettlementPrefabStore.GraveyardKey)
                        {
                            currentTile.PresentProp = new Grave(prefab);
                        }
                        else if (currentKey == SettlementPrefabStore.TurretKey)
                        {
                            var template = EntityTemplateLoader.GetEntityTemplate("turret");

                            var turret = new Entity(template, Settlement.Faction);

                            var turretBarrel = new Weapon(ItemTemplateLoader.GetItemTemplate("TurretBarrel"),
                                GlobalHelper.GetRandomEnumValue<ItemRarity>());
                            
                            turret.EquipItem(turretBarrel, Entity.EquipmentSlot.Special);

                            currentTile.SetPresentEntity(turret);
                        }
                        else
                        {
                            currentTile.PresentProp = new Prop(prefab);
                        }

                        currentAreaColumn++;
                    }

                    areaRow++;
                    currentAreaColumn = startingAreaColumn;
                }
            }
            else
            {
                var prefabs = SettlementPrefabStore.GetPropPrefabByType(propType);

                if (prefabs == null)
                {
                    continue;
                }

                var areaRow = Random.Range(0, Height);
                var areaColumn = Random.Range(0, Width);

                var currentTile = AreaTiles[areaRow, areaColumn];

                //todo should probably make a tile type enum because this is trash
                if (currentTile.PresentWallTile != null ||
                    currentTile.GetPrefabTileTexture().name.Contains("floor") ||
                    currentTile.GetPrefabTileTexture().name.Contains("road") ||
                    currentTile.GetPrefabTileTexture().name.Contains("path") ||
                    currentTile.GetBlocksMovement())
                {
                    continue;
                }

                if (propType == SettlementPrefabStore.SettlementPropType.Field)
                {
                    currentTile.PresentProp = new Field(FieldType.Wheat, prefabs[Random.Range(0, prefabs.Count)]);
                }
                else if (propType == SettlementPrefabStore.SettlementPropType.Security)
                {
                    var template = EntityTemplateLoader.GetEntityTemplate("turret");

                    var turret = new Entity(template, Settlement.Faction);

                    var turretBarrel = new Weapon(ItemTemplateLoader.GetItemTemplate("TurretBarrel"),
                        GlobalHelper.GetRandomEnumValue<ItemRarity>());

                    turret.EquipItem(turretBarrel, Entity.EquipmentSlot.Special);

                    currentTile.SetPresentEntity(turret);

                    //todo raise new entity event
                }
                else
                {
                    currentTile.PresentProp = new Prop(prefabs[Random.Range(0, prefabs.Count)]);
                }
            }

            propChance -= 23;

            if (propChance < 1)
            {
                propChance = 1;
            }

            roll = Random.Range(1, 101);
        }
    }

    private void PlaceMiscProps()
    {
        var propChance = 75;

        var roll = Random.Range(1, 101);

        while (roll <= propChance)
        {
            var propType = GetRandomMiscPropType();

            var blueprintChance = 17;

            roll = Random.Range(1, 101);

            if (roll <= blueprintChance)
            {
                var propBlueprint = GetPropBlueprintByType(propType);

                var areaRow = Random.Range(0, Height);
                var startingAreaColumn = Random.Range(0, Width);
                var currentAreaColumn = startingAreaColumn;

                var blueprintHeight = propBlueprint.GetLength(0);
                var blueprintWidth = propBlueprint.GetLength(1);

                var propPrefabs = new Dictionary<char, List<GameObject>>();

                for (var currentRow = 0; currentRow < blueprintHeight; currentRow++)
                {
                    for (var currentColumn = 0; currentColumn < blueprintWidth; currentColumn++)
                    {
                        if (areaRow < 0 || areaRow >= Height || currentAreaColumn < 0 || currentAreaColumn >= Width)
                        {
                            continue;
                        }

                        var currentTile = AreaTiles[areaRow, currentAreaColumn];

                        //todo should probably make a tile type enum because this is trash
                        if (currentTile.PresentWallTile != null ||
                            currentTile.GetPrefabTileTexture().name.Contains("floor") ||
                            currentTile.GetPrefabTileTexture().name.Contains("road") ||
                            currentTile.GetPrefabTileTexture().name.Contains("path") ||
                            currentTile.GetBlocksMovement())
                        {
                            continue;
                        }

                        var currentKey = propBlueprint[currentRow, currentColumn];

                        if (!propPrefabs.ContainsKey(currentKey))
                        {
                            var prefabsForCurrentKey = GetPropPrefabsByKey(currentKey);
                            propPrefabs.Add(currentKey, prefabsForCurrentKey);
                        }

                        if (propPrefabs[currentKey] == null || propPrefabs[currentKey].Count < 1)
                        {
                            continue;
                        }

                        var prefab = propPrefabs[currentKey][Random.Range(0, propPrefabs[currentKey].Count)];

                        if (currentKey == FieldKey)
                        {
                            //todo pick field type
                            currentTile.PresentProp = new Field(FieldType.Wheat, prefab);
                        }
                        else if (currentKey == GraveyardKey)
                        {
                            currentTile.PresentProp = new Grave(prefab);
                        }
                        else
                        {
                            currentTile.PresentProp = new Prop(prefab);
                        }

                        currentAreaColumn++;
                    }
                    areaRow++;
                    currentAreaColumn = startingAreaColumn;
                }
            }
            else
            {
                var prefabs = GetPropPrefabByType(propType);

                if (prefabs == null)
                {
                    continue;
                }

                var areaRow = Random.Range(0, Height);
                var areaColumn = Random.Range(0, Width);

                var currentTile = AreaTiles[areaRow, areaColumn];

                //todo should probably make a tile type enum because this is trash
                if (currentTile.PresentWallTile != null ||
                    currentTile.GetPrefabTileTexture().name.Contains("floor") ||
                    currentTile.GetPrefabTileTexture().name.Contains("road") ||
                    currentTile.GetPrefabTileTexture().name.Contains("path") ||
                    currentTile.GetBlocksMovement())
                {
                    continue;
                }

                if (propType == MiscPropType.Field)
                {
                    currentTile.PresentProp = new Field(FieldType.Wheat, prefabs[Random.Range(0, prefabs.Count)]);
                }
                else if (propType == MiscPropType.Cheese)
                {
                    currentTile.PresentProp = new CheeseTree(prefabs[Random.Range(0, prefabs.Count)]);
                }
                else
                {
                    currentTile.PresentProp = new Prop(prefabs[Random.Range(0, prefabs.Count)]);
                }
            }

            propChance -= 5;

            if (propChance < 1)
            {
                propChance = 1;
            }

            roll = Random.Range(1, 101);
        }
    }

    private char[,] GetPropBlueprintByType(MiscPropType propType)
    {
        var blueprints = _miscPropBlueprints[propType];

        var index = Random.Range(0, blueprints.Count);

        return blueprints[index];
    }

    [CanBeNull]
    private List<GameObject> GetPropPrefabsByKey(char key)
    {
        return PropPrefabs.ContainsKey(key) ? PropPrefabs[key] : null;
    }

    [CanBeNull]
    private List<GameObject> GetPropPrefabByType(MiscPropType propType)
    {
        switch (propType)
        {
            case MiscPropType.Cheese:
                return new List<GameObject>{WorldData.Instance.CheeseTreePrefab};
            case MiscPropType.Field:
                return new List<GameObject>(WorldData.Instance.WheatFieldTiles);
            case MiscPropType.Graveyard:
                return new List<GameObject>(WorldData.Instance.GraveyardProps);
            default:
                return null;
        }
    }

    private MiscPropType GetRandomMiscPropType()
    {
        return GlobalHelper.GetRandomEnumValue<MiscPropType>();
    }

    private void LoadPropBlueprintsFromFile()
    {
        _miscPropBlueprints = new Dictionary<MiscPropType, List<char[,]>>();

        var blueprintFile = WorldData.Instance.MiscPropBlueprintsFile.text.Split("\r\n"[0]).ToList();

        var currentStep = LoadingSteps.NewBlueprint;

        var numColumns = 0;

        var x = 0;

        var currentPreFab = MiscPropType.Field;

        foreach (var line in blueprintFile)
        {
            var trimmedLine = line.Trim('\n');

            if (string.IsNullOrEmpty(trimmedLine))
            {
                currentStep = LoadingSteps.NewBlueprint;
                continue;
            }

            if (currentStep == LoadingSteps.NewBlueprint)
            {
                currentPreFab = GetKeyForCurrentPrefab(trimmedLine);

                if (!_miscPropBlueprints.ContainsKey(currentPreFab))
                {
                    _miscPropBlueprints.Add(currentPreFab, new List<char[,]>());
                }

                currentStep = LoadingSteps.Dimensions;
                x = 0;
                continue;
            }

            if (currentStep == LoadingSteps.Dimensions)
            {
                var dimensions = trimmedLine.Split(' ');
                var numRows = int.Parse(dimensions[0]);
                numColumns = int.Parse(dimensions[1]);
                _miscPropBlueprints[currentPreFab].Add(new char[numRows, numColumns]);
                currentStep = LoadingSteps.Template;
                continue;
            }

            if (currentStep == LoadingSteps.Template)
            {
                for (var currentColumn = 0; currentColumn < numColumns; currentColumn++)
                {
                    var row = _miscPropBlueprints[currentPreFab].Last();

                    row[x, currentColumn] = trimmedLine[currentColumn];
                }
                x++;
            }
        }
    }

    private static MiscPropType GetKeyForCurrentPrefab(string trimmedLine)
    {
        if (trimmedLine.Contains("field"))
        {
            return MiscPropType.Field;
        }
        if (trimmedLine.Contains("graveyard"))
        {
            return MiscPropType.Graveyard;
        }

        return MiscPropType.Cheese;
    }

    private void AssignFactionCitizensToArea()
    {
        if (PresentFactions == null || PresentFactions.Count < 1)
        {
            return;
        }

        foreach (var faction in PresentFactions)
        {
            if (faction?.RemainingCitizensToPlace == null || faction.RemainingCitizensToPlace.Count < 1)
            {
                continue;
            }

            int maxNpcs;

            if (SettlementSection != null && Settlement.Faction.Name.Equals(faction.Name))
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
                        Debug.Log("Water placement error: " + e.Message);
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
        return !tile.GetBlocksMovement() && (SettlementSection == null ||
               SettlementSection.Lots.All(lot => !lot.IsPartOfLot(new Vector2(tile.GridPosition.x, tile.GridPosition.y))));
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
