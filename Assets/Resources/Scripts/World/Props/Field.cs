using System;
using UnityEngine;

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
}
