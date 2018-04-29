using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaMap : MonoBehaviour {
    private Transform _areaMapHolderTransform;
    private Area _currentArea;
    private GameObject _playerSprite;
    private Entity _player;

    public GameObject AStar;
    public GameObject AreaMapHolder;
    public GameObject NPCSpriteHolder;
    public float GraphOffset = -0.5f;
    public bool AreaReady;

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
        DontDestroyOnLoad(gameObject);

        //_playerSprite = Instantiate(GameManager.Instance.PlayerSprite, new Vector2(0, 0), Quaternion.identity);
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
        }
        else if(_player == null || _player != GameManager.Instance.Player)
        {
            _player = GameManager.Instance.Player;
            _playerSprite = Instantiate(_player.GetSpritePrefab(), _player.CurrentPosition, Quaternion.identity);
            _playerSprite.transform.SetParent(GameManager.Instance.transform);
            _player.SetSprite(_playerSprite);
        }

        _areaMapHolderTransform = AreaMapHolder.transform;

        _currentArea = GameManager.Instance.CurrentArea;
        _currentArea.TurnOrder = new Queue<Entity>();
        _currentArea.TurnOrder.Enqueue(_player);
        _currentArea.BuildArea();
        DrawArea();
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
        if (AreaMapHolder != null || NPCSpriteHolder != null) {
            Deconstruct();
        }
        Init();
    }

    public void DrawArea() {
        for (var i = 0; i < _currentArea.Width; i++){
            for (var j = 0; j < _currentArea.Height; j++) {
                var texture = _currentArea.AreaTiles[i, j].GetTileTexture();
                var instance = Instantiate(texture, new Vector2(i, j), Quaternion.identity);
                instance.transform.SetParent(_areaMapHolderTransform);
            }
        }
    }

    public void RemoveEntity(Entity entity) {
        Destroy(entity.GetSprite());

        _currentArea.AreaTiles[(int)entity.CurrentPosition.x, (int)entity.CurrentPosition.y].SetBlocksMovement(false);
        _currentArea.AreaTiles[(int)entity.CurrentPosition.x, (int)entity.CurrentPosition.y].SetPresentEntity(null);
        _currentArea.PresentEntities.Remove(entity);
    }

    public void InstantiatePlayerSprite()
    {
        _playerSprite = Instantiate(_player.GetSpritePrefab(), _player.CurrentPosition, Quaternion.identity);
        _playerSprite.transform.SetParent(GameManager.Instance.transform);
        _player.SetSprite(_playerSprite);
    }

    private void Deconstruct() {
        RemoveAllNpcs();
        Destroy(AreaMapHolder);
    }

    private void PlacePlayer()
    {
        if (GameManager.Instance.PlayerInStartingArea)
        {
            GameManager.Instance.PlayerInStartingArea = false;
            if (_currentArea != null)
            {
                var placed = false;
                var y = _currentArea.Height / 2;
                var x = _currentArea.Width / 2;
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
                    y = Random.Range(0, _currentArea.Height);
                    x = Random.Range(0, _currentArea.Width);
                }
            }
        }
        else
        {
            _playerSprite.transform.position = GameManager.Instance.Player.CurrentPosition;
        }
        _currentArea?.PresentEntities.Add(_player);
    }

    private void PlaceNPCs()
    {
        NPCSpriteHolder = new GameObject("NPCSpriteHolder");
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

                    npcSprite.transform.SetParent(NPCSpriteHolder.transform);
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
        }
    }

    private void RemoveAllNpcs() {
        foreach (var e in _currentArea.PresentEntities.ToArray()) {
            RemoveEntity(e);
        }
        Destroy(NPCSpriteHolder);
    }

    private void CreateAStarGraph() {
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
