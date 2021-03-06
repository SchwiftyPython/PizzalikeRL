﻿using System;
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

            //todo message event
            return;
        }

        remainingTurns = duration;

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (remainingTurns > 0)
            {
                entity.Heal(_amountToHealPerTurn); //todo event
                remainingTurns--;

                Debug.Log($"{entity.EntityType} healed for {_amountToHealPerTurn}");

                //todo message event
            }
            else
            {
                EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);

                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }
}
