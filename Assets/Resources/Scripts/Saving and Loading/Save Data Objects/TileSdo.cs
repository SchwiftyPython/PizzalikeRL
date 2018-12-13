using UnityEngine;

public class TileSdo
{
    public Visibilities Visibility;

    public EntitySdo PresentEntitySdo;

    //public Prop PresentProp;

    public Item PresentItem;

    public Rarities Rarity;

    public int PrefabIndex;

    public bool BlocksMovement;

    public bool BlocksLight;

    public Vector2 GridPosition;

    public string Id;

    public bool Revealed;

    public Lot Lot;

    //public GameObject PresentWallTile { get; set; } //todo WallTile object with type and index

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
            PresentEntitySdo = EntitySdo.ConvertToEntitySdo(tile.GetPresentEntity()),
            PresentItem = tile.PresentItem,
            Rarity = tile.Rarity,
            PrefabIndex = tile.PrefabIndex,
            BlocksMovement = tile.GetBlocksMovement(),
            BlocksLight = tile.GetBlocksLight(),
            GridPosition = tile.GridPosition,
            Id = tile.Id,
            Revealed = tile.Revealed,
            Lot = tile.Lot
        };
    }
}
