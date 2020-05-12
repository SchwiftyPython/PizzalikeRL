using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Settlement
{
    private readonly IDictionary<SettlementSize, int> _numAreasForSettlementSize = new Dictionary<SettlementSize, int>
    {
        { SettlementSize.Outpost, 1 },
        { SettlementSize.Hamlet, 2 },
        { SettlementSize.Village, 3 },
        { SettlementSize.SmallCity, 4 },
        { SettlementSize.Fortress, 9 },
        { SettlementSize.LargeCity, 9 }
    };

    private int _population;
    private readonly Cell _cell;
    private int _yearFounded;
    private string _history;
    private IDictionary<Area, SettlementSection> _areas;

    public string Name;
    public readonly SettlementSize Size;
    public Faction Faction;
    public List<Entity> Citizens { get; }

    public Settlement(Faction faction, SettlementSize size, Cell cell, int population, bool isStartingArea = false)
    {
        Faction = faction;
        Size = size;
        Citizens = new List<Entity>();
        _cell = cell;
        _population = population;

        Name = SettlementPrefabStore.GenerateName();

        while (WorldData.Instance.Settlements.ContainsKey(Name))
        {
            Name = SettlementPrefabStore.GenerateName();
        }

        WorldData.Instance.Settlements.Add(Name, this);

        if (isStartingArea)
        {
            _areas = new Dictionary<Area, SettlementSection>();
            var area = _cell.Areas[1, 1];
            area.Settlement = this;
            area.SettlementSection = new SettlementSection();
            _areas.Add(area, area.SettlementSection);
        }
        else
        {
            PickAreas();
        }
    }

    public bool IsBuilt()
    {
        return _areas.Keys.All(area => area.AreaBuilt());
    }

    public void Build()
    {
        BuildAreas();
    }

    public Settlement(SettlementSdo sdo)
    {
        if (sdo == null)
        {
            return;
        }

        Faction = WorldData.Instance.Factions[sdo.FactionName];
        Size = sdo.Size;
        Name = sdo.Name;
        _cell = WorldData.Instance.MapDictionary[sdo.CellId];
        _population = sdo.Population;
        _history = sdo.History;

        Citizens = new List<Entity>();

        foreach (var id in sdo.CitizenIds)
        {
            Citizens.Add(WorldData.Instance.Entities[id]);
        }

        _areas = new Dictionary<Area, SettlementSection>();

        foreach (var id in sdo.AreaIds)
        {
            var splitId = id.Split(' ');

            var area = _cell.Areas[int.Parse(splitId[0]), int.Parse(splitId[1])];

            _areas.Add(new KeyValuePair<Area, SettlementSection>(area, area.SettlementSection));
        }
    }

    public SettlementSdo GetSettlementSdo()
    {
        SettlementSdo sdo;
        sdo = new SettlementSdo();
        sdo.CellId = _cell.Id;
        sdo.Population = _population;
        sdo.FactionName = Faction?.Name;
        sdo.History = _history;
        sdo.Name = Name;
        sdo.CitizenIds = new List<Guid>();
        sdo.Size = Size;
        sdo.AreaIds = new List<string>();
        sdo.SectionSdos = new List<SettlementSectionSdo>();

        foreach (var area in _areas.Keys)
        {
            sdo.AreaIds.Add(area.Id);
        }

        foreach (var section in _areas.Values)
        {
            sdo.SectionSdos.Add(new SettlementSectionSdo(section));
        }

        if (Citizens != null)
        {
            foreach (var citizen in Citizens)
            {
                sdo.CitizenIds.Add(citizen.Id);
            }
        }

        return sdo;
    }

    private void PickAreas()
    {
        _areas = new Dictionary<Area, SettlementSection>();
        for (var i = 0; i < _numAreasForSettlementSize[Size]; i++)
        {
            var settlementPlaced = false;
            while (!settlementPlaced)
            {
                var x = Random.Range(0, _cell.GetCellHeight());
                var y = Random.Range(0, _cell.GetCellWidth());

                if (_areas.Count > 0 && !AreaIsAdjacentToAnotherSettlementArea(x, y))
                {
                    continue;
                }

                var area = _cell.Areas[x, y];

                if (_areas.ContainsKey(area))
                {
                    continue;
                }

                area.Settlement = this;
                area.SettlementSection = new SettlementSection();

                if (area.PresentFactions == null)
                {
                    area.PresentFactions = new List<Faction>();
                }

                area.PresentFactions.Add(Faction);
                _areas.Add(area, area.SettlementSection);
                settlementPlaced = true;
            }
        }
    }

    private bool AreaIsAdjacentToAnotherSettlementArea(int x, int y)
    {
        foreach (var area in _areas.Keys)
        {
            var xDelta = Math.Abs(area.X - x);
            var yDelta = Math.Abs(area.Y - y);

            if (xDelta <= 1 && yDelta <= 1)
            {
                //no diagonals
                if (xDelta > 0 && yDelta > 0)
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }

    private void BuildAreas()
    {
        foreach (var area in _areas.Keys)
        {
            area.Build();
        }
    }
}
