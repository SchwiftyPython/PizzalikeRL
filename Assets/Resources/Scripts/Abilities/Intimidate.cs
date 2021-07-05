public class Intimidate : Ability
{
    public Intimidate(AbilityTemplate template, Entity owner) : base(template, owner)
    {

    }

    public override void Use()
    {
        RemainingCooldownTurns = Cooldown;

        var adjacentTiles = Owner.CurrentTile.GetAdjacentTiles();

        foreach (var tile in adjacentTiles)
        {
            var presentEntity = tile.GetPresentEntity();

            if (presentEntity != null && presentEntity.GetAttitudeTowards(Owner) == Attitude.Hostile)
            {
                var winner = GlobalHelper.AttackerDefenderAttributeCheck(Owner, presentEntity,
                    GlobalHelper.AttributeCheck.Intelligence);

                if (winner == GlobalHelper.AttributeCheckWinner.Attacker)
                {
                    var fearDice = GlobalHelper.GetDiceFromString(Dice);

                    var duration = DiceRoller.Instance.RollDice(fearDice);

                    presentEntity.ApplyEffect("fear", duration, -1);
                }
            }
        }

        UseAbilitySuccess();

        base.Use();
    }
}
