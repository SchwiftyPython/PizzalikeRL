using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LotSdo
{
    public int Height;
    public int Width;

    public SerializableVector3 UpperLeftCorner;
    public SerializableVector3 UpperRightCorner;
    public SerializableVector3 LowerRightCorner;
    public SerializableVector3 LowerLeftCorner;

    public BuildingSdo AssignedBuildingSdo;

    public static List<LotSdo> ConvertToLotSdos(List<Lot> lots)
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

    public static List<Lot> ConvertToLots(List<LotSdo> sdos)
    {
        return sdos.Select(ConvertToLot).ToList();
    }

    public static Lot ConvertToLot(LotSdo sdo)
    {
        return sdo == null ? null : new Lot(sdo);
    }
}
