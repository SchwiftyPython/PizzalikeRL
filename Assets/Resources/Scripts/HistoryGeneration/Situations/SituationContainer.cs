using System.Collections.Generic;
using UnityEditor;

public class SituationContainer
{
    public GUID SituationId;

    public List<string> NextSituations;

    public List<Faction> Factions;

    public List<Entity> NamedCharacters;

    public int TurnsTilNextSituation;
}
