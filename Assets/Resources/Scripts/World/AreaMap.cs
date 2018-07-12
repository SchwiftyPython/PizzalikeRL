using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaMap : MonoBehaviour
{
    private Transform _areaMapHolderTransform;
    private Area _currentArea;
    private GameObject _playerSprite;
    private Entity _player;

    public GameObject AStar;
    public GameObject AreaMapHolder;
    public GameObject NpcSpriteHolder;
    public float GraphOffset = -0.5f;
    public bool AreaReady;

    public PopUpWindow PizzaOrderPopUp;
    public GameObject ObjectInfoWindow;

    public GameObject Camera;

    public static AreaMap Instance;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {
        AreaReady = false;
        AreaMapHolder = new GameObject("AreaMapHolder");

        //temp til player creation
        if (GameManager.Instance.Player == null)
        {
            _player = new Entity(EntityTemplateLoader.GetEntityTemplate("human"), null, true);
            _playerSprite = Instantiate(_player.GetSpritePrefab(), new Vector2(0, 0), Quaternion.identity);
            _playerSprite.transform.SetParent(GameManager.Instance.transform);
            _player.SetSprite(_playerSprite);
            GameManager.Instance.Player = _player;

            //Inventory Testing /////////////////////////////////////////////////////

            var item = new Armor(ItemTemplateLoader.GetEntityTemplate("helmet"), ItemRarity.Common);
            _player.Inventory.Add(item.Id, item);

            item = new Armor(ItemTemplateLoader.GetEntityTemplate("sword"), ItemRarity.Common);
            _player.Inventory.Add(item.Id, item);

            item = new Armor(ItemTemplateLoader.GetEntityTemplate("helmet"), ItemRarity.Rare);
            _player.Inventory.Add(item.Id, item);

            item = new Armor(ItemTemplateLoader.GetEntityTemplate("sword"), ItemRarity.Rare);
            _player.Inventory.Add(item.Id, item);

            //END Inventory Testing////////////////////////////////////////////////////////
        }
        else if(_player == null || _player != GameManager.Instance.Player)
        {
            _player = GameManager.Instance.Player;
            _playerSprite = _player.GetSprite();
            _playerSprite.transform.SetParent(GameManager.Instance.transform);
            //_player.SetSprite(_playerSprite);
        }

        _areaMapHolderTransform = AreaMapHolder.transform;

        _currentArea = GameManager.Instance.CurrentArea;
        _currentArea.TurnOrder = new Queue<Entity>();
        _currentArea.TurnOrder.Enqueue(_player);
        _currentArea.BuildArea();
        DrawArea();
        DrawSettlement();
        PlacePlayer();
        if (_currentArea.PresentEntities.Count > 1)
        {
            PlaceNPCs();
        }

        CreateAStarGraph();
        AstarPath.active.Scan();
        AreaReady = true;
    }
   
    public void EnterArea()
    {
        //Destroy(WorldMap.Instance?.Camera);
        if (AreaMapHolder != null || NpcSpriteHolder != null)
        {
            Deconstruct();
        }
        Init();
    }

    public void DrawArea()
    {
        for (var i = 0; i < _currentArea.Width; i++)
        {
            for (var j = 0; j < _currentArea.Height; j++)
            {
                var texture = _currentArea.AreaTiles[i, j].GetTileTexture();
                var instance = Instantiate(texture, new Vector2(i, j), Quaternion.identity);
                instance.transform.SetParent(_areaMapHolderTransform);
            }
        }
    }

    public void DrawSettlement()
    {
        var settlement = _currentArea.settlement;

        //testing
        var areaY = 20;

        //testing
        var buildingType = "building_small";

        var buildingBlueprint = BuildingPrefabStore.GetBuildingPrefab(buildingType);

        for (var x = 0; x < buildingBlueprint.GetLength(0); x++)
        {
            var areaX = 40;
            areaY--;
            for (var y = 0; y < buildingBlueprint.GetLength(1); y++)
            {
                var tileCode = buildingBlueprint[x, y];
                var tileType = BuildingPrefabStore.TileKeys[tileCode];

                Debug.Log($"Tile Code: {tileCode}    Tile Type: {tileType}");

                var tile = tileType.Contains("wall") ? BuildingPrefabStore.BrownStoneWallTiles[tileType] : BuildingPrefabStore.WoodenFloorTiles[tileType];

                _currentArea.AreaTiles[areaX, areaY].SetTileTexture(tile);
                var instance = Instantiate(tile, new Vector2(areaX, areaY), Quaternion.identity);
                instance.transform.SetParent(_areaMapHolderTransform);

                areaX++;
            }
        }
    }

    public void RemoveEntity(Entity entity)
    {
        Destroy(entity.GetSprite());

        _currentArea.AreaTiles[(int) entity.CurrentPosition.x, (int) entity.CurrentPosition.y].SetBlocksMovement(false);
        _currentArea.AreaTiles[(int) entity.CurrentPosition.x, (int) entity.CurrentPosition.y].SetPresentEntity(null);
        _currentArea.PresentEntities.Remove(entity);
    }

    public void InstantiatePlayerSprite()
    {
        _playerSprite = Instantiate(_player.GetSpritePrefab(), _player.CurrentPosition, Quaternion.identity);
        _playerSprite.transform.SetParent(GameManager.Instance.transform);
        _player.SetSprite(_playerSprite);
    }

    private void Deconstruct()
    {
        RemoveAllNpcs();
        Destroy(AreaMapHolder);
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
                var y = 2;
                var x = _currentArea.Width - 20;
                while (!placed)
                {
                    if (!_currentArea.AreaTiles[x, y].GetBlocksMovement())
                    {
                        _playerSprite.transform.position = new Vector3(x, y);
                        _player.CurrentPosition = new Vector3(x, y);
                        //Debug.Log(("current area: " + _currentArea.AreaTiles[x, y]));
                        _currentArea.AreaTiles[x, y].SetPresentEntity(_player);
                        placed = true;
                    }
                    y = Random.Range(0, 10);
                    x = Random.Range(_currentArea.Width - 25, _currentArea.Width);
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
        }
    }

    private void PlaceNPCs()
    {
        NpcSpriteHolder = new GameObject("NPCSpriteHolder");
        foreach (var e in _currentArea.PresentEntities)
        {
            if (e.IsPlayer())
            {
                continue;
            }

            var placed = false;
            var y = Random.Range(0, _currentArea.Height);
            var x = Random.Range(0, _currentArea.Width);
            while (!placed)
            {
                if (!_currentArea.AreaTiles[x, y].GetBlocksMovement())
                {
                    var npcSprite = Instantiate(e.GetSpritePrefab(), new Vector3(x, y, 0f), Quaternion.identity);

                    npcSprite.AddComponent<EnemyController>();
                    npcSprite.AddComponent<Seeker>();
                    npcSprite.AddComponent<EntityInfo>();

                    npcSprite.transform.SetParent(NpcSpriteHolder.transform);
                    e.SetSprite(npcSprite);
                    _currentArea.AreaTiles[x, y].SetPresentEntity(e);
                    _currentArea.AreaTiles[x, y].SetBlocksMovement(true);
                    e.CurrentPosition = new Vector3(x, y, 0f);
                    _currentArea.TurnOrder.Enqueue(e);
                    placed = true;
                }
                y = Random.Range(0, _currentArea.Height);
                x = Random.Range(0, _currentArea.Width);
            }
            e.CurrentArea = _currentArea;
            e.CurrentCell = _currentArea.ParentCell;
        }
    }

    private void RemoveAllNpcs()
    {
        foreach (var e in _currentArea.PresentEntities.ToArray())
        {
            RemoveEntity(e);
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
        gg.center = new Vector3(gg.width / 2 + GraphOffset, gg.depth / 2, -0.1f);
        gg.SetDimensions(gg.width, gg.depth, gg.nodeSize);
        gg.collision.use2D = true;
        gg.collision.type = ColliderType.Ray;
        gg.collision.mask.value = 256; //Set mask to obstacle        
        gg.rotation.x = -90;
        gg.cutCorners = false;
    }
}
