using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class CharacterBackground
{
    [XmlAttribute("name")]
    public string Name;

    [XmlElement("Description")]
    public string Description;

    [XmlArray("Attributes")]
    [XmlArrayItem("Attribute")]
    public List<AttributeModification> AttributeModifications;
}

[Serializable]
public class AttributeModification
{
    [XmlElement("Name")]
    public string Name;

    [XmlElement("Value")]
    public int Value;
}
