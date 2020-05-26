using System;

[Serializable]
public class FieldSdo : PropSdo
{
    public FieldType Type;

    public FieldSdo(Field field)
    {
        Type = field.Type;
    }
}
