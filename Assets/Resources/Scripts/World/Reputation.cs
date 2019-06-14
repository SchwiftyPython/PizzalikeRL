using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public enum ReputationState
{
    Loved,
    Liked,
    Neutral,
    Disliked,
    Hated
}

public enum Attitude
{
    Allied,
    Neutral,
    Hostile
}

public enum EntityGroupType
{
    Faction,
    EntityType
}

public enum Alignment
{
    Good,
    Neutral,
    Evil
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

    private static readonly Dictionary<ReputationState, float> GoodAlignmentStateValues =
        new Dictionary<ReputationState, float>
        {
            {ReputationState.Loved, .58f},
            {ReputationState.Liked, .17f},
            {ReputationState.Neutral, .16f},
            {ReputationState.Disliked, .05f},
            {ReputationState.Hated, .04f}
        };

    private static readonly Dictionary<ReputationState, float> NeutralAlignmentStateValues =
        new Dictionary<ReputationState, float>
        {
            {ReputationState.Loved, .01f},
            {ReputationState.Liked, .23f},
            {ReputationState.Neutral, .61f},
            {ReputationState.Disliked, .14f},
            {ReputationState.Hated, .01f}
        };

    private static readonly Dictionary<ReputationState, float> EvilAlignmentStateValues =
        new Dictionary<ReputationState, float>
        {
            {ReputationState.Loved, .03f},
            {ReputationState.Liked, .03f},
            {ReputationState.Neutral, .04f},
            {ReputationState.Disliked, .4f},
            {ReputationState.Hated, .5f}
        };

    private readonly Dictionary<Alignment, Dictionary<ReputationState, float>> _alignmentStateValues =
        new Dictionary<Alignment, Dictionary<ReputationState, float>>
        {
            {Alignment.Good, GoodAlignmentStateValues},
            {Alignment.Neutral, NeutralAlignmentStateValues},
            {Alignment.Evil, EvilAlignmentStateValues}
        };

    private readonly EntityGroupType _groupType;
    private readonly string _name;

    [Serializable]
    public class ReputationDictionary : SerializableDictionary<string, int>
    { }

    public ReputationDictionary Relationships;

    public Reputation(EntityGroupType groupType, string name)
    {
        _groupType = groupType;
        _name = name;
        Relationships = new ReputationDictionary();
        GenerateReputation();
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

    public ReputationState GetReputationStateForGroup(string otherGroup)
    {
        var relationshipValue = GetReputationValueForGroup(otherGroup);

        return GetReputationStateForValue(relationshipValue);
    }

    public int GetReputationValueForGroup(string otherGroup)
    {
        return Relationships[otherGroup];
    }

    public ReputationState GetReputationStateForValue(int relationshipValue)
    {
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

    private void GenerateReputation()
    {
        var entityGroupRelationships = WorldData.Instance.EntityGroupRelationships;

        var groupsOfSameType = entityGroupRelationships.Where(group => group.Value._groupType == _groupType)
            .ToDictionary(group => group.Key, group => group.Value);

        var alignment = GetRandomAlignment<Alignment>();

        var alignmentValues = _alignmentStateValues[alignment];

        var loveCount = (int) (groupsOfSameType.Count * alignmentValues[ReputationState.Loved]);
        var likeCount = (int) (groupsOfSameType.Count * alignmentValues[ReputationState.Liked]);
        var neutralCount = (int) (groupsOfSameType.Count * alignmentValues[ReputationState.Neutral]);
        var dislikeCount = (int) (groupsOfSameType.Count * alignmentValues[ReputationState.Disliked]);
        var hateCount = (int) (groupsOfSameType.Count * alignmentValues[ReputationState.Hated]);

        SetReputationForState(ReputationState.Loved, loveCount, groupsOfSameType, entityGroupRelationships);
        SetReputationForState(ReputationState.Liked, likeCount, groupsOfSameType, entityGroupRelationships);
        SetReputationForState(ReputationState.Neutral, neutralCount, groupsOfSameType, entityGroupRelationships);
        SetReputationForState(ReputationState.Disliked, dislikeCount, groupsOfSameType, entityGroupRelationships);
        SetReputationForState(ReputationState.Hated, hateCount, groupsOfSameType, entityGroupRelationships);

        while (groupsOfSameType.Count > 0)
        {
            SetReputationForState(ReputationState.Neutral, groupsOfSameType.Count, groupsOfSameType,
                entityGroupRelationships);
        }
    }

    //todo move contents to global helper and use this as wrapper
    private static T GetRandomAlignment<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T) values.GetValue(Random.Range(0, values.Length));
    }

    private void SetReputationForState(ReputationState state, int count,
        Dictionary<string, Reputation> groupsOfSameType,
        IReadOnlyDictionary<string, Reputation> entityGroupRelationships)
    {
        for (var i = 0; i < count; i++)
        {
            var group = groupsOfSameType.Keys.ElementAt(Random.Range(0, groupsOfSameType.Count));

            if (state == ReputationState.Neutral)
            {
                entityGroupRelationships[group].Relationships.Add(_name, 0);

                Relationships.Add(group, 0);
            }
            else
            {
                entityGroupRelationships[group].Relationships.Add(_name, _reputationStateThresholds[state]);

                Relationships.Add(group, _reputationStateThresholds[state]);
            }

            groupsOfSameType.Remove(group);
        }
    }
}
