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

    public SettlementSectionSdo SettlementSectionSdo;

    public string ParentCellId;

    public int X;

    public int Y;

    public static AreaSdo[] ConvertAreasForSaving(Area[,] areas)
    {
        var height = areas.GetLength(0);
        var width = areas.GetLength(1);

        var convertedAreas = new AreaSdo[height, width];

        for (var currentRow = 0; currentRow < height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < width; currentColumn++)
            {
                var currentArea = areas[currentRow, currentColumn];

                var tempSdo = ConvertAreaForSaving(currentArea);
                convertedAreas[currentRow, currentColumn] = tempSdo;
            }
        }

        var index = 0;
        var single = new AreaSdo[height * width];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                single[index] = convertedAreas[row, column];
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
        AreaSdo tempSdo;
        tempSdo = new AreaSdo();
        tempSdo.PresentEntityIds = new List<Guid>();
        tempSdo.AreaTiles = area.AreaBuilt()
            ? TileSdo.ConvertAreaTilesForSaving(area.AreaTiles)
            : null;
        tempSdo.BiomeType = area.BiomeType;
        tempSdo.PresentFactionNames = new List<string>();
        tempSdo.TurnOrderIds = new Queue<Guid>();
        tempSdo.X = area.X;
        tempSdo.Y = area.Y;
        tempSdo.ParentCellId = area.ParentCell.Id;
        tempSdo.SettlementSdo = area.Settlement?.GetSettlementSdo();
        tempSdo.SettlementSectionSdo = new SettlementSectionSdo(area.SettlementSection);

        if (area.PresentEntities != null)
        {
            foreach (var entity in area.PresentEntities)
            {
                tempSdo.PresentEntityIds.Add(entity.Id);
            }
        }

        if (area.TurnOrder != null)
        {
            foreach (var entity in area.TurnOrder)
            {
                tempSdo.TurnOrderIds.Enqueue(entity.Id);
            }
        }

        return tempSdo;
    }

    public static Area[,] ConvertAreasForPlaying(AreaSdo[] sdos)
    {
        var areas = new Area[3, 3];

        foreach (var sdo in sdos)
        {
            var area = new Area();
            area.X = sdo.X;
            area.Y = sdo.Y;
            area.PresentEntities = new List<Entity>();
            area.AreaTiles = sdo.AreaTiles != null
                ? TileSdo.ConvertToAreaTiles(sdo.AreaTiles, area.Height, area.Width, area.BiomeType)
                : null;
            area.BiomeType = sdo.BiomeType;
            area.PresentFactions = new List<Faction>();
            area.TurnOrder = new Queue<Entity>();
            area.ParentCell = WorldData.Instance.MapDictionary[sdo.ParentCellId];

            if (sdo.PresentEntityIds.Count > 0)
            {
                foreach (var id in sdo.PresentEntityIds)
                {
                    var entity = WorldData.Instance.Entities[id];
                    area.PresentEntities.Add(entity);
                    entity.CurrentArea = area;
                    entity.CurrentCell = area.ParentCell;
                }

                foreach (var id in sdo.TurnOrderIds)
                {
                    area.TurnOrder.Enqueue(WorldData.Instance.Entities[id]);
                }
            }
            areas[area.X, area.Y] = area;
        }
        return areas;
    }
}
