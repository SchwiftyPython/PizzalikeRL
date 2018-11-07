using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wander : Goal
{
    private const int MaxWanderDistance = 25;

    public override void Create()
    {
        Pop();

        var area = ParentController.Self.CurrentArea;
        var areaWidth = area.Width;
        var areaHeight = area.Height;

        var x = Random.Range(0, areaWidth);
        var y = Random.Range(0, areaHeight);

        const int maxTries = 40;
        var numTries = 0;
        while (!ParentController.Self.AreaMapCanMove(new Vector2(x, y)) ||
               DistanceToTarget(new Vector2(x, y)) > MaxWanderDistance)
        {
            numTries++;
            if (numTries > maxTries)
            {
                FailToParent();
                return;
            }
            x = Random.Range(0, areaWidth);
            y = Random.Range(0, areaHeight);
        }
        PushChildGoal(new MoveToLocal(area, x, y), ParentGoal);
    }

    private int DistanceToTarget(Vector2 target)
    {
        var a = target.x - ParentController.Self.CurrentPosition.x;
        var b = target.y - ParentController.Self.CurrentPosition.y;

        return (int)Math.Sqrt(a * a + b * b);
    }
}
