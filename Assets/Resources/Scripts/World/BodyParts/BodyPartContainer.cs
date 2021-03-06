using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("BodyPartCollection")]
public class BodyPartContainer
{
    [XmlArray("BodyParts")]
    [XmlArrayItem("BodyPart")]
    public List<BodyPartTemplate> BodyPartTemplates = new List<BodyPartTemplate>();

    public static BodyPartContainer Load(string path)
    {
        var xml = Resources.Load<TextAsset>(path);

        var serializer = new XmlSerializer(typeof(BodyPartContainer));

        var reader = new StringReader(xml.text);

        var bodyParts = serializer.Deserialize(reader) as BodyPartContainer;

        reader.Close();

        return bodyParts;
    }
}