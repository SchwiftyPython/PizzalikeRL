using System;
using System.Collections.Generic;

[Serializable]
public class ChestSdo : PropSdo
{
    public List<Guid> ContentIds;

    public ChestSdo(Chest chest)
    {
        ContentIds = new List<Guid>();

        foreach (var item in chest.GetContents())
        {
            ContentIds.Add(item.Id);
        }
    }
}
