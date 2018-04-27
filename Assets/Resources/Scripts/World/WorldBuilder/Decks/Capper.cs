using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Capper
{
    private const int NumCardsToRecord = 10;

    //Rarity caps per _numCardsToRecord
    private const int CommonCap = 5;
    private const int UncommonCap = 4;
    private const int RareCap = 1;

    private readonly Dictionary<Rarities, int> _rarityCaps;

    public Queue<Rarities> RecordedCards;

    public Capper()
    {
        RecordedCards = new Queue<Rarities>();
        _rarityCaps = new Dictionary<Rarities, int>
        {
            { Rarities.Common, CommonCap },
            { Rarities.Uncommon, UncommonCap },
            { Rarities.Rare, RareCap }
        };
    }

    public bool IsCapped(Rarities rarity)
    {
        var rarityCount = RecordedCards.Count(card => card == rarity);

        return rarityCount >= _rarityCaps[rarity];
    }

    public void RecordCardRarity(Rarities rarity)
    {
        if (RecordedCards.Count >= NumCardsToRecord)
        {
            RecordedCards.Dequeue();
        }
        RecordedCards.Enqueue(rarity);
    }
}
