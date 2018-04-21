using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionDeck : IDeck
{
    private List<string> _deck;

    public static int NumCellsToSkipBeforeNextDraw { get; } = 10;

    public FactionDeck()
    {
        Build();
        Shuffle();
    }

    public void Build()
    {
        _deck = new List<string>();
        foreach (var faction in WorldData.Instance.Factions.Values)
        {
            var numFactionCards = faction.Population / 100;

            for (var i = 0; i < numFactionCards; i++)
            {
                _deck.Add(faction.Type);
            }
        }
    }

    public void Shuffle()
    {
        for (var i = _deck.Count - 1; i > 0; i--)
        {
            var n = Random.Range(0, i + 1);
            var temp = _deck[i];
            _deck[i] = _deck[n];
            _deck[n] = temp;
        }
    }

    public string Draw()
    {
        var card = _deck.First();
        _deck.RemoveAt(0);
        return card;
    }
}
