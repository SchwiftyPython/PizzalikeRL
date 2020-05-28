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

    public PropSdo[] PropSdos;

    public static List<BuildingSdo> ConvertToBuildingSdos(List<Building> buildings)
    {
        return buildings?.Select(ConvertToBuildingSdo).ToList();
    }

    public static BuildingSdo ConvertToBuildingSdo(Building building)
    {
        if (building == null)
        {
            return null;
        }

        var sdo = new BuildingSdo();
        sdo.WallTypeIndex = building.WallTypeIndex;
        sdo.FloorTypeIndex = building.FloorTypeIndex;
        sdo.Width = building.Width;
        sdo.Height = building.Height;
        sdo.Blueprint = ConvertBlueprintForSaving(building.Blueprint);
        sdo.PropSdos = ConvertPropsForSaving(building.Props);
        return sdo;
    }

    public static List<Building> ConvertToBuildings(List<BuildingSdo> sdos)
    {
        return sdos.Select(ConvertToBuilding).ToList();
    }

    public static Building ConvertToBuilding(BuildingSdo sdo)
    {
        return new Building(sdo);
    }

    public static char[] ConvertBlueprintForSaving(char[,] blueprint) 
    {
        return GlobalHelper.Convert2dArrayTo1dArray(blueprint);
    }

    public static char[,] ConvertBlueprintForLoading(int height, int width, char[] blueprint)
    {
        return GlobalHelper.Convert1dArrayTo2dArray(height, width, blueprint);
    }

    public static PropSdo[] ConvertPropsForSaving(Prop[,] props)
    {
        var height = props.GetLength(0);
        var width = props.GetLength(1);

        var propSdos = new PropSdo[height, width];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                var prop = props[row, column];

                propSdos[row, column] = ConvertPropForSaving(prop);
            }
        }

        return GlobalHelper.Convert2dArrayTo1dArray(propSdos);
    }

    public static Prop[,] ConvertPropsForPlaying(int height, int width, PropSdo[] sdos)
    {
        var props = new Prop[sdos.Length];
        for (var i = 0; i < sdos.Length; i++)
        {
            props[i] = ConvertPropForPlaying(sdos[i]);
        }

        return GlobalHelper.Convert1dArrayTo2dArray(height, width, props);
    }

    private static PropSdo ConvertPropForSaving(Prop prop)
    {
        if (prop == null)
        {
            return null;
        }

        var propType = prop.GetType();

        PropSdo sdo = null;

        if (propType == typeof(CheeseTree))
        {
            sdo = new CheeseTreeSdo();
        }
        else if (propType == typeof(Chest))
        {
            sdo = new ChestSdo((Chest)prop);
        }
        else if (propType == typeof(Field))
        {
            sdo = new FieldSdo((Field)prop);
        }
        else if (propType == typeof(Grave))
        {
            sdo = new GraveSdo((Grave)prop);
        }
        else if (propType == typeof(Furniture))
        {
            sdo = new FurnitureSdo((Furniture)prop);
        }

        return sdo;
    }

    private static Prop ConvertPropForPlaying(PropSdo sdo)
    {
        if (sdo == null)
        {
            return null;
        }

        var sdoType = sdo.GetType();

        if (sdoType == typeof(CheeseTreeSdo))
        {
            return new CheeseTree();
        }

        if (sdoType == typeof(ChestSdo))
        {
            return new Chest((ChestSdo)sdo);
        }

        if (sdoType == typeof(FieldSdo))
        {
            return new Field((FieldSdo)sdo);
        }

        if (sdoType == typeof(GraveSdo))
        {
            return new Grave((GraveSdo)sdo);
        }

        if (sdoType == typeof(FurnitureSdo))
        {
            return new Furniture((FurnitureSdo)sdo);
        }

        return null;
    }
}
