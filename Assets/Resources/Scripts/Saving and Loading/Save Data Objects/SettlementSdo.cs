using System;
using System.Collections.Generic;

[Serializable]
public class SettlementSdo
{
    public int Population;

    public CellSdo CellSdo;

    public int YearFounded;

    public string History;

    public List<Entity> NamedNpcs;

    public List<AreaSdo> AreaSdos;

    public List<Building> Buildings;

    public string Name;

    public SettlementSize Size;

    public List<Lot> Lots;

    public FactionSdo FactionSdo;
}
