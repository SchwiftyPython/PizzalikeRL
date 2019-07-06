using System;
using System.Collections.Generic;

[Serializable]
public class SettlementSection 
{
    public char[,] Blueprint;

    public List<Lot> Lots;

    public List<Building> Buildings;

    public SettlementSection(){}
}
