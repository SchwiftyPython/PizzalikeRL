using System;
using System.Collections.Generic;
using UnityEngine;

public enum WorldSpriteLayer
{
    Base,
    Detail,
    Mountain,
    River,
    Road,
    SettlementFloor,
    SettlementWall
}

public class WorldTile
{
    [Serializable]
    public class LayerPrefabIndexDictionary : SerializableDictionary<WorldSpriteLayer, int> { }

    public LayerPrefabIndexDictionary LayerPrefabIndexes;

    public Dictionary<WorldSpriteLayer, GameObject> Layers;

    public WorldTile()
    {
        LayerPrefabIndexes = new LayerPrefabIndexDictionary();
        Layers = new Dictionary<WorldSpriteLayer, GameObject>();

        var layers = (WorldSpriteLayer[])Enum.GetValues(typeof(WorldSpriteLayer));

        foreach (var layer in layers)
        {
            Layers.Add(layer, null);
        }
    }
}
