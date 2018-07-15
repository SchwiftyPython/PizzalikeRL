using System.Collections.Generic;
using System.Xml.Serialization;

public class FactionTemplate
{
    [XmlAttribute("type")]
    public string Type;

    [XmlArray("EntityTypes")]
    [XmlArrayItem("Type")]
    public List<string> EntityTypes;
}
