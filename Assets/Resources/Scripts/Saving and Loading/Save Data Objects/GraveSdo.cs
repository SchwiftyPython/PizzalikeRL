using System;

[Serializable]
public class GraveSdo : PropSdo
{
    public Guid Id;
    public Grave.Inscription Inscription;

    public GraveSdo(Grave grave)
    {
        Id = grave.Id;
        Inscription = grave.inscription;
    }
}
