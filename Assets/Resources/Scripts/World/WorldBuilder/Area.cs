using System.Collections.Generic;
using UnityEngine;

public class Area {
    private GameObject[] _biomeTypeTiles;

    public List<Entity> PresentEntities { get; set; }
    public List<Faction> PresentFactions{ get; set; }

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
        if (AreaTiles != null)
        {
            return;
        }
        const int maxNpcsPlacedAtOnce = 9;
        PresentEntities = new List<Entity>();
        //PresentFactions = new List<Faction>();
        AreaTiles = new Tile[Width, Height];
        _biomeTypeTiles = WorldData.Instance.GetBiomeTiles(BiomeType);

        for (var i = 0; i < Width; i++)
        {
            for (var j = 0; j < Height; j++)
            {
                var texture = _biomeTypeTiles[Random.Range(0, _biomeTypeTiles.Length)];
                if (texture.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), true, true);
                }
                else
                {
                    AreaTiles[i, j] = new Tile(texture, new Vector2(i, j), false, false);

//                    //for testing
//                    const int maxNPCS = 5;
//                    if (Random.Range(0, 100) < 1 && PresentEntities.Count < maxNPCS) {
//                        var npcTypes = WorldData.Instance.BiomePossibleEntities[BiomeType];
//                        var npc = EntityTemplateLoader.GetEntityTemplate(npcTypes[0]);
//                        PresentEntities.Add(new Entity(npc));
//                    }
                }
            }
        }

        if (PresentFactions == null)
        {
            return;
        }

        foreach (var faction in PresentFactions)
        {
            var numNpcsToPlace = Random.Range(1, maxNpcsPlacedAtOnce + 1);

            for (var k = 0; k < numNpcsToPlace; k++)
            {
                PresentEntities.Add(k != numNpcsToPlace - 1
                    ? new Entity(faction.EntityType)
                    : faction.Leader);
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
