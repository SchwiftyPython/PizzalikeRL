using System.Collections.Generic;

public enum Direction
{
    Left,
    Right,
    Top,
    Bottom
}

public class River
{
    public int Length;
    public List<Cell> Cells;
    public int Id;

    public int Intersections;
    public float TurnCount;
    public Direction CurrentDirection;

    public River(int id)
    {
        Id = id;
        Cells = new List<Cell>();
    }

    public void AddCell(Cell cell)
    {
        cell.SetRiverPath(this);
        Cells.Add(cell);
    }
}

public class RiverGroup
{
    public List<River> Rivers = new List<River>();
}
