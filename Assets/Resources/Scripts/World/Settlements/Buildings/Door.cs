using UnityEngine;

public class Door : MonoBehaviour
{
    private enum DoorState
    {
        Open,
        Closed
    }

    private DoorState _currentState;

    public Tile CurrentTile;
    public GameObject ClosedPrefab;
    public GameObject OpenPrefab;

    private void Start()
    {
        _currentState = DoorState.Closed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_currentState == DoorState.Open)
        {
            return;
        }

        transform.GetComponent<SpriteRenderer>().sprite =
            OpenPrefab.GetComponent<SpriteRenderer>().sprite;

        Destroy(transform.GetComponent<Collider2D>());
        Destroy(transform.GetComponent<Rigidbody2D>());

        CurrentTile.SetBlocksLight(false);

        _currentState = DoorState.Open;
    }
}
