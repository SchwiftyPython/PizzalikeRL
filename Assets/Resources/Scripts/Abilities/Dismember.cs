using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dismember : Ability
{
    public Dismember(AbilityTemplate template, Entity owner) : base(template, owner)
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

            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityTileSelectedEventName, this);

            var target = directionStruct.target;

            RemainingCooldownTurns = Cooldown;

            var possibleParts = new List<BodyPart>();
            foreach (var part in target.Body.Values)
            {
                if (part.Type.Equals("head", StringComparison.OrdinalIgnoreCase) ||
                    part.Type.Equals("torso", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                possibleParts.Add(part);
            }

            if (possibleParts.Count <= 0)
            {
                EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, $@"{target.EntityType} has no limbs!");
                return;
            }

            var dismemberedPart = Owner.ApplyMeleeDamageReturnBodyPartHit(target, true);

            var winner =
                GlobalHelper.AttackerDefenderAttributeCheck(Owner, target, GlobalHelper.AttributeCheck.Strength);

            if (winner == GlobalHelper.AttributeCheckWinner.Attacker)
            {
                target.RemoveBodyPart(dismemberedPart);

                EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                    $@"{target.EntityType}'s {dismemberedPart.Name} was chopped off!");

                var dice = GlobalHelper.GetDiceFromString(Dice);

                var bleedDuration = DiceRoller.Instance.RollDice(dice);

                target.ApplyEffect("bleed", bleedDuration, 0);
            }
            else
            {
                EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                    $@"{target.EntityType} resisted dismember!");
            }

            UseAbilitySuccess();
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}
