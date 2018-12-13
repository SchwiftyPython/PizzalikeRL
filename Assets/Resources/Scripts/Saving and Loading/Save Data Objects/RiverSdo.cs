using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class RiverSdo
{
    public int Length;
    public int Id; 

    public static List<RiverSdo> ConvertToRiverSdos(List<River> rivers)
    {
        return rivers.Select(river => new RiverSdo
            {
                Length = river.Length,
                Id = river.Id
            })
            .ToList();
    }
}
