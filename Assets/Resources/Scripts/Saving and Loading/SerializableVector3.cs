﻿using UnityEngine;
using System;

/// <summary>
/// Since unity doesn't flag the Vector3 as serializable, we
/// need to create our own version. This one will automatically convert
/// between Vector3 and SerializableVector3
/// </summary>
[Serializable]
public struct SerializableVector3
{
    /// <summary>
    /// x component
    /// </summary>
    public float X;

    /// <summary>
    /// y component
    /// </summary>
    public float Y;

    /// <summary>
    /// z component
    /// </summary>
    public float Z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3(float rX, float rY, float rZ)
    {
        X = rX;
        Y = rY;
        Z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"[{X}, {Y}, {Z}]";
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.X, rValue.Y, rValue.Z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector2
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector2(SerializableVector3 rValue)
    {
        return new Vector2(rValue.X, rValue.Y);
    }

    /// <summary>
    /// Automatic conversion from Vector2 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(Vector2 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, 0);
    }
}