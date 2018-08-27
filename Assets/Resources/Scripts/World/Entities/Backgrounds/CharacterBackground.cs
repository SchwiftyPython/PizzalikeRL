using System.Xml.Serialization;

public class CharacterBackground
{
    [XmlAttribute("name")]
    public string Name;

    [XmlElement("Description")]
    public string Description;
}
