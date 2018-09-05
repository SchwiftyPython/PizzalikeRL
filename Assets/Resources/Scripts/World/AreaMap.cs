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
    public float Offset = .5f;
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

        if(_player == null || _player != GameManager.Instance.Player)
        {
            _player = GameManager.Instance.Player;
            InstantiatePlayerSprite();
            
        }

        _areaMapHolderTransform = AreaMapHolder.transform;

        _currentArea = GameManager.Instance.CurrentArea;
        _currentArea.TurnOrder = new Queue<Entity>();
        _currentArea.TurnOrder.Enqueue(_player);
        _currentArea.BuildArea();
        DrawArea();
        PlaceBuildings();
        PlacePlayer();

        //testing
        _currentArea.PresentEntities.Add(new Entity(EntityTemplateLoader.GetEntityTemplate("human")));

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
                var texture = _currentArea.AreaTiles[i, j].GetPrefabTileTexture();
                var instance = Instantiate(texture, new Vector2(i, j), Quaternion.identity);
                _currentArea.AreaTiles[i, j].TextureInstance = instance;
                instance.transform.SetParent(_areaMapHolderTransform);
                instance.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void PlaceBuildings()
    {
        if (_currentArea.Settlement?.Lots == null)
        {
            return;
        }

        var settlement = _currentArea.Settlement;

        foreach (var lot in settlement.Lots)
        {
            var areaY = (int)lot.LowerLeftCorner.x;
            var areaX = (int)lot.LowerLeftCorner.y;
            var building = lot.AssignedBuilding;
            for (var currentRow = 0; currentRow < building.Height; currentRow++)
            {
                areaY--;
                for (var currentColumn = 0; currentColumn < building.Width; currentColumn++)
                {
                    if (building.WallTiles[currentRow, currentColumn] != null)
                    {
                        var tile = building.FloorTiles[currentRow, currentColumn];

                        _currentArea.AreaTiles[areaX, areaY].SetPrefabTileTexture(tile);
                        var instance = Instantiate(tile, new Vector2(areaX, areaY), Quaternion.identity);
                        instance.transform.SetParent(_areaMapHolderTransform);

                        tile = building.WallTiles[currentRow, currentColumn];

                        _currentArea.AreaTiles[areaX, areaY].SetBlocksMovement(true);
                        instance = Instantiate(tile, new Vector2(areaX, areaY), Quaternion.identity);
                        instance.transform.SetParent(_areaMapHolderTransform);
                    }
                    else
                    {
                        var tile = building.FloorTiles[currentRow, currentColumn];

                        _currentArea.AreaTiles[areaX, areaY].SetPrefabTileTexture(tile);
                        var instance = Instantiate(tile, new Vector2(areaX, areaY), Quaternion.identity);
                        instance.transform.SetParent(_areaMapHolderTransform);
                    }
                    areaX++;
                }
                areaX = (int)lot.LowerLeftCorner.y;
            }


            /*var areaX = (int) lot.UpperLeftCorner.x;
            for (var x = 0; x < buildingBlueprint.GetLength(0); x++)
            {
                areaY--;
                for (var y = 0; y < buildingBlueprint.GetLength(1); y++)
                {
                    var tileCode = buildingBlueprint[x, y];

                    //Need to place floor tile before wall tile
                    if (BuildingPrefabStore.WallTileKeys.ContainsKey(tileCode))
                    {
                        var tileType = BuildingPrefabStore.FloorTileKeys[tileCode];

                        var tile = BuildingPrefabStore.WoodenFloorTiles[tileType];

                        //Debug.Log($"Tile Code: {tileCode}    Tile Type: {tileType}");

                        _currentArea.AreaTiles[areaX, areaY].SetTileTexture(tile);
                        var instance = Instantiate(tile, new Vector2(areaX, areaY), Quaternion.identity);
                        instance.transform.SetParent(_areaMapHolderTransform);

                        tileType = BuildingPrefabStore.WallTileKeys[tileCode];
                        tile = BuildingPrefabStore.BrownStoneWallTiles[tileType];

                        _currentArea.AreaTiles[areaX, areaY].SetBlocksMovement(true);
                        instance = Instantiate(tile, new Vector2(areaX, areaY), Quaternion.identity);
                        instance.transform.SetParent(_areaMapHolderTransform);
                    }
                    else
                    {
                        //Debug.Log($"Tile Code: {tileCode} ");

                        if (tileCode != 'a')
                        {
                            continue;
                        }

                        var tileType = BuildingPrefabStore.FloorTileKeys[tileCode];

                        var tile = BuildingPrefabStore.WoodenFloorTiles[tileType];

                        //Debug.Log($"Tile Code: {tileCode}    Tile Type: {tileType}");

                        _currentArea.AreaTiles[areaX, areaY].SetTileTexture(tile);
                        var instance = Instantiate(tile, new Vector2(areaX, areaY), Quaternion.identity);
                        instance.transform.SetParent(_areaMapHolderTransform);
                    }
                    areaX++;
                }
            }*/
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
        gg.center = new Vector3(gg.width / 2 , gg.depth / 2 + Offset, -0.1f);
        gg.SetDimensions(gg.width, gg.depth, gg.nodeSize);
        gg.collision.use2D = true;
        gg.collision.type = ColliderType.Ray;
        gg.collision.mask.value = 256; //Set mask to obstacle        
        gg.rotation.x = -90;
        gg.cutCorners = false;
    }
}
