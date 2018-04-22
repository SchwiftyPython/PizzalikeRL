using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionDeck : IDeck
{
    public List<string> Cards { get; private set; }

    public int NumCellsToSkipBeforeNextDraw { get; } = 25;

    public int cardIndex;

    public FactionDeck()
    {
        cardIndex = 0;
        Build();
        Shuffle();
    }

    public void Build()
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

    public void Shuffle()
    {
        for (var i = Cards.Count - 1; i > 0; i--)
        {
            var n = Random.Range(0, i + 1);
            var temp = Cards[i];
            Cards[i] = Cards[n];
            Cards[n] = temp;
        }
    }

    public string Draw()
    {
        var card = Cards[cardIndex];
        if (cardIndex >= Cards.Count)
        {
            Shuffle();
            cardIndex = 0;
        }
        else
        {
            cardIndex++;
        }
        return card;
    }
}
