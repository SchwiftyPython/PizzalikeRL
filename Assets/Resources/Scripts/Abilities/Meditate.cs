using System;

public class Meditate : Ability
{
    public Meditate(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        RemainingCooldownTurns = Cooldown;

        Owner.HealRate = 2 * Owner.HealRate;

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EntityMovedOrTookActionEventName, this);
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EntityTookDamageEventName, this);

        UseAbilitySuccess();
        base.Use();
    }

    public override void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.EntityMovedOrTookActionEventName, StringComparison.OrdinalIgnoreCase))
        {
            if (!(parameter is Entity entity))
            {
                return;
            }

            if (entity.Id != Owner.Id)
            {
                return;
            }

            Owner.HealRate = Entity.DefaultHealRate;

            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EntityMovedOrTookActionEventName, this);
        }

        if (eventName.Equals(GlobalHelper.EntityTookDamageEventName, StringComparison.OrdinalIgnoreCase))
        {
            if (!(broadcaster is Entity entity))
            {
                return;
            }

            if (entity.Id != Owner.Id)
            {
                return;
            }

            Owner.HealRate = Entity.DefaultHealRate;

            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EntityMovedOrTookActionEventName, this);
        }
    }
}
