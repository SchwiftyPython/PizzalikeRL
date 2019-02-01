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

    public char[] Blueprint;

    public static List<BuildingSdo> ConvertToBuildingSdos(List<Building> buildings)
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
            Height = building.Height,
            Blueprint = ConvertBlueprintForSaving(building.Blueprint)
        };
    }

    public static Building ConvertToBuilding(BuildingSdo sdo)
    {
        return new Building(sdo);
    }

    public static char[] ConvertBlueprintForSaving(char[,] blueprint)
    {
        var height = blueprint.GetLength(0);
        var width = blueprint.GetLength(1);

        var index = 0;
        var single = new char[height * width];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                single[index] = blueprint[row, column];
                index++;
            }
        }
        return single;
    }

    public static char[,] ConvertBlueprintForLoading(char[] blueprint)
    {
        var index = 0;
        var sqrt = (int)Math.Sqrt(blueprint.Length);
        var multi = new char[sqrt, sqrt];
        for (var y = 0; y < sqrt; y++)
        {
            for (var x = 0; x < sqrt; x++)
            {
                multi[x, y] = blueprint[index];
                index++;
            }
        }
        return multi;
    }
}
