using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class EntityTemplate {

    [XmlAttribute("type")]
    public string type;

    [XmlElement("MinStrength")]
    public int minStrength;

    [XmlElement("MaxStrength")]
    public int maxStrength;

    [XmlElement("MinAgility")]
    public int minAgility;

    [XmlElement("MaxAgility")]
    public int maxAgility;

    [XmlElement("MinConstitution")]
    public int minConstitution;

    [XmlElement("MaxConstitution")]
    public int maxConstitution;

    [XmlElement("Playable")]
    public bool playable;

    [XmlElement("Nameable")]
    public bool nameable;

    [XmlElement("CanMutate")]
    public bool canMutate;

    [XmlElement("IsHostile")]
    public bool isHostile;

    [XmlArray("PossibleInventory")]
    [XmlArrayItem("Item")]
    public List<Item> possibleInventory;    

    [XmlElement("SpritePath")]
    public string spritePath;

    /*
    [XmlArray("AITypes")]
    [XmlArrayItem("AI")]
    */

    [XmlArray("BodyParts")]
    [XmlArrayItem("Part")]
    public List<BodyPart> parts;
}
