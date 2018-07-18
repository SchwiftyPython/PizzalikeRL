using System.Collections.Generic;

public class SettlementPrefab
{
    public char[,] Blueprint;

    private List<Lot> _lots;

    public SettlementPrefab(char [,] blueprint)
    {
        Blueprint = blueprint;
        _lots = new List<Lot>();
    }

}
