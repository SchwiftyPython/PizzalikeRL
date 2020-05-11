using System;
using System.Collections.Generic;

[Serializable]
public class SettlementSectionSdo
{
    public List<LotSdo> LotSdos;

    public List<BuildingSdo> BuildingSdos;

    public SettlementSectionSdo(SettlementSection section)
    {
        if (section == null)
        {
            return;
        }

        LotSdos = LotSdo.ConvertToLotSdos(section.Lots);
        BuildingSdos = BuildingSdo.ConvertToBuildingSdos(section.Buildings);
    }
}
