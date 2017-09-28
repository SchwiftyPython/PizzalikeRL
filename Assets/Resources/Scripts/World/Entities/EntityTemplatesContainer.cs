using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("EntityTemplatesCollection")]
public class EntityTemplatesContainer {

    [XmlArray("EntityTemplates")]
    [XmlArrayItem("EntityTemplate")]
    public List<EntityTemplate> entityTemplates = new List<EntityTemplate>();

    public static EntityTemplatesContainer Load(string path) {
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(EntityTemplatesContainer));

        StringReader reader = new StringReader(_xml.text);

        EntityTemplatesContainer entityTemplates = serializer.Deserialize(reader) as EntityTemplatesContainer;

        reader.Close();

        return entityTemplates;
    }
}
