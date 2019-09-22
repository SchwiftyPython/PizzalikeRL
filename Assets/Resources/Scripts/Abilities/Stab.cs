public class Stab : Ability
{
    public Stab(AbilityTemplate template, Entity owner) : base(template, owner)
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

            target.ApplyEffect(Effect, 2, 2);

            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityTileSelectedEventName, this);
        }

        base.OnNotify(eventName, broadcaster, parameter);
    }
}
