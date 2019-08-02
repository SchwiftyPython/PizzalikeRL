using UnityEngine;

public class Prop
{
    public GameObject Prefab { get; }

    public GameObject Texture;

    public bool IsContainer;

	public Prop(GameObject prefab, bool isContainer = false)
	{
	    Prefab = prefab;
	    IsContainer = isContainer;
	}

    protected Prop()
    {
        throw new System.NotImplementedException();
    }
}
