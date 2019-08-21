using System;
using System.Collections.Generic;
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
    private List<Entity> _citizens;
    private IDictionary<Area, SettlementSection> _areas;

    public string Name;
    public readonly SettlementSize Size;
    public Faction Faction;

    public Settlement(Faction faction, SettlementSize size, Cell cell, int population, bool isStartingArea = false)
    {
        Faction = faction;
        Size = size;
        Name = SettlementPrefabStore.GenerateName();
        _cell = cell;
        _population = population;

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
        var sdo = new SettlementSdo
        {
            CellId = _cell.Id,
            Population = _population,
            FactionName = Faction.Name,
            History = _history,
            Name = Name, 
            CitizenIds = new List<Guid>(),
            Size = Size,
            AreaIds = new List<string>()
        };

        foreach (var area in _areas.Keys)
        {
            sdo.AreaIds.Add(area.Id);
        }

        foreach (var lotSdo in sdo.LotSdos)
        {
            sdo.BuildingSdos.Add(lotSdo.AssignedBuildingSdo);
        }

        if (_citizens != null)
        {
            foreach (var citizen in _citizens)
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
