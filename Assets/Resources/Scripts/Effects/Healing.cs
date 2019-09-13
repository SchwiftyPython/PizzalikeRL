using System;
using UnityEngine;

[Serializable]
public class Healing : Effect, ISubscriber
{
    private readonly int _amountToHealPerTurn;

    public Healing(int duration, int amount, Entity entity)
    {
        this.duration = duration;
        name = "healing";
        this.entity = entity;
        _amountToHealPerTurn = amount;

        if (duration < 0)
        {
            entity.Heal(_amountToHealPerTurn); //todo event

            Debug.Log($"{entity.EntityType} healed for {_amountToHealPerTurn}");

            //todo message event
            return;
        }

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (duration > 0)
            {
                entity.Heal(_amountToHealPerTurn); //todo event
                duration--;

                Debug.Log($"{entity.EntityType} healed for {_amountToHealPerTurn}");

                //todo message event
            }
            else
            {
                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }
}
