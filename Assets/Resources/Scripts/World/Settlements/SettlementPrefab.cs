using System.Collections.Generic;

public class SettlementPrefab
{
    public char[,] Blueprint;

    public List<Lot> Lots;

    public SettlementPrefab(char [,] blueprint)
    {
        Blueprint = blueprint;
        Lots = new List<Lot>();
    }
}
