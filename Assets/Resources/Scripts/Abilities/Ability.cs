using System.Xml.Serialization;

public class Ability
{
    [XmlAttribute("name")]
    public string Name;
    
    [XmlElement("Cost")]
    public int Cost;

    [XmlElement("Attribute")]
    public string Attribute;

    [XmlElement("AttributePrereq")]
    public string AttributePrereq;

    [XmlElement("RequiresBackground")]
    public string RequiresBackground;

    [XmlElement("Description")]
    public string Description;

    [XmlElement("RequiresProperty")]
    public string RequiresProperty;

    [XmlElement("ParentAbility")]
    public string ParentAbility;

    [XmlElement("Cooldown")]
    public int Cooldown;
}
