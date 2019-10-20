using UnityEngine;

public class SpinWeb : Ability
{
    private readonly GameObject _prefab = WorldData.Instance.SpiderWebPrefab;
    private const int ImmobilizeDuration = 5;

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

            directionStruct.target.ApplyEffect(new Immobilize(ImmobilizeDuration));

            AbilityManager.InstantiateAbilityPrefab(tile, _prefab);

            RemainingCooldownTurns = Cooldown;

            UseAbilitySuccess();
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}