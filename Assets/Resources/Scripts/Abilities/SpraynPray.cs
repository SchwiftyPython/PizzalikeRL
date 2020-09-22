public class SpraynPray : Ability
{
    private const int ShotCount = 15;
    private const int AccuracyModifierPerShot = -1;
    private const float DamageModifier = .4f;

    public SpraynPray(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilityTileSelectedEventName, this);

        var range = Owner.GetEquippedMissileWeapon().Range;

        EventMediator.Instance.Broadcast(GlobalHelper.DirectionalAbilityEventName, this, range);

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

            var accuracyModifier = 0;
            for (var i = 0; i < ShotCount; i++)
            {
                Owner.RangedMissileAttack(target, accuracyModifier, DamageModifier);

                accuracyModifier += AccuracyModifierPerShot;
            }

            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityTileSelectedEventName, this);

            UseAbilitySuccess();
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}
