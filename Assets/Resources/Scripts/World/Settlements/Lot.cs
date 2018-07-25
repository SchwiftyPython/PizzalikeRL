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

        UpperRightCorner = new Vector2(UpperLeftCorner.x, UpperLeftCorner.y + Width);
        LowerRightCorner = new Vector2(UpperRightCorner.x + Height, UpperRightCorner.y);
        LowerLeftCorner = new Vector2(UpperLeftCorner.x + Height, UpperLeftCorner.y);

        Debug.Log($"Lot upper left corner: {UpperLeftCorner}");
        Debug.Log($"Lot upper right corner: {UpperRightCorner}");
        Debug.Log($"Lot lower right corner: {LowerRightCorner}");
        Debug.Log($"Lot lower left corner: {LowerLeftCorner}");
        Debug.Log($"Lot height: {Height}");
        Debug.Log($"Lot width: {Width}");
    }

    public bool IsPartOfLot(Vector2 point)
    {
        if (point.x > LowerLeftCorner.x)
        {
            return false;
        }
        if (point.x < UpperLeftCorner.x)
        {
            return false;
        }
        if (point.y < LowerLeftCorner.y)
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
