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

    public WorldTile.LayerPrefabIndexDictionary WorldMapSpriteData { get; set; }

    public List<string> PresentFactionNames;

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
            PresentFactionNames = new List<string>()
        };

        if (cell.PresentFactions != null)
        {
            foreach (var faction in cell.PresentFactions)
            {
                sdo.PresentFactionNames.Add(faction.Name);
            }
        }
        
        sdo.AreaSdos = AreaSdo.ConvertAreasForSaving(cell.Areas);
        sdo.SettlementSdo = cell.Settlement?.GetSettlementSdo();

        if (sdo.SettlementSdo != null)
        {
            foreach (var areaSdo in sdo.AreaSdos)
            {
                if (cell.Areas[areaSdo.X, areaSdo.Y].Settlement != null)
                {
                    areaSdo.SettlementSdo = sdo.SettlementSdo;
                }
            }
        }

        return sdo;
    }
}
