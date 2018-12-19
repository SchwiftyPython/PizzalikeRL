using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class BuildingSdo
{
    public int WallTypeIndex;

    public int FloorTypeIndex;

    public int Width;

    public int Height;

    public static List<BuildingSdo> ConverToBuildingSdos(List<Building> buildings)
    {
        return buildings?.Select(ConvertToBuildingSdo).ToList();
    }

    public static BuildingSdo ConvertToBuildingSdo(Building building)
    {
        return new BuildingSdo
        {
            WallTypeIndex = building.WallTypeIndex,
            FloorTypeIndex = building.FloorTypeIndex,
            Width = building.Width,
            Height = building.Height
        };
    }
}
