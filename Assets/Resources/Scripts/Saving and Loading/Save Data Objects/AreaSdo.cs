using System;
using System.Collections.Generic;

[Serializable]
public class AreaSdo
{
    public List<Entity> PresentEntities { get; set; }

    public List<FactionSdo> PresentFactions { get; set; }
    
    public BiomeType BiomeType { get; set; }

    public Tile[] AreaTiles { get; set; }

    public Queue<Entity> TurnOrder { get; set; }

    public SettlementSdo SettlementSdo;

    public int X;

    public int Y;

    public static AreaSdo[] ConvertAreasForSaving(Area[,] areas, CellSdo parentCellSdo)
    {
        var width = areas.GetLength(0);
        var height = areas.GetLength(1);

        var convertedAreas = new AreaSdo[width, height];

        for (var currentRow = 0; currentRow < height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < width; currentColumn++)
            {
                var currentArea = areas[currentColumn, currentRow];

                var tempSdo = ConvertAreaForSaving(currentArea);
                convertedAreas[currentColumn, currentRow] = tempSdo;
            }
        }

        var index = 0;
        var single = new AreaSdo[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                single[index] = convertedAreas[x, y];
                index++;
            }
        }
        return single;
    }

    public static List<AreaSdo> ConvertAreasForSaving(List<Area> areas)
    {
        var sdos = new List<AreaSdo>();
        foreach (var area in areas)
        {
            var tempSdo = ConvertAreaForSaving(area);
            sdos.Add(tempSdo);
        }
        return sdos;
    }

    public static AreaSdo ConvertAreaForSaving(Area areas)
    {
        var tempSdo = new AreaSdo
        {
            //PresentEntities = currentArea.PresentEntities,
            /*AreaTiles = currentArea.AreaBuilt()
                ? AreaSdo.ConvertAreaTilesForSaving(currentArea.AreaTiles)
                : null,
            BiomeType = currentArea.BiomeType,
            PresentFactions = currentArea.PresentFactions == null
                ? null
                : FactionSdo.ConvertToFactionSdos(currentArea.PresentFactions),
            
            Settlement = currentArea.Settlement,
            TurnOrder = currentArea.TurnOrder,
            X = currentArea.X,
            Y = currentArea.Y*/
        };
        return tempSdo;
    }
}
