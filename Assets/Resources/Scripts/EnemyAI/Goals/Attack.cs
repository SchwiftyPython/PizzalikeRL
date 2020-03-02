using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Attack : Goal
{
    private enum AttackMove
    {
        RangedWeapon,
        MeleeWeapon,
        MoveToTarget
    }

    private Dictionary<AttackMove, Func<bool>> _allAttackMoves;

    private Entity _target;

    public Attack()
    {
        InitializeAllAttackMoves();
    }

    public Attack(Entity target)
    {
        _target = target;
        InitializeAllAttackMoves();
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
        base.Push(parentController);
    }

    public override void TakeAction()
    {
        if (_target == null)
        {
            _target = FindSomethingToAttack();

            if (_target == null)
            {
                //Debug.Log(Self.Fluff != null ? Self.Fluff.Name : Self.EntityType + " lost target!");
                FailToParent();
                return;
            }
        }
        if (_target.IsDead())
        {
            //Debug.Log("Target is dead!");
            _target = null;
            FailToParent();
            return;
        }

        var remainingMoves = new Dictionary<AttackMove, Func<bool>>(_allAttackMoves);

        for (var i = 0; i < _allAttackMoves.Count; i++)
        {
            var index = Random.Range(0, remainingMoves.Count);

            var selectedMove = remainingMoves.ElementAt(index);

            if (selectedMove.Value.Invoke())
            {
                return;
            }

            remainingMoves.Remove(selectedMove.Key);
        }
        
        //Debug.Log(Self.Fluff != null ? Self.Fluff.Name : Self.EntityType + " can't attack!");
        FailToParent();
    }

    private bool TryRangedWeapon()
    {
        var attackType = GlobalHelper.GetRandomEnumValue<GlobalHelper.RangedAttackType>();

        Weapon equippedWeapon = null;

        //todo apply los via raycast. Probably in Entity. Make RangedWeaponCanHit method. Include range check
        if (attackType == GlobalHelper.RangedAttackType.Missile && Self.HasMissileWeaponsEquipped() &&
            Self.EquippedMissileWeaponsInRangeOfTarget(_target))
        {
            equippedWeapon = Self.GetEquippedMissileWeapon();
        }


        if (attackType == GlobalHelper.RangedAttackType.Thrown && Self.HasThrownWeaponEquipped() &&
            Self.ThrownWeaponInRangeOfTarget(_target))
        {
            equippedWeapon = Self.GetEquippedThrownWeapon();
        }

        if (equippedWeapon == null)
        {
            return false;
        }

        if(equippedWeapon.IsAoeWeapon())
        {
            List<Tile>tiles = null;

            if (equippedWeapon.AOE.GetType() == typeof(BlastAOE))
            {
                tiles =
                    new List<Tile>(AreaMap.Instance.GetCellsInCircle(_target.CurrentTile.X, _target.CurrentTile.Y,
                        ((BlastAOE)equippedWeapon.AOE).GetRadius()));
            }

            Self.RangedAttackAOE( tiles, equippedWeapon);

            return true;
        }

        Self.RangedAttack(_target, equippedWeapon);
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
        if (GameManager.Instance.IsWorldMapSceneActive() || Self.CurrentArea != _target.CurrentArea || !Self.Mobile ||
            Self.CalculateDistanceToTarget(_target) <= 1)
        {
            return false;
        }

        var targetLocation = _target.CurrentTile;

        PushChildGoal(new MoveToLocal(_target.CurrentArea, targetLocation.X, targetLocation.Y));
        ParentController.Goals.Peek().TakeAction();
        ParentController.Goals.Peek().TakeAction();
        return true;
    }

    private Entity FindSomethingToAttack()
    {
        const int minSearchRadius = 4;
        const int maxSearchRadius = 16;

        var searchRadius = Random.Range(minSearchRadius, maxSearchRadius);

        var area = Self.CurrentArea;

        return (from entity in area.PresentEntities
            let distance = Self.CalculateDistanceToTarget(entity)
            where distance <= searchRadius
            let attitude = Self.GetAttitudeTowards(entity)
            where attitude == Attitude.Hostile
            select entity).FirstOrDefault();
    }

    private void InitializeAllAttackMoves()
    {
        _allAttackMoves = new Dictionary<AttackMove, Func<bool>>
        {
            {AttackMove.RangedWeapon, TryRangedWeapon},
            {AttackMove.MeleeWeapon, TryMeleeWeapon},
            {AttackMove.MoveToTarget, TryMovingToTarget}
        };
    }
}
