using System;
using System.Collections.Generic;

public class BodyPart
{
    public string Name;
    public string Type;
    public string NeedsPart;

    public bool CanEquipWeapon;
    public bool CanEquipArmor;

    public string AttackVerb;

    public BodyPart ParentBodyPart;
    public List<BodyPart> ChildrenBodyParts;
    public int MaxChildrenBodyParts;

    public int Coverage;
    public int CurrentHp;
    public int MaxHp;

    public Guid Id;
    public Guid ParentId;

    public BodyPart(BodyPartTemplate template)
    {
        Name = template.Name;
        Type = template.Type;
        NeedsPart = template.NeedsPart;
        CanEquipWeapon = template.CanEquipWeapon;
        CanEquipArmor = template.CanEquipArmor;
        AttackVerb = template.AttackVerb;
        MaxChildrenBodyParts = template.MaxChildrenBodyParts;
        Coverage = template.Coverage;
        CurrentHp = MaxHp = template.MaxHp;
        ChildrenBodyParts = new List<BodyPart>();
    }
}
