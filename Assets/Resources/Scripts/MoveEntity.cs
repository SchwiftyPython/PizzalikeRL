using System.Collections.Generic;
using UnityEngine;

public class MoveEntity {
    public enum Direction {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    protected IDictionary<Direction, Vector2> Directions = new Dictionary<Direction, Vector2> {
        {Direction.North, Vector2.up },
        {Direction.NorthEast, Vector2.one },
        {Direction.East, Vector2.right },
        {Direction.SouthEast, new Vector2(1, -1) },
        {Direction.South, Vector2.down },
        {Direction.SouthWest, new Vector2(-1, -1) },
        {Direction.West, Vector2.left },
        {Direction.NorthWest, new Vector2(-1, 1) }
    };
    
	protected Vector2 StartTile;
	protected Vector2 EndTile; 

    public virtual void Move(Vector2 target) {}

    public virtual bool TileOutOfBounds(Vector2 target) {
        return true;
    }

    public virtual bool TargetTileBlocked(Vector2 target) {
        return GameManager.Instance.CurrentAreaPosition.AreaTiles[(int)target.x, (int)target.y].GetBlocksMovement();
    }

    public virtual bool TargetTileBlockedByEntity(Vector2 target) {
        return GameManager.Instance.CurrentAreaPosition.AreaTiles[(int)target.x, (int)target.y].GetPresentEntity() != null;
    }
}
