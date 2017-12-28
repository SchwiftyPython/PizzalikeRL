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
    public Queue<Entity> TurnOrder { get; set; }

    private int _x;
    public int X {
        get {
            return _x;
        }

        set {
            _x = value;
        }
    }

    private int _y;
    public int Y {
        get {
            return _y;
        }

        set {
            _y = value;
        }
    }

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
                    if (Random.Range(0, 1000) < 1) {
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

    public bool AreaBuilt()
    {
        return AreaTiles != null;
    }

    public Tile GetTileAt(Vector3 position)
    {
        return AreaTiles[(int)position.x, (int)position.y];
    }
}
