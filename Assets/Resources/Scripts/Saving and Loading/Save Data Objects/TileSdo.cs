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

    public string PrefabName;

    public bool BlocksMovement;

    public bool BlocksLight;

    public SerializableVector3 GridPosition;

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
            PrefabName = tile.PrefabName,
            BlocksMovement = tile.GetBlocksMovement(),
            BlocksLight = tile.GetBlocksLight(),
            GridPosition = tile.GridPosition,
            Id = tile.Id,
            Revealed = tile.Revealed,
            LotSdo = LotSdo.ConvertToLotSdo(tile.Lot)
        };
    }

    public static Tile[,] ConvertToAreaTiles(TileSdo[] sdos, int height, int width)
    {
        var areaTiles = new Tile[height, width];

        foreach (var sdo in sdos)
        {
            var tile = ConvertToAreaTile(sdo);

            areaTiles[tile.X, tile.Y] = tile;
        }

        return areaTiles;
    }

    public static Tile ConvertToAreaTile(TileSdo sdo)
    {
        var tile = new Tile();
        var id = sdo.Id.Split(' ');

        tile.X = Convert.ToInt32(id[0]);
        tile.Y = Convert.ToInt32(id[1]);
        tile.PrefabName = sdo.PrefabName;
        tile.SetPrefabTileTexture(Resources.Load<GameObject>($"/Prefabs/{tile.PrefabName}"));
        tile.Visibility = sdo.Visibility;
        tile.SetPresentEntity(sdo.PresentEntityId != Guid.Empty
            ? WorldData.Instance.Entities[sdo.PresentEntityId]
            : null);
        tile.PresentItem = sdo.PresentItemId != Guid.Empty ? WorldData.Instance.Items[sdo.PresentItemId] : null;
        tile.Rarity = sdo.Rarity;
        tile.SetBlocksMovement(sdo.BlocksMovement);
        tile.SetBlocksLight(sdo.BlocksLight);
        tile.GridPosition = sdo.GridPosition;
        tile.Revealed = sdo.Revealed;

        return tile;
    }
}
