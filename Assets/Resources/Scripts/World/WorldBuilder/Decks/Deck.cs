using System.Collections.Generic;
using UnityEngine;

public abstract class Deck
{
    public List<string> Cards { get; set; }

    public abstract void Build();

    public abstract string Draw();

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
}
