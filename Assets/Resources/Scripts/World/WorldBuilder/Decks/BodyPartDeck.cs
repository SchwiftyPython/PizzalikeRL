using System.Collections.Generic;

public sealed class BodyPartDeck : Deck<BodyPart>
{
    private readonly List<BodyPart> _body;

    public int CardIndex;

    public override List<BodyPart> Cards { get; set; }

    public BodyPartDeck(List<BodyPart> body)
    {
        _body = body;
        CardIndex = 0;
        Build();
        Shuffle();
    }

    public override void Build()
    {
        Cards = new List<BodyPart>();
        foreach (var bodyPart in _body)
        {
            Cards.Add(bodyPart);
        }
        Size = Cards.Count;
    }

    public override BodyPart Draw()
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
