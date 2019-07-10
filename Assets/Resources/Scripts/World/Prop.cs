using UnityEngine;

public class Prop
{
    public GameObject Prefab { get; }

    public GameObject Texture;

	public Prop(GameObject prefab)
	{
	    Prefab = prefab;
	}
}
