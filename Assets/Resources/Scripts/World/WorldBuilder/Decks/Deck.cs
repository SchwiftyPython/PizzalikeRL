using System.Collections.Generic;
using UnityEngine;

public abstract class Deck<T>
{
    public abstract List<T> Cards { get; set; }

    public abstract void Build();

    public abstract T Draw();

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
