using System.Collections.Generic;

public class Settlement
{
    private int _population;
    private SettlementSize _size;
    private Cell _cell;
    private Faction _faction;
    private int _yearFounded;
    private string _history;
    private List<Entity> _namedNpcs;

    public string Name;

    public Settlement(Faction faction, SettlementSize size, Cell cell, int population)
    {
        _faction = faction;
        _size = size;
        _cell = cell;
        _population = population;
    }
}
