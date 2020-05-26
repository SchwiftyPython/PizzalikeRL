using System;
using UnityEngine;

[Serializable]
public class CheeseTree : Prop
{
    public CheeseTree(GameObject prefab) : base(prefab)
    {
    }

    public CheeseTree()
    {
        Prefab = WorldData.Instance.CheeseTreePrefab;
        IsContainer = false;
    }
}
