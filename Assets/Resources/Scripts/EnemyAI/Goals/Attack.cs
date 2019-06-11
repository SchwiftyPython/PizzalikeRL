using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Goal
{
    private Entity _target;

    public Attack()
    {
        _target = FindSomethingToAttack();

        if (_target == null)
        {
            FailToParent();
        }
    }

    public Attack(Entity target)
    {
        _target = target;
    }

    public override bool Finished()
    {
        return false;
    }

    public override void Push(EnemyController parentController)
    {
        if (parentController.Self == _target)
        {
            return;
        }
        base.Push(ParentController);
    }

    public override void TakeAction()
    {
        if (_target == null)
        {
            Debug.Log(ParentController.Self + " lost target!");
            FailToParent();
            return;
        }
        if (_target.IsDead())
        {
            Debug.Log("Target is dead!");
            _target = null;
            FailToParent();
            return;
        }
    }

    private Entity FindSomethingToAttack()
    {
        const int minSearchRadius = 4;
        const int maxSearchRadius = 16;

        var searchRadius = Random.Range(minSearchRadius, maxSearchRadius);

        var area = ParentController.Self.CurrentArea;

        Entity target = null;

        foreach (var entity in area.PresentEntities)
        {
            var distance = ParentController.Self.CalculateDistanceToTarget(entity);

            //todo add fov if enemy seems too smart
            if (distance <= searchRadius)
            {
                var attitude = ParentController.Self.GetAttitudeTowardsTarget(entity);

                if (attitude == Attitude.Hostile)
                {
                    target = entity;
                    break;
                }
            }
        }

        return target;
    }
}
