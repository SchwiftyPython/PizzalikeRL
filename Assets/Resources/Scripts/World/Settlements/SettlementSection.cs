using System.Collections.Generic;

public class SettlementSection 
{
    public List<Lot> Lots;

    public List<Building> Buildings;

    public SettlementSection(SettlementSectionSdo sdo)
    {
        Lots = LotSdo.ConvertToLots(sdo.LotSdos);
        Buildings = BuildingSdo.ConvertToBuildings(sdo.BuildingSdos);
    }
}
