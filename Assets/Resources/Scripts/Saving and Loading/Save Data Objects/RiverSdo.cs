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
}
