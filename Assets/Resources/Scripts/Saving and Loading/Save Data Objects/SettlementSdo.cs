using System;
using System.Collections.Generic;

[Serializable]
public class SettlementSdo
{
    public string CellId;

    public List<string> AreaIds;
 
    public int Population;

    public int YearFounded;

    public string History;

    public List<Guid> CitizenIds;

    public List<BuildingSdo> BuildingSdos;

    public string Name;

    public SettlementSize Size;

    public List<LotSdo> LotSdos;

    public string FactionName;
}
