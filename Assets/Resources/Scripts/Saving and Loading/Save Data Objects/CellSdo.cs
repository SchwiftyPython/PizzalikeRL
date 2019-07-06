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
                if (cell.Areas[areaSdo.X, areaSdo.Y].SettlementSection != null)
                {
                    areaSdo.SettlementSdo = sdo.SettlementSdo;
                }
            }
        }

        return sdo;
    }
    
    public static Cell ConvertToCell(CellSdo cellSdo)
    {
        if (cellSdo == null)
        {
            return null;
        }

        var cell = WorldData.Instance.MapDictionary[cellSdo.Id];

        cell.biomeType = cellSdo.BiomeType;
        cell.Rivers = RiverSdo.ConvertToRivers(cellSdo.RiverSdos);
        cell.Settlement = new Settlement(cellSdo.SettlementSdo);
        cell.LoadCellSprite(cellSdo.WorldMapSpriteData);

        cell.PresentFactions = new List<Faction>();

        foreach (var factionName in cellSdo.PresentFactionNames)
        {
            cell.PresentFactions.Add(WorldData.Instance.Factions[factionName]);
        }

        cell.Areas = AreaSdo.ConvertAreasForPlaying(cellSdo.AreaSdos);

        return cell;
    }
}
