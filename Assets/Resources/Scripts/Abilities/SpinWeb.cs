using UnityEngine;

public class SpinWeb : Ability
{
    public SpinWeb(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilityTileSelectedEventName, this);

        EventMediator.Instance.Broadcast(GlobalHelper.SingleTileAbilityEventName, Owner, Range);

        base.Use();
    }

    public override void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.AbilityTileSelectedEventName)
        {
            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityTileSelectedEventName, this);

            if (!(parameter is Tile target))
            {
                return;
            }

            if (target.PresentProp != null)
            {
                return;
            }

            target.PresentProp = new Web();

            AbilityManager.InstantiateAbilityPrefab(target, target.PresentProp.Prefab);

            RemainingCooldownTurns = Cooldown;
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}
