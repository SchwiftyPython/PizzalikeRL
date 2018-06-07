using UnityEngine;

public class WorldMap : MonoBehaviour
{

    private Transform _worldMapHolder;
    private Cell[,] _map;
    private int _mapHeight;
    private int _mapWidth;

    private GameObject _playerSprite;

    public GameObject Camera;

    public static WorldMap Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DrawMap();
        PlacePlayer();
    }

    private void DrawMap()
    {
        _worldMapHolder = transform;
        _map = WorldData.Instance.Map;
        _mapHeight = WorldData.Instance.Height;
        _mapWidth = WorldData.Instance.Width;

        for (var x = 0; x < _mapWidth; x++)
        {
            for (var y = 0; y < _mapHeight; y++)
            {
                var instance = Instantiate(_map[x, y].WorldMapSprite, new Vector2(x, y), Quaternion.identity);
                instance.transform.SetParent(_worldMapHolder);
            }
        }
    }

    private void PlacePlayer()
    {
        _playerSprite = GameManager.Instance.Player.GetSprite();
        _playerSprite.transform.position = new Vector3(GameManager.Instance.CurrentCell.X, GameManager.Instance.CurrentCell.Y);
        GameManager.Instance.Player.CurrentPosition = _playerSprite.transform.position;
        Camera.transform.SetParent(_playerSprite.transform);
        Camera.SetActive(true);
        Camera.transform.localPosition = new Vector3(0, 0, -10);
    }
}
