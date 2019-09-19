﻿public class Heal : Ability
{
    public Heal(AbilityTemplate template, Entity owner) : base(template, owner)
    {
    }

    public override void Use()
    {
        RemainingCooldownTurns = Cooldown;

        var dice = GlobalHelper.GetDiceFromString(Dice);

        var amountToHeal = DiceRoller.Instance.RollDice(dice);

        Owner.ApplyEffect("heal", Duration, amountToHeal);

        base.Use();
    }
}
