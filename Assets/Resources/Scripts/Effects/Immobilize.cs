using UnityEngine;

public class Immobilize : Effect, ISubscriber
{
    public Immobilize(int duration)
    {
        this.duration = duration;
        name = "immobilize";
        remainingTurns = duration;
    }

    public override void Apply(Entity target)
    {
        entity = target;

        entity.Mobile = false;

        Debug.Log($"{entity.EntityType} is immobile!");

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        //todo check if it was this entity's turn
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (remainingTurns > 0)
            {
                remainingTurns--;
            }
            else
            {
                entity.Mobile = true;

                GlobalHelper.DestroyObject(entity.CurrentTile.AbilityTexture);

                Debug.Log($"{entity.EntityType} is no longer immobile!");

                EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);

                EventMediator.Instance.Broadcast(GlobalHelper.EffectDoneEventName, this);
            }
        }
    }
}