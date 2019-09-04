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

    [XmlElement("RequiresBodyPart")]
    public string RequiresBodyPart;

    [XmlElement("RequiresProperty")]
    public string RequiresProperty;

    [XmlElement("ParentAbility")]
    public string ParentAbility;

    [XmlElement("Dice")]
    public string Dice;

    [XmlElement("Duration")]
    public int Duration;

    [XmlElement("Cooldown")]
    public int Cooldown;

    //todo need boolean for starting ability
}
