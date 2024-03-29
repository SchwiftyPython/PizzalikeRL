﻿using System.Xml.Serialization;

public class BodyPartTemplate
{
    [XmlAttribute("name")]
    public string Name;

    [XmlElement("Type")]
    public string Type;

    [XmlElement("MaxHP")]
    public int MaxHp;

    [XmlElement("CanEquipWeapon")]
    public bool CanEquipWeapon;

    [XmlElement("CanEquipArmor")]
    public bool CanEquipArmor;

    [XmlElement("NeedsPart")]
    public string NeedsPart;

    [XmlElement("AttackVerb")]
    public string AttackVerb;

    [XmlElement("Coverage")]
    public int Coverage;

    [XmlElement("MaxChildrenBodyParts")]
    public int MaxChildrenBodyParts;
}
