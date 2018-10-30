using System.Collections.Generic;
using UnityEngine;

public sealed class CreatureDeck : Deck<Entity>
{
    private readonly List<EntityTemplate> _availableCreatures;

    public int CardIndex;

    public override List<Entity> Cards { get; set; }

    public CreatureDeck(BiomeType biomeType)
    {
        _availableCreatures = EntityTemplateLoader.GetWildTemplatesForBiome(biomeType);
        Size = 10;
        CardIndex = 0;
        Build();
        Shuffle();
    }

    public override void Build()
    {
        Cards = new List<Entity>();

        var numCards = 0;
        var index = 0;
        while (numCards < Size)
        {
            var template = _availableCreatures[index];

            var roll = Random.Range(1, 101);

            if (roll < 10)
            {
                Cards.Add(new Entity(template));
                numCards++;
            }

            if (index >= _availableCreatures.Count - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }
    }

    public override Entity Draw()
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
