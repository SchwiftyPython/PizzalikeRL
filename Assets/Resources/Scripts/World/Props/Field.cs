using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FieldType
{
    Wheat
}

[Serializable]
public class Field : Prop
{
    public FieldType Type;

    public Field(FieldType type, GameObject prefab) : base(prefab)
    {
        Type = type;
    }

    public Field(FieldSdo sdo)
    {
        Type = sdo.Type;

        GameObject[] tiles;

        if (Type == FieldType.Wheat)
        {
            tiles = WorldData.Instance.WheatFieldTiles;
        }
        else
        {
            Debug.Log($@"Invalid Field Type! {Type}");
            throw new ArgumentException($@"Invalid Field Type! {Type}");
        }

        Prefab = tiles[Random.Range(0, tiles.Length)];
        IsContainer = false;
    }
}
