using System.Collections.Generic;

public sealed class FactionDeck : Deck<Faction>
{
    public int CardIndex;

    public override List<Faction> Cards { get; set; }

    public FactionDeck()
    {
        CardIndex = 0;
        Build();
        Shuffle();
    }

    public override void Build()
    {
        Cards = new List<Faction>();
        foreach (var faction in WorldData.Instance.Factions.Values)
        {
            var numFactionCards = faction.Population / 100;

            for (var i = 0; i < numFactionCards; i++)
            {
                Cards.Add(faction);
            }
        }
        Size = Cards.Count;
    }

    public override Faction Draw()
    {
        var card = Cards[CardIndex];
        if (CardIndex >= Size)
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
