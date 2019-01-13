using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class RiverSdo
{
    public int Length;
    public int Id;

    public List<string> CellIds;

    public static List<RiverSdo> ConvertToRiverSdos(List<River> rivers)
    {
        return rivers.Select(river =>
            {
                var sdo = new RiverSdo
                {
                    Length = river.Length,
                    Id = river.Id,
                    CellIds = new List<string>()
                };

                foreach (var cell in river.Cells)
                {
                    sdo.CellIds.Add(cell.Id);
                }

                return sdo;
            })
            .ToList();
    }

    public static List<River> ConvertToRivers(List<RiverSdo> riverSdos)
    {
        var rivers = new List<River>();

        foreach (var sdo in riverSdos)
        {
            River river;
            if (WorldData.Instance.Rivers.ContainsKey(sdo.Id))
            {
                river = WorldData.Instance.Rivers[sdo.Id];
            }
            else
            {
                river = new River(sdo.Id);
                river.Length = sdo.Length;

                foreach (var id in sdo.CellIds)
                {
                    river.Cells.Add(WorldData.Instance.MapDictionary[id]);
                }

                WorldData.Instance.Rivers.Add(sdo.Id, river);
            }
            rivers.Add(river);
        }

        return rivers;
    }
}
