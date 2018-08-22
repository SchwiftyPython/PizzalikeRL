using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("EntityTemplatesCollection")]
public class EntityTemplatesContainer
{

    [XmlArray("EntityTemplates")]
    [XmlArrayItem("EntityTemplate")]
    public List<EntityTemplate> EntityTemplates = new List<EntityTemplate>();

    public static EntityTemplatesContainer Load(string path)
    {
        var xml = Resources.Load<TextAsset>(path);

        var serializer = new XmlSerializer(typeof(EntityTemplatesContainer));

        var reader = new StringReader(xml.text);

        var entityTemplates = serializer.Deserialize(reader) as EntityTemplatesContainer;

        reader.Close();

        return entityTemplates;
    }
}
