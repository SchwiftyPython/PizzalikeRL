using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWeb : Ability
{
    public SpinWeb(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilityTileSelectedEventName, this);

        EventMediator.Instance.Broadcast(GlobalHelper.SingleTileAbilityEventName, this, Range);

        base.Use();
    }

    public override void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.AbilityTileSelectedEventName)
        {
            if (!(parameter is Tile target))
            {
                return;
            }

            
            //todo apply web sprite to tile -- need a dang web sprite
            //todo set a trap property on tile?

            RemainingCooldownTurns = Cooldown;

            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityTileSelectedEventName, this);
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}
