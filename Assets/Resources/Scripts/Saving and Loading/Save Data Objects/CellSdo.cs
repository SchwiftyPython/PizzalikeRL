using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CellSdo
{
    public BiomeType BiomeType;

    public int X, Y;

    public List<River> Rivers = new List<River>();

    public AreaSdo[] AreaSdos;

    public Dictionary<Cell.WorldSpriteLayer, GameObject> WorldMapSprite { get; set; }

    public List<FactionSdo> PresentFactionSdos;

    public SettlementSdo SettlementSdo;

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
                    ParentCellSdo = parentCellSdo,
                    Settlement = currentArea.Settlement,
                    TurnOrder = currentArea.TurnOrder,
                    X = currentArea.X,
                    Y = currentArea.Y*/
                };

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

    public static List<AreaSdo> ConvertAreasForSaving(List<Area> areas, CellSdo parentCellSdo)
    {
        var sdos = new List<AreaSdo>();
        foreach (var area in areas)
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
                ParentCellSdo = parentCellSdo,
                Settlement = currentArea.Settlement,
                TurnOrder = currentArea.TurnOrder,
                X = currentArea.X,
                Y = currentArea.Y*/
            };
            sdos.Add(tempSdo);
        }
        return sdos;
    }
}
