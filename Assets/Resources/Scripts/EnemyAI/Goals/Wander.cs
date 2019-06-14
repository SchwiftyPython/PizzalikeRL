using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wander : Goal
{
    private const int MaxWanderDistance = 12;

    public override void Create()
    {
        Pop();

        var area = Self.CurrentArea;
        var areaHeight = area.Height;
        var areaWidth = area.Width;

        var x = Random.Range(0, areaHeight);
        var y = Random.Range(0, areaWidth);

        const int maxTries = 40;
        var numTries = 0;
        while (!Self.AreaMapCanMoveLocal(new Vector2(x, y)) ||
               DistanceToTarget(new Vector2(x, y)) > MaxWanderDistance)
        {
            numTries++;
            if (numTries > maxTries)
            {
                FailToParent();
                return;
            }
            x = Random.Range(0, areaHeight);
            y = Random.Range(0, areaWidth);
        }

        PushChildGoal(
            numTries - MaxWanderDistance < 0
                ? new MoveToLocal(area, x, y)
                : new MoveToLocal(area, x, y, numTries - MaxWanderDistance), ParentGoal);
    }

    private int DistanceToTarget(Vector2 target)
    {
        var a = target.x - Self.CurrentTile.X;
        var b = target.y - Self.CurrentTile.Y;

        return (int)Math.Sqrt(a * a + b * b);
    }
}
