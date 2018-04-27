using System.Collections.Generic;

public sealed class FactionDeck : Deck
{
    public int NumCellsToSkipBeforeNextDraw { get; } = 25;

    public int CardIndex;

    public FactionDeck()
    {
        CardIndex = 0;
        Build();
        Shuffle();
    }

    public override void Build()
    {
        Cards = new List<string>();
        foreach (var faction in WorldData.Instance.Factions.Values)
        {
            var numFactionCards = faction.Population / 100;

            for (var i = 0; i < numFactionCards; i++)
            {
                Cards.Add(faction.Type);
            }
        }
    }

    public override string Draw()
    {
        var card = Cards[CardIndex];
        if (CardIndex >= Cards.Count)
        {
            Shuffle();
            CardIndex = 0;
        }
        else
        {
            CardIndex++;
        }
        return card;
    }
}
