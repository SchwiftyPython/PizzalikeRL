using System.Collections.Generic;
using System.Xml.Serialization;

public class ItemTemplate
{
    [XmlAttribute("type")]
    public string Type;

    [XmlElement("Category")]
    public string Category;

    [XmlElement("BodyPartCategory")]
    public string BodyPartCategory;
}
