using UnityEngine;

public class Daze : Effect, ISubscriber
{
    private const int AttributePenalty = 5;

    public Daze(int duration, Entity target)
    {
        this.duration = duration;
        name = "daze";
        entity = target;
        remainingTurns = duration;

        entity.Agility -= AttributePenalty;
        entity.Intelligence -= AttributePenalty;
        entity.Strength -= AttributePenalty;

        //todo message event target dazed
        //todo check if player for different message
        Debug.Log($"{entity.EntityType} is dazed for {duration} turns!");
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
                entity.Agility += AttributePenalty;
                entity.Intelligence += AttributePenalty;
                entity.Strength += AttributePenalty;

                //todo message event target dazed
                Debug.Log($"{entity.EntityType} is no longer dazed!");

                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }
}
