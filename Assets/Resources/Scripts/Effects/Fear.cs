using System.Linq;
using UnityEngine;

public class Fear : Effect, ISubscriber
{
    private const int DefaultDuration = 3;

    public Fear(int duration, Entity target)
    {
        this.duration = duration < 1 ? DefaultDuration : duration;

        name = "fear";
        entity = target;
        remainingTurns = this.duration;

        entity.CanAttack = false;

        //todo this could be a property in Entity
        var entityName = target.Fluff != null ? target.Fluff.Name : target.EntityType;

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
        EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, $@"{entityName} is afraid!");

        var controller = entity.GetSprite().GetComponent<EnemyController>();

        controller.Goals.Clear();

        var scaryThing = FindScaryThing();

        new Flee(scaryThing, duration).Push(controller);

        EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, $@"{entityName} is fleeing!");
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (remainingTurns > 0)
            {
                remainingTurns--;
            }
            else
            {
                entity.CanAttack = true;

                EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, $"{entity.EntityType} is no longer afraid!");

                EventMediator.Instance.UnsubscribeFromAllEvents(this);
                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }

    private Entity FindScaryThing()
    {
        const int minSearchRadius = 4;
        const int maxSearchRadius = 16;

        var searchRadius = Random.Range(minSearchRadius, maxSearchRadius);

        var area = this.entity.CurrentArea;

        return (from entity in area.PresentEntities
            let distance = this.entity.CalculateDistanceToTarget(entity)
            where distance <= searchRadius
            let attitude = this.entity.GetAttitudeTowards(entity)
            where attitude == Attitude.Hostile
            select entity).FirstOrDefault();
    }
}
