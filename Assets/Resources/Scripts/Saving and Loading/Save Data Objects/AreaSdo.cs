using System;
using System.Collections.Generic;

[Serializable]
public class AreaSdo
{
    public List<Entity> PresentEntities { get; set; }

    public List<FactionSdo> PresentFactions { get; set; }
    
    public BiomeType BiomeType { get; set; }

    public CellSdo ParentCellSdo { get; set; }

    public Tile[] AreaTiles { get; set; }

    public Queue<Entity> TurnOrder { get; set; }

    public SettlementSdo SettlementSdo;

    public int X;

    public int Y;

    public static Tile[] ConvertAreaTilesForSaving(Tile[,] map)
    {
        var index = 0;
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        var single = new Tile[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                single[index] = map[x, y];
                index++;
            }
        }
        return single;
    }
}
