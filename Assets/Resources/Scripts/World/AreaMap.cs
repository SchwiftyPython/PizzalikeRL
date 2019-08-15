using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaMap : MonoBehaviour
{
    private readonly Dictionary<ItemRarity, float> _itemDropChances = new Dictionary<ItemRarity, float>
    {
        { ItemRarity.Common,  .288f },
        { ItemRarity.Uncommon, .236f },
        { ItemRarity.Rare, .098f },
        { ItemRarity.Legendary, .076f }
    };

    private Transform _areaMapHolderTransform;
    private Area _currentArea;
    private GameObject _playerSprite;
    private Entity _player;
    private bool _customerPresent;

    private Dictionary<string, GameObject> _waterTiles;

    public GameObject AStar;
    public GameObject AreaMapHolder;
    public GameObject NpcSpriteHolder;
    public float Offset = .5f;
    public bool AreaReady;

    public Fov Fov;
    public GameObject FovHolder;

    public PopUpWindow PizzaOrderPopUp;
    public GameObject ObjectInfoWindow;
    public GameObject DroppedItemPopUp;
    public GameObject ExclamationPointPrefab;

    public GameObject Camera;

    public static AreaMap Instance;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_player == null || _player != GameManager.Instance.Player || _playerSprite == null)
        {
            _player = GameManager.Instance.Player;
            InstantiatePlayerSprite();
        }
    }

    private void Init()
    {
        AreaReady = false;
        AreaMapHolder = new GameObject("AreaMapHolder");

        if (FovHolder == null)
        {
            FovHolder = GameObject.Find("Fov");
        }

        Fov = FovHolder.GetComponent<Fov>();

        if (_player == null || _player != GameManager.Instance.Player)
        {
            _player = GameManager.Instance.Player;
            InstantiatePlayerSprite();
        }

        _areaMapHolderTransform = AreaMapHolder.transform;

        _currentArea = GameManager.Instance.CurrentArea;
        _currentArea.TurnOrder = new Queue<Entity>();
        _currentArea.TurnOrder.Enqueue(_player);
        _currentArea.Build();

//        if (_currentArea.ParentCell.Rivers.Count > 0)
//        {
//            PlaceWaterTiles();
//        }

        DrawArea();
        PlaceBuildings();
        PlacePlayer();

        if (_currentArea.PresentEntities.Count > 1)
        {
            PlaceNpcs();
            MarkCustomers();
        }

        CreateAStarGraph();
        AstarPath.active.Scan();

        Fov.Init(_currentArea);
        var v = new Vinteger(_player.CurrentTile.X, _player.CurrentTile.Y);
        Fov.Refresh(v);

        AreaReady = true;
    }
   
    public void EnterArea()
    {
        if (AreaMapHolder != null || NpcSpriteHolder != null)
        {
            Deconstruct();
        }
        Init();
    }

    public void DrawArea()
    {
        for (var currentRow = 0; currentRow < _currentArea.Height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < _currentArea.Width; currentColumn++)
            {
                var tile = _currentArea.AreaTiles[currentRow, currentColumn];

                var prefab = tile.GetPrefabTileTexture();
                var instance = Instantiate(prefab, new Vector2(currentColumn, currentRow), Quaternion.identity);
                tile.TextureInstance = instance;
                instance.transform.SetParent(_areaMapHolderTransform);

                if (tile.PresentProp != null)
                {
                    prefab = tile.PresentProp.Prefab;
                    instance = Instantiate(prefab, new Vector2(currentColumn, currentRow), Quaternion.identity);
                    tile.PresentProp.Texture = instance;
                    instance.transform.SetParent(tile.TextureInstance.transform);
                }

                tile.FovTile = Instantiate(Fov.FovCenterPrefab, new Vector3(currentColumn, currentRow, -4), Quaternion.identity);
                tile.FovTile.transform.SetParent(FovHolder.transform);
            }
        }
    }

    public void MarkCustomers()
    {
        foreach (var entity in _currentArea.PresentEntities)
        {
            if (!entity.IsCustomer)
            {
                continue;
            }

            var marker = Instantiate(ExclamationPointPrefab, new Vector2(0, 0), Quaternion.identity);
                
            marker.transform.SetParent(entity.GetSprite().transform);

            marker.transform.localPosition = new Vector3(0.5f, 1.35f, -1);
        }
    }

    public void PlaceBuildings()
    {
        if (_currentArea.SettlementSection?.Lots == null)
        {
            return;
        }

        var settlementSection = _currentArea.SettlementSection;

        foreach (var lot in settlementSection.Lots)
        {
            var areaY = (int)lot.LowerLeftCorner.y;
            var areaX = (int)lot.LowerLeftCorner.x;
            var building = lot.AssignedBuilding;
            for (var currentRow = 0; currentRow < building.Height; currentRow++)
            {
                for (var currentColumn = 0; currentColumn < building.Width; currentColumn++)
                {
                    if (building.WallTiles[currentRow, currentColumn] != null)
                    {
                        var tile = building.FloorTiles[currentRow, currentColumn];

                        _currentArea.AreaTiles[areaX, areaY].SetPrefabTileTexture(tile);

                        if (_currentArea.AreaTiles[areaX, areaY].TextureInstance != null)
                        {
                            Destroy(_currentArea.AreaTiles[areaX, areaY].TextureInstance);
                        }

                        var instance = Instantiate(tile, new Vector2(areaY, areaX), Quaternion.identity);
                        _currentArea.AreaTiles[areaX, areaY].TextureInstance = instance;
                        instance.transform.SetParent(_areaMapHolderTransform);

                        tile = building.WallTiles[currentRow, currentColumn];

                        if (tile.name.Contains("door"))
                        {
                            //todo check for obstacle in front of door and replace

                            _currentArea.AreaTiles[areaX, areaY].SetBlocksMovement(false);

                            tile.GetComponent<Door>().CurrentTile = _currentArea.AreaTiles[areaX, areaY];
                        }
                        else
                        {
                            _currentArea.AreaTiles[areaX, areaY].SetBlocksMovement(true);
                        }
                        
                        _currentArea.AreaTiles[areaX, areaY].SetBlocksLight(true);
                        instance = Instantiate(tile, new Vector2(areaY, areaX), Quaternion.identity);
                        _currentArea.AreaTiles[areaX, areaY].PresentWallTile = instance;
                        instance.transform.SetParent(_areaMapHolderTransform);
                    }
                    else if(building.FloorTiles[currentRow, currentColumn] != null)
                    {
                        var tile = building.FloorTiles[currentRow, currentColumn];

                        _currentArea.AreaTiles[areaX, areaY].SetPrefabTileTexture(tile);

                        if (_currentArea.AreaTiles[areaX, areaY].TextureInstance != null)
                        {
                            Destroy(_currentArea.AreaTiles[areaX, areaY].TextureInstance);
                        }

                        _currentArea.AreaTiles[areaX, areaY].SetBlocksLight(false);
                        var instance = Instantiate(tile, new Vector2(areaY, areaX), Quaternion.identity);
                        _currentArea.AreaTiles[areaX, areaY].TextureInstance = instance;
                        instance.transform.SetParent(_areaMapHolderTransform);

                        if (building.Props[currentRow, currentColumn] != null)
                        {
                            if (building.Props[currentRow, currentColumn].name.Contains("chest"))
                            {
                                _currentArea.AreaTiles[areaX, areaY].PresentProp =
                                    new Chest(building.Props[currentRow, currentColumn]);
                            }
                            else
                            {
                                _currentArea.AreaTiles[areaX, areaY].PresentProp =
                                    new Prop(building.Props[currentRow, currentColumn]);
                            }

                            instance = Instantiate(_currentArea.AreaTiles[areaX, areaY].PresentProp.Prefab,
                                new Vector2(areaY, areaX), Quaternion.identity);

                            _currentArea.AreaTiles[areaX, areaY].PresentProp.Texture = instance;

                            instance.transform.SetParent(_areaMapHolderTransform);
                        }
                    }
                    _currentArea.AreaTiles[areaX, areaY].Lot = lot;
                    areaY++;
                }
                areaX--;
                areaY = (int)lot.LowerLeftCorner.y;
            }
        }
    }

    public void RemoveEntity(Entity entity)
    {
        Destroy(entity.GetSprite());

        entity.CurrentTile.SetBlocksMovement(false);
        entity.CurrentTile.SetPresentEntity(null);
        _currentArea.PresentEntities.Remove(entity);
        entity.UnsubscribeFromAllEvents();
    }

    //<Summary>
    // Removes dead entities from area
    //</Summary>
    public void RemoveDeadEntity(Entity entity)
    {
        RemoveEntity(entity);
        HandleItemDrops(entity);
    }

    public void InstantiatePlayerSprite()
    {
       var existingPlayerSprite = GameObject.FindWithTag("Player");

        if (existingPlayerSprite != null)
        {
            Destroy(existingPlayerSprite);
        }

        _playerSprite = Instantiate(_player.GetSpritePrefab(), _player.CurrentPosition, Quaternion.identity);
        _playerSprite.transform.SetParent(GameManager.Instance.transform);
        _playerSprite.tag = "Player";
        _player.SetSprite(_playerSprite);
        _player.GetSprite().AddComponent<Seeker>();
        _player.GetSprite().AddComponent<AstarAI>();
    }

    public void Deconstruct()
    {
        RemoveAllNpcs();

        if (AreaMapHolder != null)
        {
            Destroy(AreaMapHolder);
        }

        if (FovHolder == null)
        {
            return;
        }

        for (var i = 0; i < FovHolder.transform.childCount; i++)
        {
            Destroy(FovHolder.transform.GetChild(i).gameObject);
        }
    }

    private void HandleItemDrops(Entity entity)
    {
        var itemRarityForRoll = DetermineRarityForItemDropRoll();

        Item item = null;
        if (ItemDropped(itemRarityForRoll))
        {
            item = ItemStore.Instance.GetRandomItemForRarity(itemRarityForRoll);

            entity.CurrentTile.PresentItems.Add(item);
        }

        if (entity.ToppingDropped != null)
        {
            entity.CurrentTile.PresentTopping = new Topping(entity.ToppingDropped);
        }

        if (entity.CurrentTile.PresentItems.Count > 1 ||
            entity.CurrentTile.PresentItems.Count > 0 && entity.CurrentTile.PresentTopping != null)
        {
           var chestSprite = Instantiate(
                WorldData.Instance.SmallChest, entity.CurrentTile.GridPosition,
                Quaternion.identity);

            foreach (var droppedItem in entity.CurrentTile.PresentItems)
            {
                droppedItem.WorldSprite = chestSprite;
            }

            entity.CurrentTile.PresentTopping.WorldSprite = chestSprite;

            return;
        }

        if (entity.CurrentTile.PresentItems.Count > 0)
        {
            if (item != null)
            {
                item.WorldSprite = Instantiate(
                    item.WorldPrefab, entity.CurrentTile.GridPosition,
                    Quaternion.identity);
            }
        }

        if (entity.CurrentTile.PresentTopping != null)
        {
            entity.CurrentTile.PresentTopping.WorldSprite = Instantiate(
                entity.CurrentTile.PresentTopping.WorldSpritePrefab, entity.CurrentTile.GridPosition,
                Quaternion.identity);
        }
    }

    private static ItemRarity DetermineRarityForItemDropRoll()
    {
        var roll = Random.Range(0, 4);

        switch (roll)
        {
            case 0: return ItemRarity.Common;
            case 1: return ItemRarity.Uncommon;
            case 2: return ItemRarity.Rare;
            case 3: return ItemRarity.Legendary;
            default: return ItemRarity.Common;
        }
    }

    private bool ItemDropped(ItemRarity rarity)
    {
        var roll = Random.Range(0f, 1f);

        return roll < _itemDropChances[rarity];
    }

    private void PlacePlayer()
    {
        if (GameManager.Instance.PlayerInStartingArea || GameManager.Instance.PlayerEnteringAreaFromWorldMap)
        {
            GameManager.Instance.PlayerEnteringAreaFromWorldMap = false;
            GameManager.Instance.PlayerInStartingArea = false;
            if (_currentArea != null)
            {
                var placed = false;
                var y = _currentArea.Width - 5;
                var x = _currentArea.Height - 20;
                while (!placed)
                {
                    if (!_currentArea.AreaTiles[x, y].GetBlocksMovement())
                    {
                        _player.CurrentPosition = new Vector3(x, y);
                        _player.CurrentTile = _currentArea.AreaTiles[x, y];
                        //Debug.Log(("current area: " + _currentArea.AreaTiles[x, y]));
                        _currentArea.AreaTiles[x, y].SetPresentEntity(_player);
                        placed = true;
                    }
                    y = Random.Range(0, 10);
                    x = Random.Range(_currentArea.Height - 25, _currentArea.Height);
                }
            }
        }
        else
        {
            _playerSprite.transform.position = GameManager.Instance.Player.CurrentPosition;
            
        }
        _currentArea?.PresentEntities.Add(_player);
        _player.CurrentArea = _currentArea;

        if (_currentArea != null)
        {
            _player.CurrentCell = _currentArea.ParentCell;
            _currentArea.AreaTiles[_player.CurrentTile.X, _player.CurrentTile.Y]
                .Visibility = Visibilities.Visible;
        }
    }

    private void PlaceNpcs()
    {
        NpcSpriteHolder = new GameObject("NPCSpriteHolder");
        foreach (var e in _currentArea.PresentEntities)
        {
            if (e.IsPlayer())
            {
                continue;
            }

            var placed = false;
            var row = Random.Range(0, _currentArea.Height);
            var column = Random.Range(0, _currentArea.Width);
            while (!placed)
            {
                if (!_currentArea.AreaTiles[row, column].GetBlocksMovement())
                {
                    var npcSprite = Instantiate(e.GetSpritePrefab(), new Vector2(column, row), Quaternion.identity);

                    npcSprite.AddComponent<EnemyController>();
                    npcSprite.AddComponent<Seeker>();
                    npcSprite.AddComponent<EntityInfo>();

                    npcSprite.GetComponent<EnemyController>().Self = e;

                    npcSprite.transform.SetParent(NpcSpriteHolder.transform);

                    e.SetSprite(npcSprite);

                    e.CurrentTile = _currentArea.AreaTiles[row, column];
                    e.CurrentTile.SetPresentEntity(e);
                    e.CurrentTile.SetBlocksMovement(true);
                    e.CurrentPosition = new Vector3(row, column, 0f);

                    _currentArea.TurnOrder.Enqueue(e);
                    placed = true;
                }
                row = Random.Range(0, _currentArea.Height);
                column = Random.Range(0, _currentArea.Width);
            }
            e.CurrentArea = _currentArea;
            e.CurrentCell = _currentArea.ParentCell;
        }
    }

    private void RemoveAllNpcs()
    {
        if (_currentArea == null)
        {
            return;
        }

        foreach (var e in _currentArea.PresentEntities.ToArray())
        {
            Destroy(e.GetSprite());
            e.CurrentTile?.SetBlocksMovement(false);
            e.CurrentTile?.SetPresentEntity(null);
        }
        Destroy(NpcSpriteHolder);
    }

    private void CreateAStarGraph()
    {
        var data = AStar.GetComponent<AstarPath>().data;
        var gg = data.AddGraph(typeof(GridGraph)) as GridGraph;

        if (gg == null)
        {
            return;
        }
        gg.width = _currentArea.Width;
        gg.depth = _currentArea.Height;
        gg.nodeSize = 1;
        gg.center = new Vector3(gg.width / 2, gg.depth / 2 + Offset, -0.1f);
        gg.SetDimensions(gg.width, gg.depth, gg.nodeSize);
        gg.collision.use2D = true;
        gg.collision.type = ColliderType.Ray;
        gg.collision.mask.value = 256; //Set mask to obstacle        
        gg.rotation.x = -90;
        gg.cutCorners = false;
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
            var x = Random.Range(0, _currentArea.Width);
            var y = Random.Range(0, _currentArea.Height);

            startTile = _currentArea.AreaTiles[x, y];

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

        //todo temporary until additional water tiles available 
        var maxWidthAndHeight = Random.Range(3, _currentArea.Height / 2);

        var maxWaterHeight = maxWidthAndHeight;
        var maxWaterWidth = maxWidthAndHeight;

        var tempMap = new Tile[_currentArea.Height, _currentArea.Width];
        var currentWidth = 0;
        var success = true;
        for (var currentRow = (int)startTile.GridPosition.y; currentWidth < maxWaterWidth; currentRow++)
        {
            var currentHeight = 0;
            for (var currentColumn = (int)startTile.GridPosition.x; currentHeight < maxWaterHeight; currentColumn++)
            {
                if (currentRow >= _currentArea.Height || currentColumn >= _currentArea.Width)
                {
                    success = false;
                    break;
                }

                var currentTile = tempMap[currentRow, currentRow] ?? new Tile(null, new Vector2(currentRow, currentColumn), false, false);

                UpdateNeighborsForTempTile(currentTile, tempMap);

                if (CanPlaceWaterTile(currentTile))
                {
                    var waterTilePrefab = GetCorrectWaterTilePrefab(currentTile, currentWidth, currentHeight, maxWaterWidth, maxWaterHeight);

                    tempMap[currentRow, currentColumn] = new Tile(waterTilePrefab, new Vector2(currentRow, currentColumn), false, false);
                }
                else
                {
                    success = false;
                    break;
                }
                currentHeight++;
            }
            currentWidth++;
        }

        if (!success)
        {
            return;
        }
        //todo fix this
        currentWidth = 0;
        for (var currentRow = (int)startTile.GridPosition.x; currentWidth < maxWaterWidth; currentRow++)
        {
            var currentHeight = 0;
            for (var currentColumn = (int)startTile.GridPosition.y; currentWidth < maxWaterHeight; currentColumn++)
            {
                _currentArea.AreaTiles[currentRow, currentColumn] = tempMap[currentRow, currentColumn];
                currentHeight++;
            }
            currentWidth++;
        }

        //Debug.Log($"Water placed in cell {_currentArea.ParentCell.X}, {_currentArea.ParentCell.Y}");
    }

    private bool CanPlaceWaterTile(Tile tile)
    {
        return _currentArea.SettlementSection == null ||
               _currentArea.SettlementSection.Lots.All(lot => !lot.IsPartOfLot(new Vector2(tile.GridPosition.x, tile.GridPosition.y)));
    }

    private Dictionary<string, GameObject> GetWaterTiles()
    {
        switch (_currentArea.BiomeType)
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

    private GameObject GetCorrectWaterTilePrefab(Tile tile, int currentWidth, int currentHeight, int maxWaterWidth, int maxWaterHeight)
    {
        if (tile.Left == null)
        {
            if (tile.Top == null)
            {
                return _waterTiles["upper_left"];
            }
            if (currentHeight == maxWaterHeight - 1)
            {
                return _waterTiles["lower_left"];
            }
            return _waterTiles["vertical_left"];
        }
        if (tile.Top == null)
        {
            if (currentWidth == maxWaterWidth - 1)
            {
                return _waterTiles["upper_right"];
            }
            return _waterTiles["horizontal_top"];
        }
        if (currentHeight == maxWaterHeight - 1)
        {
            if (currentWidth == maxWaterWidth - 1)
            {
                return _waterTiles["lower_right"];
            }
            return _waterTiles["horizontal_bottom"];
        }
        if (currentWidth == maxWaterWidth - 1)
        {
            return _waterTiles["vertical_right"];
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

    private void UpdateNeighborsForTempTile(Tile tile, Tile[,] tempMap)
    {
        tile.Top = GetTempTop(tile, tempMap);
        tile.Bottom = GetTempBottom(tile, tempMap);
        tile.Left = GetTempLeft(tile, tempMap);
        tile.Right = GetTempRight(tile, tempMap);
    }

    //todo fix these
    private Tile GetTempTop(Tile t, Tile[,] tempMap)
    {
        return tempMap[(int)t.GridPosition.x, MathHelper.Mod((int)(t.GridPosition.y - 1), _currentArea.Height)];
    }
    private Tile GetTempBottom(Tile t, Tile[,] tempMap)
    {
        return tempMap[(int)t.GridPosition.x, MathHelper.Mod((int)(t.GridPosition.y + 1), _currentArea.Height)];
    }
    private Tile GetTempLeft(Tile t, Tile[,] tempMap)
    {
        return tempMap[MathHelper.Mod((int)(t.GridPosition.x - 1), _currentArea.Width), (int)t.GridPosition.y];
    }
    private Tile GetTempRight(Tile t, Tile[,] tempMap)
    {
        return tempMap[MathHelper.Mod((int)(t.GridPosition.x + 1), _currentArea.Width), (int)t.GridPosition.y];
    }
}
