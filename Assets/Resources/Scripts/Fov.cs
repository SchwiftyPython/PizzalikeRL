using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fov : MonoBehaviour
{
    private const int ViewDistance = 100; 
    private const int ShadowDistance = 5; 

    private Tile[,] _grid;
    private int _gridHeight;
    private int _gridWidth;

    private Entity _player;

    public void Init(Area area)
    {
        _grid = area.AreaTiles;
        _gridHeight = area.Height;
        _gridWidth = area.Width;

        _player = GameManager.Instance.Player;
    }

    public void Refresh(Vinteger pos)
    {
        for (var octant = 0; octant < 8; octant++)
        {
            RefreshOctant(pos, octant);
        }
    }

    public void RefreshOctant(Vinteger start, int octant, int maxRows = 999)
    {
        var line = new ShadowLine();

        var fullShadow = false;

        for (var row = 1; row < maxRows; row++)
        {
            var bounds = start.Add(TransformOctant(row, 0, octant));
            if (!InBoundsAndClose(bounds.X, bounds.Y))
            {
                break;
            }

            for (var col = 0; col <= row; col++)
            {
                var pos = start.Add(TransformOctant(row, col, octant));

                if (!InBoundsAndClose(pos.X, pos.Y))
                {
                    break;
                }

                if (fullShadow)
                {
                    var tile = _grid[pos.X, pos.Y];
                    tile.Visibility = Tile.Visibilities.Invisible;
                }
                else
                {
                    var projection = ProjectTile(row, col);

                    var visible = !line.IsInShadow(projection);

                    var tile = _grid[pos.X, pos.Y];

                    var blocksLight = tile.GetBlocksLight();

                    if (!visible)
                    {
                        tile.Visibility = Tile.Visibilities.Invisible;
                    }
                    else
                    {
                        tile.Visibility = Tile.Visibilities.Visible;
                        tile.Revealed = true;
                    }

                    if (visible && blocksLight)
                    {
                        line.Add(projection);
                        fullShadow = line.IsFullShadow();
                    }
                }

                /*var posDist = pos.Distance(start);
                if (posDist > ShadowDistance)
                {
                    var tile = grid[pos.x, pos.y];
                    if (tile != null)
                    {
                        if (posDist < ShadowDistance + 1)
                        {
                            if (tile.Visibility == (Tile.Visibilities) 1 && tile.Revealed)
                            {
                                tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.gray;
                            }
                            else
                            {
                                tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.black;
                            }
                        }
                        else
                        {
                            tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.black;
                        }
                    }
                }*/
            }
        }
        //return line.shadows;
    }

    public bool InBoundsAndClose(int x, int y)
    {
        var inBounds = !(x < 0 || y < 0 || x > _gridWidth - 1 || y > _gridHeight - 1);

        if (inBounds)
        {
            if (Mathf.Abs(x - _player.CurrentPosition.x) > ViewDistance ||
                Mathf.Abs(y - _player.CurrentPosition.y) > ViewDistance)
            {
                inBounds = false;
            }
        }

        return inBounds;
    }

    private static Vinteger TransformOctant(int row, int col, int octant)
    {
        switch (octant)
        {
            case 0:
                return new Vinteger(col, -row);
            case 1:
                return new Vinteger(row, -col);
            case 2:
                return new Vinteger(row, col);
            case 3:
                return new Vinteger(col, row);
            case 4:
                return new Vinteger(-col, row);
            case 5:
                return new Vinteger(-row, col);
            case 6:
                return new Vinteger(-row, -col);
            case 7:
                return new Vinteger(-col, -row);
            default:
                return new Vinteger(0, 0);
        }
    }

    private static Shadow ProjectTile(int row, int col)
    {
        var topLeft = (float) col / (row + 2);
        var bottomRight = (float) (col + 1) / (row + 1);

        return new Shadow(topLeft, bottomRight, new Vinteger(col, row + 2), new Vinteger(col + 1, row + 1));
    }
}

public class ShadowLine
{
    public List<Shadow> Shadows = new List<Shadow>();

    public bool IsInShadow(Shadow projection)
    {
        return Shadows.Any(sh => sh.Contains(projection));
    }

    public void Add(Shadow sh)
    {
        var index = 0;
        for (; index < Shadows.Count; index++)
        {
            if (Shadows[index].Start >= sh.Start)
            {
                break;
            }
        }

        Shadow overlappingPrevious = null;
        if (index > 0 && Shadows[index - 1].End > sh.Start)
        {
            overlappingPrevious = Shadows[index - 1];
        }

        Shadow overlappingNext = null;
        if (index < Shadows.Count && Shadows[index].Start < sh.End)
        {
            overlappingNext = Shadows[index];
        }

        if (overlappingNext != null)
        {
            if (overlappingPrevious != null)
            {
                overlappingPrevious.End = overlappingNext.End;
                overlappingPrevious.EndPos = overlappingNext.EndPos;
                Shadows.RemoveAt(index);
            }
            else
            {
                overlappingNext.Start = sh.Start;
                overlappingNext.StartPos = sh.StartPos;
            }
        }
        else
        {
            if (overlappingPrevious != null)
            {
                overlappingPrevious.End = sh.End;
                overlappingPrevious.EndPos = sh.EndPos;
            }
            else
            {
                Shadows.Insert(index, sh);
            }
        }
    }

    public bool IsFullShadow()
    {
        return Shadows.Count == 1 && Shadows[0].Start == 0f && Shadows[0].End == 1f;
    }
}

public class Shadow
{
    public float Start;
    public float End;
    public Vinteger StartPos;
    public Vinteger EndPos;

    public Shadow(float shadowStart, float shadowEnd, Vinteger startPosition, Vinteger endPosition)
    {
        Start = shadowStart;
        End = shadowEnd;
        StartPos = startPosition;
        EndPos = endPosition;
    }

    public bool Contains(Shadow other)
    {
        return Start <= other.Start && End >= other.End;
    }
}

public class Vinteger
{
    public int X;
    public int Y;

    public Vinteger(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public Vinteger Add(Vinteger other)
    {
        return new Vinteger(X + other.X, Y + other.Y);
    }

    public float Distance(Vinteger other)
    {
        return Mathf.Sqrt(Mathf.Pow(this.X - other.X, 2) + Mathf.Pow(this.Y - other.Y, 2));
    }
}
