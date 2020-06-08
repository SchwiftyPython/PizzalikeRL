using System;
using UnityEngine;

[Serializable]
public class CheeseTree : Prop
{
    public CheeseTree(string prefabKey, GameObject prefab) : base(prefabKey, prefab)
    {
    }

    public CheeseTree()
    {
        PrefabKey = 0.ToString();
        Prefab = WorldData.Instance.CheeseTreePrefab;
        IsContainer = false;
    }
}
