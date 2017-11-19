using System.Collections.Generic;
using UnityEngine;

public class Area {
    private GameObject[] _biomeTypeTiles;

    public List<Entity> PresentEntities { get; set; }

    public int Width = 80;
    public int Height = 25;

    //TODO: Could probably just reference parent cell for BiomeType
    public BiomeType BiomeType { get; set; }

    public Tile[,] AreaTiles { get; set; }

    public void BuildArea()
    {
        PresentEntities = new List<Entity>();
        if (AreaTiles != null) return;
        AreaTiles = new Tile[Width, Height];
        _biomeTypeTiles = WorldData.Instance.GetBiomeTiles(BiomeType);
        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var texture = _biomeTypeTiles[Random.Range(0, _biomeTypeTiles.Length)];
                if (texture.name.Equals("pizza_wall"))
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), true, true);
                }
                else
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), false, false);
                    //for testing
                    if (Random.Range(0, 100) < 10) {
                        var npcTypes = WorldData.Instance.BiomePossibleEntities[BiomeType];
                        var npc = EntityTemplateLoader.GetEntityTemplate(npcTypes[0]);
                        PresentEntities.Add(new Entity(npc, false));
                    }
                }
            }
        }
        
    }

    public bool EntitiesPresent()
    {
        return PresentEntities.Count > 0;
    }
}
