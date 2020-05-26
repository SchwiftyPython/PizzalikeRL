using System.Collections.Generic;
using UnityEngine;

public class Prop
{
    public GameObject Prefab { get; set; }

    public GameObject Texture;

    public bool IsContainer;

    public List<string> EventsTriggeredBy;

    public Prop(GameObject prefab, bool isContainer = false)
	{
	    Prefab = prefab;
	    IsContainer = isContainer;
	}

    protected Prop() { }

    public bool TriggeredByEvent(string eventName)
    {
        return EventsTriggeredBy.Contains(eventName);
    }

    public virtual void Trigger(string eventName, object parameter = null) { }
}
