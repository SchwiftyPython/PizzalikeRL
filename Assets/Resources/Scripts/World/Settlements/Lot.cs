using UnityEngine;

public class Lot
{
    public int Height;
    public int Width;

    public Vector2 UpperLeftCorner;
    public Vector2 UpperRightCorner;
    public Vector2 LowerRightCorner;
    public Vector2 LowerLeftCorner;

    public Building AssignedBuilding;

    public Lot(Vector2 upperLeftCorner, int height, int width)
    {
        UpperLeftCorner = upperLeftCorner;
        Height = height;
        Width = width;

        UpperRightCorner = new Vector2(UpperLeftCorner.x + Width, UpperLeftCorner.y);
        LowerRightCorner = new Vector2(UpperRightCorner.x, UpperRightCorner.y + Height);
        LowerLeftCorner = new Vector2(UpperLeftCorner.x, UpperLeftCorner.y + Height);
    }

    public bool IsPartOfLot(Vector2 point)
    {
        if (point.x > LowerRightCorner.x)
        {
            return false;
        }
        if (point.x < UpperLeftCorner.x)
        {
            return false;
        }
        if (point.y < UpperLeftCorner.y)
        {
            return false;
        }
        if (point.y > LowerRightCorner.y)
        {
            return false;
        }
        return true;
    }

}
