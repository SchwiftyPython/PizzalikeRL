using System.Collections.Generic;

public class Situation
{
    private readonly SituationContainer _situationContainer;

    public Situation(SituationContainer sc)
    {
        _situationContainer = new SituationContainer
        {
            SituationId = sc.SituationId,
            NextSituations = sc.NextSituations,
            Factions = sc.Factions,
            NamedCharacters = sc.NamedCharacters,
            TurnsTilNextSituation = sc.TurnsTilNextSituation
        };
    }

    public SituationContainer GetSituationContainer()
    {
        return _situationContainer;
    }

    public List<string> GetNextSituations()
    {
        return _situationContainer.NextSituations;
    }

    public List<Faction> GetFactions()
    {
        return _situationContainer.Factions;
    }

    public List<Entity> GetNamedCharacters()
    {
        return _situationContainer.NamedCharacters;
    }

    public int GetTurnsTilNextSituation()
    {
        return _situationContainer.TurnsTilNextSituation;
    }

    public void DecrementTurnsTilNextSituation()
    {
        _situationContainer.TurnsTilNextSituation--;
    }


}
