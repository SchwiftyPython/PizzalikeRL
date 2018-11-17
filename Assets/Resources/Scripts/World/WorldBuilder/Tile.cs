using UnityEngine;

public class Tile
{
    public enum Visibilities
    {
        Invisible,
        Visible
    }

    private Visibilities _visibility;

    private Entity _presentEntity;
    private Prop _presentProp;
    private Item _presentItem;
    private GameObject _prefabTexture;

    private bool _blocksMovement;
    private bool _blocksLight;

    public Tile Left;
    public Tile Right;
    public Tile Top;
    public Tile Bottom;

    public Vector2 GridPosition;

    public GameObject FovTile;
    public GameObject FovWallTile;
    public GameObject FovWallTilePrefab;

    public bool Revealed;
    public Lot Lot;
    public GameObject PresentWallTile { get; set; }

    public Visibilities Visibility
    {
        get { return _visibility; }

        set
        {
            _visibility = value;

            if (TextureInstance == null && PresentWallTile == null)
            {
                return;
            }

            SetTileVisibility(_visibility);
        }
    }

    public GameObject TextureInstance;

    public Tile()
    { }

    public Tile(GameObject texture, Vector2 position, bool blocksMovement, bool blocksLight)
    {
        _prefabTexture = texture;
        GridPosition = position;
        _blocksMovement = blocksMovement;
        _blocksLight = blocksLight;
    }

    public void SetPresentEntity(Entity entity)
    {
        _presentEntity = entity;
    }

    public Entity GetPresentEntity()
    {
        return _presentEntity;
    }

    public void SetGridPosition(Vector2 position)
    {
        GridPosition = position;
    }

    public Vector2 GetGridPosition()
    {
        return GridPosition;
    }

    public void SetPrefabTileTexture(GameObject texture)
    {
        _prefabTexture = texture;
    }

    public GameObject GetPrefabTileTexture()
    {
        return _prefabTexture;
    }

    public void SetBlocksMovement(bool blocksMovement)
    {
        _blocksMovement = blocksMovement;
    }

    public bool GetBlocksMovement()
    {
        return _blocksMovement;
    }

    public void SetBlocksLight(bool blocksLight)
    {
        _blocksLight = blocksLight;
    }

    public bool GetBlocksLight()
    {
        return _blocksLight;
    }

    public bool IsWall()
    {
        return PresentWallTile != null;
    }

    private void SetTileVisibility(Visibilities visibility)
    {
        var visibleColor = new Color(1, 1, 1, 0);
        var revealedColor = new Color(0f, 0f, 0f, 0.5f);
        var invisibleColor = Color.black;

        var color = visibleColor;
        if (visibility != Visibilities.Visible)
        {
            color = Revealed ? revealedColor : invisibleColor;
        }

        FovTile.GetComponent<SpriteRenderer>().color = color;

        //todo will need to refine this based on player pos vs wall. Should work for now.
//        if (IsWall())
//        {
//            Debug.Log("Wall tile: " + visibility);
//        }
    }
}
