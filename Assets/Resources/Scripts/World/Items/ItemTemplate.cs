using System.Xml.Serialization;

public class ItemTemplate
{
    [XmlAttribute("type")]
    public string Type;

    //todo may need to make enum
    [XmlElement("Category")]
    public string Category;

    [XmlElement("BodyPartCategory")]
    public string BodyPartCategory;
}
