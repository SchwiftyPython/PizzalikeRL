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

    private bool TryRangedWeapon()
    {
        //todo apply los via raycast. Probably in Entity. Make RangedWeaponCanHit method. Include range check
        if (!Self.HasRangedWeaponEquipped() || !Self.EquippedWeaponInRangeOfTarget(_target))
        {
            return false;
        }
        Self.RangedAttack(_target);
        return true;
    }

    private bool TryMeleeWeapon()
    {
        //todo make IsTargetAdjacent method in Entity
        if (Self.CalculateDistanceToTarget(_target) > 1)
        {
            return false;
        }
        Self.MeleeAttack(_target);
        return true;
    }

    private bool TryMovingToTarget()
    {
        //todo can pursue player outside of current area once MoveToGlobal goal is created
        if (GameManager.Instance.IsWorldMapSceneActive() || Self.CurrentArea != _target.CurrentArea)
        {
            return false;
        }
        //todo add goal
        return true;
    }

    private Entity FindSomethingToAttack()
    {
        const int minSearchRadius = 4;
        const int maxSearchRadius = 16;

        var searchRadius = Random.Range(minSearchRadius, maxSearchRadius);

        var area = Self.CurrentArea;

        Entity target = null;

        foreach (var entity in area.PresentEntities)
        {
            var distance = Self.CalculateDistanceToTarget(entity);
            
            if (distance <= searchRadius)
            {
                var attitude = Self.GetAttitudeTowardsTarget(entity);

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
