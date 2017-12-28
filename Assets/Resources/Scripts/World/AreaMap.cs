using System.Collections.Generic;
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

        _playerSprite = Instantiate(GameManager.Instance.PlayerSprite, new Vector2(0, 0), Quaternion.identity);
    }

    private void Init()
    {
        AreaReady = false;
        AreaMapHolder = new GameObject("AreaMapHolder");

        //temp til movement and pathfinding is okay
        if (GameManager.Instance.Player == null)
        {
            _player = new Entity(true, _playerSprite);
            GameManager.Instance.Player = _player;
        }
        else
        {
            _player = GameManager.Instance.Player;
        }

        _areaMapHolderTransform = AreaMapHolder.transform;

        _currentArea = GameManager.Instance.CurrentAreaPosition;
        _currentArea.TurnOrder = new Queue<Entity>();
        _currentArea.TurnOrder.Enqueue(_player);
        _currentArea.BuildArea();
        DrawArea();
        PlacePlayer();
        if (_currentArea.EntitiesPresent())
        {
            PlaceNPCs();
        }
        CreateAStarGraph();
        AstarPath.active.Scan();
    }

    public void EnterArea()
    {
        Destroy(AreaMapHolder);
        Init();
        AreaReady = true;
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

    private void PlacePlayer() {
        if (GameManager.Instance.PlayerInStartingArea) {
            GameManager.Instance.PlayerInStartingArea = false;
            if (_currentArea != null) {
                _playerSprite.transform.position = new Vector3(_currentArea.Width / 2, _currentArea.Height / 2);
                _player.CurrentPosition = new Vector3(_currentArea.Width / 2, _currentArea.Height / 2);
                //Debug.Log(("current area: " + _currentArea.AreaTiles[_currentArea.Width / 2, _currentArea.Height / 2]));
                _currentArea.AreaTiles[_currentArea.Width / 2, _currentArea.Height / 2].SetPresentEntity(_player);
            }
        }
        else {
            //TODO place player based on where they entered map
            _playerSprite.transform.position = GameManager.Instance.Player.CurrentPosition;
        }
    }

    private void PlaceNPCs() {
        foreach (var e in _currentArea.PresentEntities) {
            var placed = false;
            var y = Random.Range(0, _currentArea.Height);
            var x = Random.Range(0, _currentArea.Width);
            while (!placed) {
                if (!_currentArea.AreaTiles[x, y].GetBlocksMovement()) {
                    var npcSprite = Instantiate(e.GetSprite(), new Vector3(x, y, 0f), Quaternion.identity);
                    e.SetSprite(npcSprite);
                    e.SetSpritePosition(new Vector3(x, y, 0f));
                    _currentArea.AreaTiles[x, y].SetPresentEntity(e);
                    e.CurrentPosition = new Vector3(x, y, 0f);
                    _currentArea.TurnOrder.Enqueue(e);
                    placed = true;
                }
                y = Random.Range(0, _currentArea.Height);
                x = Random.Range(0, _currentArea.Width);
            }
        }
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
