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

        UpperRightCorner = new Vector2(UpperLeftCorner.x, UpperLeftCorner.y + Width - 1);
        LowerRightCorner = new Vector2(UpperRightCorner.x + Height - 1, UpperRightCorner.y);
        LowerLeftCorner = new Vector2(UpperLeftCorner.x + Height - 1, UpperLeftCorner.y);
    }

    public bool IsPartOfLot(Vector2 point)
    {
        if (point.x < UpperRightCorner.x)
        {
            return false;
        }
        if (point.x > LowerLeftCorner.x)
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
