using System.Collections.Generic;
using System.Xml.Serialization;

public class ItemTemplate
{
    [XmlAttribute("type")]
    public string Type;

    //todo may need to make enum
    [XmlElement("Category")]
    public string Category;

    [XmlElement("EquipmentSlotType")]
    public EquipmentSlotType EquipmentSlotType;

    [XmlElement("MultiSlot")]
    public bool MultiSlot;

    [XmlElement("Range")]
    public int Range;

    [XmlArray("Properties")]
    [XmlArrayItem("Property")]
    public List<string> Properties; //todo may need enum
}
