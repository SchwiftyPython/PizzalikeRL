using System;
using System.Collections.Generic;
using UnityEngine;

public enum Visibilities
{
    Invisible,
    Visible
}

[Serializable]
public class Tile
{
    private Visibilities _visibility;

    private Entity _presentEntity;
    private List<Item> _presentItems;

    private Rarities _rarity;
    private string _prefabName;
    private GameObject _prefabTexture;

    private bool _blocksMovement;
    private bool _blocksLight;

    public Tile Left;
    public Tile Right;
    public Tile Top;
    public Tile Bottom;

    public int X, Y;             //array positions     
    public Vector2 GridPosition; //on screen position
    public string Id;

    public GameObject FovTile;
    public GameObject FovWallTile;
    public GameObject FovWallTilePrefab;

    public bool Revealed;
    public Lot Lot;
    public GameObject PresentWallTile { get; set; }

    public Topping PresentTopping { get; set; }
    public Prop PresentProp { get; set; }

    public GameObject AbilityTexture;

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

    public List<Item> PresentItems
    {
        get { return _presentItems; }

        set { _presentItems = value; }
    }

    public Rarities Rarity
    {
        get { return _rarity; }

        set { _rarity = value; }
    }

    public string PrefabName
    {
        get { return _prefabName; }

        set { _prefabName = value; }
    }

    public GameObject TextureInstance;

    public Tile()
    { }

    public Tile(GameObject texture, Vector2 position, bool blocksMovement, bool blocksLight)
    {
        _prefabTexture = texture;
        _prefabName = texture.name;
        X = (int) position.x;
        Y = (int) position.y;
        GridPosition = new Vector2(Y, X);
        _blocksMovement = blocksMovement;
        _blocksLight = blocksLight;
        Id = X + " " + Y;
        PresentItems = new List<Item>();
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
    }
}
