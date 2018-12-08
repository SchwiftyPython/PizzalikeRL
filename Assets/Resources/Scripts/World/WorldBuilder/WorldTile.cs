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
    public Dictionary<WorldSpriteLayer, int> LayerPrefabIndexes;

    public Dictionary<WorldSpriteLayer, GameObject> Layers;

    public WorldTile()
    {
        LayerPrefabIndexes = new Dictionary<WorldSpriteLayer, int>();
        Layers = new Dictionary<WorldSpriteLayer, GameObject>();

        var layers = (WorldSpriteLayer[])Enum.GetValues(typeof(WorldSpriteLayer));

        foreach (var layer in layers)
        {
            Layers.Add(layer, null);
        }
    }
}
