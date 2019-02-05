using System;
using System.Xml.Serialization;

[Serializable]
public class CharacterBackground
{
    [XmlAttribute("name")]
    public string Name;

    [XmlElement("Description")]
    public string Description;
}
