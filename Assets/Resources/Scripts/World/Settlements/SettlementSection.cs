using System.Collections.Generic;

public class SettlementSection 
{
    public List<Lot> Lots;

    public List<Building> Buildings;

    public SettlementSection()
    {
    }

    public SettlementSection(SettlementSectionSdo sdo)
    {
        if (sdo == null)
        {
            return;
        }

        Lots = LotSdo.ConvertToLots(sdo.LotSdos);
        //Buildings = BuildingSdo.ConvertToBuildings(sdo.BuildingSdos); //lotsdo has assigned building already
    }
}
