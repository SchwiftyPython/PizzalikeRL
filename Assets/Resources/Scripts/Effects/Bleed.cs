using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : Effect, ISubscriber
{
    private readonly int _damagePerTurn;

    public Bleed(int duration, int amount, Entity target)
    {
        this.duration = duration;
        name = "bleed";
        entity = target;
        remainingTurns = duration;
        _damagePerTurn = amount;

        //todo message event target dazed
        //todo check if player for different message
        Debug.Log($"{entity.EntityType} is bleeding!");

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (remainingTurns > 0)
            {
                entity.ApplyRecurringDamage(_damagePerTurn);

                remainingTurns--;
            }
            else
            {
                //todo message event target dazed
                Debug.Log($"{entity.EntityType} is no longer bleeding!");

                EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);

                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }
}
