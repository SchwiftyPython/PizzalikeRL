using System;
using System.Collections.Generic;

[Serializable]
public class SettlementSdo
{
    public CellSdo CellSdo;

    public List<AreaSdo> AreaSdos;

    public int Population;

    public int YearFounded;

    public string History;

    public List<EntitySdo> CitizenSdos;

    public List<Building> Buildings;

    public string Name;

    public SettlementSize Size;

    public List<Lot> Lots;

    public FactionSdo FactionSdo;
}
