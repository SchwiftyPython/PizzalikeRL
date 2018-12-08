using System;
using System.Collections.Generic;

[Serializable]
public class CellSdo
{
    public BiomeType BiomeType;

    public int X, Y;

    public string Id;

    public List<RiverSdo> RiverSdos = new List<RiverSdo>();

    public AreaSdo[] AreaSdos;

    public Dictionary<WorldSpriteLayer, int> WorldMapSpriteData { get; set; }

    public List<FactionSdo> PresentFactionSdos;

    public SettlementSdo SettlementSdo;

    public static CellSdo ConvertToCellSdo(Cell cell)
    {
        var sdo = new CellSdo
        {
            BiomeType = cell.BiomeType,
            X = cell.X,
            Y = cell.Y,
            Id = cell.Id,
            RiverSdos = RiverSdo.ConvertToRiverSdos(cell.Rivers),
            WorldMapSpriteData = cell.WorldMapSprite.LayerPrefabIndexes,
            PresentFactionSdos = cell.PresentFactions == null
                ? null
                : FactionSdo.ConvertToFactionSdos(cell.PresentFactions)
        };

        sdo.AreaSdos = AreaSdo.ConvertAreasForSaving(cell.Areas, sdo);
        sdo.SettlementSdo = cell.Settlement?.GetSettlementSdo(sdo);

        return sdo;
    }
}
