using System.Collections.Generic;
using System.Xml.Serialization;

public class Situation {

    [XmlAttribute("name")]
    public string Name;

    [XmlArray("Types")]
    [XmlArrayItem("Type")]
    public List<string> Types;

    [XmlArray("Actors")]
    [XmlArrayItem("Actor")]
    public List<Entity> Actors;

    [XmlElement("ConditionCode")]
    public string ConditionCode;

    [XmlArray("AllowedFactions")]
    [XmlArrayItem("Faction")]
    public List<Faction> AllowedFactions;
}
