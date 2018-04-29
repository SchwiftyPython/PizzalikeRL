using System.Collections.Generic;
using System.Xml.Serialization;

public class EntityTemplate {

    [XmlAttribute("type")]
    public string Type;

    [XmlElement("MinStrength")]
    public int MinStrength;

    [XmlElement("MaxStrength")]
    public int MaxStrength;

    [XmlElement("MinAgility")]
    public int MinAgility;

    [XmlElement("MaxAgility")]
    public int MaxAgility;

    [XmlElement("MinConstitution")]
    public int MinConstitution;

    [XmlElement("MaxConstitution")]
    public int MaxConstitution;

    [XmlElement("Playable")]
    public bool Playable;

    [XmlElement("Nameable")]
    public bool Nameable;

    [XmlElement("CanMutate")]
    public bool CanMutate;

    [XmlElement("IsHostile")]
    public bool IsHostile;

    [XmlArray("PossibleInventory")]
    [XmlArrayItem("Item")]
    public List<Item> PossibleInventory;    

    [XmlElement("SpritePath")]
    public string SpritePath;

    /*
    [XmlArray("AITypes")]
    [XmlArrayItem("AI")]
    */

    [XmlArray("BodyParts")]
    [XmlArrayItem("Part")]
    public List<string> Parts;

    [XmlArray("PossibleBiomes")]
    [XmlArrayItem("Biome")]
    public List<string> Biomes;
}
