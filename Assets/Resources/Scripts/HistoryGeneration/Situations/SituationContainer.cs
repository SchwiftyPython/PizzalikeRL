using System;
using System.Collections.Generic;

public class SituationContainer
{
    public Guid SituationId;

    public List<string> NextSituations;

    public List<Faction> Factions;

    public List<Entity> NamedCharacters;

    public int TurnsTilNextSituation;
}
