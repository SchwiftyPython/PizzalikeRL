﻿using System.Collections.Generic;
using UnityEngine;

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
    private List<Entity> _namedNpcs;
    private List<Area> _areas;
    private List<Building> _buildings;

    public string Name;
    public readonly SettlementSize Size;
    public List<Lot> Lots;
    public Faction Faction;

    public Settlement(Faction faction, SettlementSize size, Cell cell, int population)
    {
        Faction = faction;
        Size = size;
        Name = SettlementPrefabStore.GenerateName();
        _cell = cell;
        _population = population;

        PickAreas();
        BuildAreas();
    }

    private void PickAreas()
    {
        _areas = new List<Area>();
        for (var i = 0; i < _numAreasForSettlementSize[Size]; i++)
        {
            var settlementPlaced = false;
            while (!settlementPlaced)
            {
                var x = Random.Range(0, _cell.GetCellWidth());
                var y = Random.Range(0, _cell.GetCellHeight());

                var area = _cell.Areas[x, y];

                if (area.Settlement != null)
                {
                    continue;
                }

                area.Settlement = this;

                if (area.PresentFactions == null)
                {
                    area.PresentFactions = new List<Faction>();
                }

                area.PresentFactions.Add(Faction);
                _areas.Add(area);
                settlementPlaced = true;
            }
        }
    }

    private void BuildAreas()
    {
        foreach (var area in _areas)
        {
            area.BuildArea();
        }
    }
}
