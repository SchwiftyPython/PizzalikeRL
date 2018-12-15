using System;
using System.Collections.Generic;

[Serializable]
public class AreaSdo
{
    public List<Guid> PresentEntityIds { get; set; }

    public List<string> PresentFactionNames;

    public BiomeType BiomeType { get; set; }

    public TileSdo[] AreaTiles { get; set; }

    public Queue<Guid> TurnOrderIds { get; set; }

    public SettlementSdo SettlementSdo;

    public string ParentCellId;

    public int X;

    public int Y;

    public static AreaSdo[] ConvertAreasForSaving(Area[,] areas)
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

    public static AreaSdo ConvertAreaForSaving(Area area)
    {
        var tempSdo = new AreaSdo
        {
            PresentEntityIds = new List<Guid>(),
            AreaTiles = area.AreaBuilt()
                ? TileSdo.ConvertAreaTilesForSaving(area.AreaTiles)
                : null,
            BiomeType = area.BiomeType,
            PresentFactionNames = new List<string>(),
            TurnOrderIds = new Queue<Guid>(),
            X = area.X,
            Y = area.Y,
            ParentCellId = area.ParentCell.Id
        };

        if (area.TurnOrder != null)
        {
            foreach (var entity in area.TurnOrder)
            {
                tempSdo.TurnOrderIds.Enqueue(entity.Id);
            }
        }

        return tempSdo;
    }
}
