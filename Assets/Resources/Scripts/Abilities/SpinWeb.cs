﻿public class SpinWeb : Ability
{
    public SpinWeb(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilityTileSelectedEventName, this);

        EventMediator.Instance.Broadcast(GlobalHelper.DirectionalAbilityEventName, this, Range);

        base.Use();
    }

    public override void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.AbilityTileSelectedEventName)
        {
            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityTileSelectedEventName, this);

            if (!(parameter is DirectionStruct directionStruct))
            {
                return;
            }

            var tile = directionStruct.target.CurrentTile;

            if (tile.PresentProp != null)
            {
                return;
            }

            tile.PresentProp = new Web();

            directionStruct.target.ApplyEffect(new Immobilize(5));

            AbilityManager.InstantiateAbilityPrefab(tile, tile.PresentProp.Prefab);

            RemainingCooldownTurns = Cooldown;
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}