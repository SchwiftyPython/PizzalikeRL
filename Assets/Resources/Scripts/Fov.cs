using System.Collections.Generic;
using UnityEngine;

public class Fov : MonoBehaviour
{
    private const int ViewDistance = 52; //average of area width and height. May change to max tiles visible on screen.
    private const int ShadowDistance = 20; //arbitrary

    private Tile[,] grid;
    private int gridHeight;
    private int gridWidth;

    private Entity _player;

    public void Init(Area area)
    {
        grid = area.AreaTiles;
        gridHeight = area.Height;
        gridWidth = area.Width;

        _player = GameManager.Instance.Player;
    }

    public void Refresh(Vinteger pos)
    {
        for (int octant = 0; octant < 8; octant++)
        {
            RefreshOctant(pos, octant);
        }
    }

    public void RefreshOctant(Vinteger start, int octant, int maxRows = 999)
    {
        ShadowLine line = new ShadowLine();

        bool fullShadow = false;

        for (var row = 1; row < maxRows; row++)
        {
            Vinteger bounds = start.Add(TransformOctant(row, 0, octant));
            if (!InBoundsAndClose(bounds.x, bounds.y))
            {
                break;
            }

            for (var col = 0; col <= row; col++)
            {
                Vinteger pos = start.Add(TransformOctant(row, col, octant));

                if (!InBoundsAndClose(pos.x, pos.y))
                {
                    break;
                }

                if (fullShadow)
                {
                    Tile tile = grid[pos.x, pos.y];
                    if(tile != null)
                    {
                        if (tile.Visibility == 0) //todo make enum 0 for black, 1 for white????
                        {
                            tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.black;
                        }
                        else
                        {
                            tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.gray;
                        }
                    }
                }
                else
                {
                    Shadow projection = ProjectTile(row, col);

                    bool visible = !line.IsInShadow(projection);

                    Tile tile = grid[pos.x, pos.y];

                    bool isWall = false;

                    if (tile != null)
                    {
                        if (tile.GetBlocksMovement()) //todo maybe tagged wall? or blocks light?
                        {
                            isWall = true;
                        }

                        Color color = Color.white;

                        if (!visible)
                        {
                            if (tile.Visibility == 0)
                            {
                                tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.black;
                            }
                            else
                            {
                                tile.TextureInstance.GetComponent<SpriteRenderer>().color = Color.gray;
                            }
                        }
                        else
                        {
                            tile.Visibility = 1;
                        }
                       
                        tile.TextureInstance.GetComponent<SpriteRenderer>().color = color;
                    }
                    else
                    {
                        isWall = true;
                    }

                    if (visible && isWall)
                    {
                        line.Add(projection);
                        fullShadow = line.IsFullShadow();
                    }
                }

                float posDist = pos.Distance(start);
                if (posDist > ShadowDistance)
                {
                    Tile tile = grid[pos.x, pos.y];
                    if (tile != null)
                    {
                        if (posDist < ShadowDistance + 20)
                        {
                            if (tile.Visibility == 0)
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
                }
            }
        }
        //return line.shadows;
    }

    public bool InBoundsAndClose(int x, int y)
    {
        bool retVal = !(x < 0 || y < 0 || x > gridWidth - 1 || y > gridHeight - 1);

        if (retVal)
        {
            if (Mathf.Abs(x - _player.CurrentPosition.x) > ViewDistance ||
                Mathf.Abs(y - _player.CurrentPosition.y) > ViewDistance)
            {
                retVal = false;
            }
        }

        return retVal;
    }

    private Vinteger TransformOctant(int row, int col, int octant)
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

    private Shadow ProjectTile(int row, int col)
    {
        float topLeft = (float) col / (row + 2);
        float bottomRight = (float) (col + 1) / (row + 1);

        return new Shadow(topLeft, bottomRight, new Vinteger(col, row + 2), new Vinteger(col + 1, row + 1));
    }
}

public class ShadowLine
{
    public List<Shadow> shadows = new List<Shadow>();

    public bool IsInShadow(Shadow projection)
    {
        foreach (var sh in shadows)
        {
            if (sh.Contains(projection))
            {
                return true;
            }
        }
        return false;
    }

    public void Add(Shadow sh)
    {
        int index = 0;
        for (; index < shadows.Count; index++)
        {
            if (shadows[index].start >= sh.start)
            {
                break;
            }
        }

        Shadow o_previous = null;
        if (index > 0 && shadows[index - 1].end > sh.start)
        {
            o_previous = shadows[index - 1];
        }

        Shadow o_next = null;
        if (index < shadows.Count && shadows[index].start < sh.end)
        {
            o_next = shadows[index];
        }

        if (o_next != null)
        {
            if (o_previous != null)
            {
                o_previous.end = o_next.end;
                o_previous.endPos = o_next.endPos;
                shadows.RemoveAt(index);
            }
            else
            {
                o_next.start = sh.start;
                o_next.startPos = sh.startPos;
            }
        }
        else
        {
            if (o_previous != null)
            {
                o_previous.end = sh.end;
                o_previous.endPos = sh.endPos;
            }
            else
            {
                shadows.Insert(index, sh);
            }
        }
    }

    public bool IsFullShadow()
    {
        return shadows.Count == 1 && shadows[0].start == 0 && shadows[0].end == 1;
    }
}

public class Shadow
{
    public float start;
    public float end;
    public Vinteger startPos;
    public Vinteger endPos;

    public Shadow(float shadowStart, float shadowEnd, Vinteger startPosition, Vinteger endPosition)
    {
        start = shadowStart;
        end = shadowEnd;
        startPos = startPosition;
        endPos = endPosition;
    }

    public bool Contains(Shadow other)
    {
        return start <= other.start && end >= other.end;
    }
}

public class Vinteger
{
    public int x;
    public int y;

    public Vinteger(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vinteger Add(Vinteger other)
    {
        return new Vinteger(x + other.x, y + other.y);
    }

    public float Distance(Vinteger other)
    {
        return Mathf.Sqrt(Mathf.Pow(this.x - other.x, 2) + Mathf.Pow(this.y + other.y, 2));
    }
}
