using System.Collections.Generic;
using System.Xml.Serialization;

public class Situation {

    [XmlAttribute("name")]
    public string Name;

    [XmlArray("Types")]
    [XmlArrayItem("Type")]
    public List<string> Types;

    [XmlElement("ConditionCode")]
    public string ConditionCode;

    [XmlArray("AllowedFactions")]
    [XmlArrayItem("AllowedFaction")]
    public List<string> AllowedFactions;
}
