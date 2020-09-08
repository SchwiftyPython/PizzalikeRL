using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DisarmingShot : Ability
{
    public DisarmingShot(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilityTileSelectedEventName, this);

        EventMediator.Instance.Broadcast(GlobalHelper.DirectionalAbilityEventName, this);

        base.Use();
    }

    public override void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.AbilityTileSelectedEventName)
        {
            if (!(parameter is DirectionStruct directionStruct))
            {
                return;
            }

            var target = directionStruct.target;

            RemainingCooldownTurns = Cooldown;

            var dice = GlobalHelper.GetDiceFromString(Dice);

            var damage = DiceRoller.Instance.RollDice(dice);

            Owner.ApplyDamage(target, damage);

            var saveDifficulty = DiceRoller.Instance.RollDice(dice) + 12;

            var strCheckDice = new Dice(1, 20);

            var savingThrow = DiceRoller.Instance.RollDice(strCheckDice) + target.Strength;

            if (savingThrow < saveDifficulty)
            {
                var targetWeapons = target.Equipped.Values.Where(w =>
                    w.ItemCategory.Equals("weapon", StringComparison.OrdinalIgnoreCase)).ToList();

                var chosenWeapon = targetWeapons[Random.Range(0, targetWeapons.Count)];

                target.UnEquipItem(chosenWeapon);

                var adjacentTiles = target.CurrentTile.GetAdjacentTiles();

                Tile weaponDropTile = null;
                var tileChosen = false;
                while (!tileChosen)
                {
                    if (adjacentTiles == null || adjacentTiles.Count < 1)
                    {
                        weaponDropTile = target.CurrentTile;
                        tileChosen = true;
                        continue;
                    }

                    var tile = adjacentTiles[Random.Range(0, adjacentTiles.Count)];

                    if (tile.IsWall())
                    {
                        adjacentTiles.Remove(tile);
                        continue;
                    }

                    weaponDropTile = tile;
                    tileChosen = true;
                }

                weaponDropTile.AddItemToTile(chosenWeapon);
                target.Inventory.Remove(chosenWeapon.Id);
            }

            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityTileSelectedEventName, this);

            UseAbilitySuccess();
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}
