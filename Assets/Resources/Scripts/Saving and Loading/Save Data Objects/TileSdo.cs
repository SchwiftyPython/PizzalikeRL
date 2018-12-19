using System;
using UnityEngine;

[Serializable]
public class TileSdo
{
    public Visibilities Visibility;

    public Guid PresentEntityId;

    //public Prop PresentProp;

    public Guid PresentItemId;

    public Rarities Rarity;

    public int PrefabIndex;

    public bool BlocksMovement;

    public bool BlocksLight;

    public Vector2 GridPosition;

    public string Id;

    public bool Revealed;

    public LotSdo LotSdo;
    
    public static TileSdo[] ConvertAreaTilesForSaving(Tile[,] map)
    {
        var index = 0;
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var single = new TileSdo[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                single[index] = ConvertToTileSdo(map[x, y]);
                index++;
            }
        }
        return single;
    }

    public static TileSdo ConvertToTileSdo(Tile tile)
    {
        return new TileSdo
        {
            Visibility = tile.Visibility,
            PresentEntityId = tile.GetPresentEntity() != null
                ? tile.GetPresentEntity().Id
                : Guid.Empty,
            PresentItemId = tile.PresentItem?.Id ?? Guid.Empty,
            Rarity = tile.Rarity,
            PrefabIndex = tile.PrefabIndex,
            BlocksMovement = tile.GetBlocksMovement(),
            BlocksLight = tile.GetBlocksLight(),
            GridPosition = tile.GridPosition,
            Id = tile.Id,
            Revealed = tile.Revealed,
            LotSdo = LotSdo.ConvertToLotSdo(tile.Lot)
        };
    }
}
