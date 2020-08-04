using System.Xml.Serialization;

public class SkillTemplate 
{
    [XmlAttribute("name")]
    public string Name;

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

    [XmlElement("Dice")]
    public string Dice;

    [XmlElement("StartingSkill")]
    public bool StartingSkill;
}
