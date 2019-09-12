using System;

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

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (duration > 0)
            {
                entity.Heal(_amountToHealPerTurn);
                duration--;
            }
            else
            {
                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }
}
