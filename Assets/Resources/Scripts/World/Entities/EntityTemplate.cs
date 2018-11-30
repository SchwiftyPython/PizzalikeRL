using System.Collections.Generic;
using System.Xml.Serialization;

public class EntityTemplate {

    [XmlAttribute("type")]
    public string Type;

    [XmlElement("Classification")]
    public Entity.EntityClassification Classification;

    [XmlElement("Description")]
    public string Description;

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

    [XmlElement("MinIntelligence")]
    public int MinIntelligence;

    [XmlElement("MaxIntelligence")]
    public int MaxIntelligence;

    [XmlElement("Playable")]
    public bool Playable;

    [XmlElement("Wild")]
    public bool Wild;

    [XmlElement("IsHostile")]
    public bool IsHostile;

    [XmlArray("PossibleInventory")]
    [XmlArrayItem("Item")]
    public List<Item> PossibleInventory;    

    [XmlElement("SpritePath")]
    public string SpritePath;

    [XmlArray("BodyParts")]
    [XmlArrayItem("Part")]
    public List<string> Parts;

    [XmlArray("PossibleBiomes")]
    [XmlArrayItem("Biome")]
    public List<BiomeType> Biomes;

    [XmlArray("NameFiles")]
    [XmlArrayItem("NameFile")]
    public List<string> NameFiles;
}
