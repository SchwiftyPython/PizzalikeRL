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
        if (cell == null)
        {
            return null;
        }

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


    //<Summary>
    // Converts to cell without any entity info
    //</Summary>
    public static Cell ConvertToBaseCell(CellSdo cellSdo)
    {
        if (cellSdo == null)
        {
            return null;
        }

        var cell = new Cell
        {
            biomeType = cellSdo.BiomeType,
            X = cellSdo.X,
            Y = cellSdo.Y,
            Id = cellSdo.Id,
            Rivers = RiverSdo.ConvertToRivers(cellSdo.RiverSdos)
        };
        return cell;
    }

    //<Summary>
    // Loads cell details that are not included when converting to base cell
    //</Summary>
    public static void LoadCellDetails(CellSdo cellSdo)
    {
        if (cellSdo == null)
        {
            return;
        }

        var cell = WorldData.Instance.MapDictionary[cellSdo.Id];

        //todo convert settlements
        cell.LoadCellSprite(cellSdo.WorldMapSpriteData);
        //todo get reference to present factions
        //todo convert areas
    }
}
