using UnityEngine;

public class Furniture : Prop
{
    public Furniture(string prefabKey, GameObject prefab)
    {
        PrefabKey = prefabKey;
        Prefab = prefab;
    }

    public Furniture(FurnitureSdo sdo)
    {
        PrefabKey = sdo.PrefabKey;
        Prefab = BuildingPrefabStore.GetPrefabByName(PrefabKey);
    }
}
