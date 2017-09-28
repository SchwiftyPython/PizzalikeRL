using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class BodyPart {

    [XmlAttribute("name")]
    public string name;

    [XmlElement("MaxHP")]
    public int maxHP;

    [XmlElement("CanEquipWeapon")]
    public bool canEquipWeapon;

    [XmlElement("CanEquipArmor")]
    public bool canEquipArmor;

    [XmlElement("NeedsPart")]
    public string needsPart;

    [XmlElement("AttackVerb")]
    public string attackVerb;

    [XmlElement("Coverage")]
    public int coverage;    
}
