using System;

[Serializable]
public class BuildingSdo
{
    public int WallTypeIndex;

    public int FloorTypeIndex;

    public int Width;

    public int Height;

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
