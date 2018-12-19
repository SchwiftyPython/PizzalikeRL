using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LotSdo 
{
    public int Height;
    public int Width;

    public Vector2 UpperLeftCorner;
    public Vector2 UpperRightCorner;
    public Vector2 LowerRightCorner;
    public Vector2 LowerLeftCorner;

    public BuildingSdo AssignedBuildingSdo;

    public static List<LotSdo> ConverToLotSdos(List<Lot> lots)
    {
        return lots.Select(ConvertToLotSdo).ToList();
    }

    public static LotSdo ConvertToLotSdo(Lot lot)
    {
        if (lot == null)
        {
            return null;
        }

        return new LotSdo
        {
            Height = lot.Height,
            Width = lot.Width,
            UpperLeftCorner = lot.UpperLeftCorner,
            UpperRightCorner = lot.UpperRightCorner,
            LowerRightCorner = lot.LowerRightCorner,
            LowerLeftCorner = lot.LowerLeftCorner,
            AssignedBuildingSdo = BuildingSdo.ConvertToBuildingSdo(lot.AssignedBuilding)
        };
    }
}
