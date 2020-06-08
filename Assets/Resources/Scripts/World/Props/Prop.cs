using System.Collections.Generic;
using UnityEngine;

public class Prop
{
    public GameObject Prefab { get; set; }

    public string PrefabKey;

    public GameObject Texture;

    public bool IsContainer;

    public List<string> EventsTriggeredBy;

    public Prop(string prefabKey, GameObject prefab, bool isContainer = false)
    {
        PrefabKey = prefabKey;
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
