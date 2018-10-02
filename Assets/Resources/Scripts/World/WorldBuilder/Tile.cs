using System;
using System.Collections;
using System.Collections.Generic;
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
    private Vector2 _gridPosition;

    private bool _blocksMovement;
    private bool _blocksLight;

    public GameObject FovTile;

    public bool Revealed;
    public GameObject PresentWallTile { get; set; }

    public Visibilities Visibility
    {
        get { return _visibility; }

        set
        {
            _visibility = value;

            if (TextureInstance == null)
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
        _gridPosition = position;
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
        _gridPosition = position;
    }

    public Vector2 GetGridPosition()
    {
        return _gridPosition;
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

//        //Hack to deal with floor tiles under walls -- doesn't work. Going to make walls take up entire tile for simplicity
//        if (PresentWallTile != null && TextureInstance.name.IndexOf("floor", StringComparison.OrdinalIgnoreCase) != -1)
//        {
//            TextureInstance.GetComponent<SpriteRenderer>().color = color;
//        }

        //TextureInstance.GetComponent<SpriteRenderer>().color = color;

        /*if (_presentEntity != null && !_presentEntity.IsPlayer())
        {
            _presentEntity.GetSprite().GetComponent<SpriteRenderer>().color = color;
        }
        if (PresentWallTile != null)
        {
            PresentWallTile.GetComponent<SpriteRenderer>().color = color;
        }
        if (_presentItem != null)
        {
            //todo
            //_presentItem.GetComponent<SpriteRenderer>().color = color;
        }
        if (_presentProp != null)
        {
            //todo
            //_presentProp.GetComponent<SpriteRenderer>().color = color;
        }*/
    }
}
