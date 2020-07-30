public class BandageWounds : Ability
{
    public BandageWounds(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        Owner.UseConsumableWithProperty(RequiresProperty);

        RemainingCooldownTurns = Cooldown;

        Owner.RemoveEffect(typeof(Bleed));

        if (Owner.IsPlayer())
        {
            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                "You are no longer bleeding.");
        }
        else
        {
            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                $"{Owner.Name} is no longer bleeding.");
        }

        UseAbilitySuccess();

        base.Use();
    }
}
