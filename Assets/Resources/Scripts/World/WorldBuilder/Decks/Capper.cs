using System.Collections.Generic;
using System.Linq;

public class Capper
{
    private readonly int _numCardsToRecord;

    private readonly Dictionary<Rarities, int> _rarityCaps;

    public Queue<Rarities> RecordedCards;

    public Capper(int numCardsToRecord, int commonCap, int uncommonCap, int rareCap)
    {
        RecordedCards = new Queue<Rarities>();

        _numCardsToRecord = numCardsToRecord;

        _rarityCaps = new Dictionary<Rarities, int>
        {
            { Rarities.Common, commonCap },
            { Rarities.Uncommon, uncommonCap },
            { Rarities.Rare, rareCap }
        };
    }

    public bool IsCapped(Rarities rarity)
    {
        var rarityCount = RecordedCards.Count(card => card == rarity);

        return rarityCount >= _rarityCaps[rarity];
    }

    public void RecordCardRarity(Rarities rarity)
    {
        if (RecordedCards.Count >= _numCardsToRecord)
        {
            RecordedCards.Dequeue();
        }
        RecordedCards.Enqueue(rarity);
    }
}
