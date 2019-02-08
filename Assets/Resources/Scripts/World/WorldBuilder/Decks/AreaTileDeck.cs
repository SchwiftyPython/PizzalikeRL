using System.Collections.Generic;
using UnityEngine;

public sealed class AreaTileDeck : Deck<GameObject>
{
    private readonly Dictionary<GameObject, Rarities> _cardRarityReference;

    private readonly Capper _capper;

    public int CardIndex;

    public override List<GameObject> Cards { get; set; }

    public AreaTileDeck(BiomeType biomeType)
    {
        _cardRarityReference = WorldData.Instance.GetBiomeTilesForAreaTileDeck(biomeType);
        CardIndex = 0;
        Build();
        Shuffle();
        _capper = new Capper(45, 42, 2, 1);
    }

    public override void Build()
    {
        Cards = new List<GameObject>(_cardRarityReference.Keys);
        Size = Cards.Count;
    }

    public override GameObject Draw()
    {
        var maxTries = Size;
        var validCard = false;
        var numTries = 0;

        GameObject card = null;
        var cardRarity = Rarities.Common;
        while (!validCard && numTries < maxTries)
        {
            numTries++;

            card = Cards[CardIndex];

            cardRarity = _cardRarityReference[card];

            if (!_capper.IsCapped(cardRarity))
            {
                validCard = true;
            }

            if (CardIndex >= Size - 1)
            {
                Shuffle();
                CardIndex = 0;
            }
            else
            {
                CardIndex++;
            }
        }
        _capper.RecordCardRarity(cardRarity);
        return card;
    }

}
