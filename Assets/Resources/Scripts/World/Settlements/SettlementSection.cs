using System.Collections.Generic;

public class SettlementSection 
{
    public List<Lot> Lots;

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
    }
}
