using System;
using System.Collections.Generic;

public enum ReputationState
{
    Loved,
    Liked,
    Neutral,
    Disliked,
    Hated
}

public enum EntityGroupType
{
    Faction,
    EntityType
}

[Serializable]
public class Reputation
{
    private const int MaxRelationshipLevel = 1000;
    private const int MinRelationshipLevel = MaxRelationshipLevel * -1;

    private readonly Dictionary<ReputationState, int> _reputationStateThresholds = new Dictionary<ReputationState, int>
    {
        {ReputationState.Loved, 500},
        {ReputationState.Liked, 250},
        {ReputationState.Disliked, -250},
        {ReputationState.Hated, -500}
    };

    private EntityGroupType _groupType;

    [Serializable]
    public class ReputationDictionary : SerializableDictionary<string, int> { }

    public ReputationDictionary Relationships;

    public Reputation(EntityGroupType groupType)
    {
        _groupType = groupType;
        //todo choose alignment
        //loop through all entitygroups of matching type
        //add to worlddata
    }

    public void ChangeReputationValue(string otherGroup, int reputationChange)
    {
        if (!Relationships.ContainsKey(otherGroup))
        {
            return;
        }

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

    public ReputationState GetReputationState(string otherGroup)
    {
        var relationshipValue = Relationships[otherGroup];

        if (relationshipValue >= _reputationStateThresholds[ReputationState.Loved])
        {
            return ReputationState.Loved;
        }
        if (relationshipValue >= _reputationStateThresholds[ReputationState.Liked])
        {
            return ReputationState.Liked;
        }
        if (relationshipValue > _reputationStateThresholds[ReputationState.Disliked])
        {
            return ReputationState.Neutral;
        }
        if (relationshipValue > _reputationStateThresholds[ReputationState.Hated])
        {
            return ReputationState.Disliked;
        }
        return ReputationState.Hated;
    }
}
