using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("ItemTemplateCollection")]
public class ItemTemplateContainer
{
    [XmlArray("ItemTemplates")]
    [XmlArrayItem("ItemTemplate")]
    public List<ItemTemplate> ItemTemplates = new List<ItemTemplate>();

    public static ItemTemplateContainer Load(string path)
    {
        var xml = Resources.Load<TextAsset>(path);

        var serializer = new XmlSerializer(typeof(ItemTemplateContainer));

        var reader = new StringReader(xml.text);

        var itemTemplates = serializer.Deserialize(reader) as ItemTemplateContainer;

        reader.Close();

        return itemTemplates;
    }
}