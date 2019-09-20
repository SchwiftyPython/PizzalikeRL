using UnityEngine;

public class Push : Effect
{
    public Push(Entity target, GoalDirection direction, int distance = 1)
    {
        name = "push";
        entity = target;

        var directionVector = GlobalHelper.GetVectorForDirection(direction);

        for (var i = 0; i < distance; i++)
        {
            var targetTileCoordinates = new Vector2(target.CurrentTile.X + directionVector.x,
                target.CurrentTile.Y + directionVector.y);

            if (!target.AreaMapCanMoveLocal(targetTileCoordinates))
            {
                break;
            }

            target.AreaMove(targetTileCoordinates);
        }

    }
}
