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


        if (entity.IsPlayer())
        {
            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                "You are bleeding!");
        }
        else
        {
            EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                $"{entity.Name} is bleeding!");
        }

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
    }

    public override void Remove()
    {
       EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);
       entity = null;
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName && parameter == entity)
        {
            if (remainingTurns > 0)
            {
                entity.ApplyRecurringDamage(_damagePerTurn);

                remainingTurns--;
            }
            else
            {
                if (entity.IsPlayer())
                {
                    EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                        "You are no longer bleeding.");
                }
                else
                {
                    EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this,
                        $"{entity.Name} is no longer bleeding.");
                }

                EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);

                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }
}
