using UnityEngine;

public class Lot
{
    public int Height;
    public int Width;

    public Vector2 UpperLeftCorner;

    public Lot(Vector2 upperLeftCorner, int height, int width)
    {
        UpperLeftCorner = upperLeftCorner;
        Height = height;
        Width = width;
    }

}
