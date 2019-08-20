using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, ISubscriber
{
    private const string ButtonClickEventName = "Clicked";
    private const string MeleeHitEventName = "MeleeHit";

    private readonly IList<string> _subscribedEvents = new List<string>
    {
        ButtonClickEventName,
        MeleeHitEventName
    };

    public AudioClip Click;
    public AudioClip MeleeHit;

    public AudioSource SoundSource;

    private void Awake()
    {
        SubscribeToEvents();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(ButtonClickEventName))
        {
            SoundSource.clip = Click;
            SoundSource.volume = .75f;
            Play();
        }
        else if (eventName.Equals(MeleeHitEventName))
        {
            SoundSource.clip = MeleeHit;
            SoundSource.volume = .75f;
            Play();
        }
    }

    private void Play()
    {
        SoundSource.Play();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        foreach (var eventName in _subscribedEvents)
        {
            EventMediator.Instance.SubscribeToEvent(eventName, this);
        }
    }

    private void UnsubscribeFromEvents()
    {
        EventMediator.Instance.UnsubscribeFromAllEvents(this);
    }
}
