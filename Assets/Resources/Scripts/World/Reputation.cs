using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ReputationState
{
    Loved,
    Liked,
    Disliked,
    Hated
}

[Serializable]
public class Reputation
{
    private const int MaxRelationshipLevel = 1000;
    private const int MinRelationshipLevel = MaxRelationshipLevel * -1;

    private readonly ReputationStateDictionary _reputationStateThresholds = new ReputationStateDictionary
    {
        {ReputationState.Loved, 500},
        {ReputationState.Liked, 250},
        {ReputationState.Disliked, -250},
        {ReputationState.Hated, -500}
    };

    [Serializable]
    public class ReputationStateDictionary : SerializableDictionary<ReputationState, int> { }

    [Serializable]
    public class ReputationDictionary : SerializableDictionary<EntityGroup, int> { }

    public ReputationDictionary Relationships;

    public void ChangeReputationValue(EntityGroup otherGroup, int reputationChange)
    {
        Relationships[otherGroup] += reputationChange;

        if (Relationships[otherGroup] > MaxRelationshipLevel)
        {
            Relationships[otherGroup] = MaxRelationshipLevel;
            return;
        }
        if (Relationships[otherGroup] < MinRelationshipLevel)
        {
            Relationships[otherGroup] = MinRelationshipLevel;
        }
    }
}
