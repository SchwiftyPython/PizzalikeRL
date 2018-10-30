using System.Collections.Generic;

public class CreatureDeck : Deck<Entity>
{
    private List<EntityTemplate> _availableCreatures;

    public int CardIndex;

    public override List<Entity> Cards { get; set; }

    public CreatureDeck(BiomeType biomeType)
    {
        //todo get available creatures for biome
        Size = 10;
        CardIndex = 0;
        Build();
        Shuffle();
    }

    public override void Build()
    {
        //todo loop through available creatures until deck full
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
