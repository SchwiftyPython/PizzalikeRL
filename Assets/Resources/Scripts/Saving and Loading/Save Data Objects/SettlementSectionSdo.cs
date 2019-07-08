using System.Collections.Generic;

public class SettlementSectionSdo
{
    public List<LotSdo> LotSdos;

    public List<BuildingSdo> BuildingSdos;

    public SettlementSectionSdo(SettlementSection section)
    {
        LotSdos = LotSdo.ConvertToLotSdos(section.Lots);
        BuildingSdos = BuildingSdo.ConvertToBuildingSdos(section.Buildings);
    }
}
