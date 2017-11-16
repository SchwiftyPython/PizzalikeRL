using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    private GameObject[] _biomeTypeTiles;
    private Entity[] _presentEntities;

    public int Width = 80;
    public int Height = 25;

    //TODO: Could probably just reference parent cell for BiomeType
    public BiomeType BiomeType { get; set; }

    public Tile[,] AreaTiles { get; set; }

    public void BuildArea()
    {
        if (AreaTiles != null) return;
        AreaTiles = new Tile[Height, Width];
        _biomeTypeTiles = WorldData.Instance.GetBiomeTiles(BiomeType);
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var texture = _biomeTypeTiles[Random.Range(0, _biomeTypeTiles.Length)];
                if (texture.name.Equals("area_wall_1"))
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), true, true);
                }
                else
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), false, false);
                }
            }
        }
    }

    public bool EntitiesPresent()
    {
        return _presentEntities.Length > 0;
    }
}
