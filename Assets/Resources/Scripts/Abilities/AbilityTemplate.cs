using System.Xml.Serialization;

public class AbilityTemplate
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

    [XmlElement("Cooldown")]
    public int Cooldown;

    [XmlElement("Target")]
    public string Target;

    [XmlElement("Effect")]
    public string Effect;

    [XmlElement("Range")]
    public int Range;

    [XmlElement("StartingAbility")]
    public bool StartingAbility;

    [XmlElement("UsesConsumables")]
    public bool UsesConsumables;
}
