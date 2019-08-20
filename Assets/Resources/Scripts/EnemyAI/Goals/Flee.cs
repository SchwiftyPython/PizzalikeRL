using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flee : Goal
{
    private const int MinFleeDistance = 12;

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
               DistanceToTarget(new Vector2(x, y)) < MinFleeDistance)
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

        //Debug.Log(Self.Fluff != null ? Self.Fluff.Name : Self.EntityType + " fleeing!");

        PushChildGoal(
            numTries - MinFleeDistance < 0
                ? new MoveToLocal(area, x, y)
                : new MoveToLocal(area, x, y, numTries - MinFleeDistance), ParentGoal);
    }

    private int DistanceToTarget(Vector2 target)
    {
        var a = target.x - Self.CurrentTile.X;
        var b = target.y - Self.CurrentTile.Y;

        return (int)Math.Sqrt(a * a + b * b);
    }
}
