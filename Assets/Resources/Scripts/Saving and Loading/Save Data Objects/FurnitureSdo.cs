using System;

[Serializable]
public class FurnitureSdo : PropSdo
{
    public FurnitureSdo(Furniture furniture)
    {
        PrefabKey = furniture.PrefabKey;
    }
}
