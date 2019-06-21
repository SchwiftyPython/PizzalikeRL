using System.Collections.Generic;
using UnityEngine;

public class EventMediator : MonoBehaviour
{
    private Dictionary<string, List<object>> _eventSubscriptions;

    public static EventMediator Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        _eventSubscriptions = new Dictionary<string, List<object>> ();
    }

    public void SubscribeToEvent(string eventName, object subscriber)
    {
        if (_eventSubscriptions.ContainsKey(eventName))
        {
            _eventSubscriptions[eventName].Add(subscriber);
        }
        else
        {
            _eventSubscriptions.Add(eventName, new List<object>{subscriber});
        }
    }

    public void UnsubscribeFromEvent(string eventName, object subscriber)
    {
        if (!_eventSubscriptions.ContainsKey(eventName))
        {
            return;
        }

        _eventSubscriptions[eventName].Remove(subscriber);
    }

    public void Broadcast(string eventName, object broadcaster)
    {
        if (!_eventSubscriptions.ContainsKey(eventName))
        {
            return;
        }

        var subscribers = _eventSubscriptions[eventName];

        foreach (var sub in subscribers)
        {
            NotifySubscriber(eventName, broadcaster, sub);
        }
    }

    private void NotifySubscriber(string eventName, object broadcaster, object subscriber)
    {
        //todo going to have to figure out if we're going to attempt to cast to classes with OnNotify method
    }
}
