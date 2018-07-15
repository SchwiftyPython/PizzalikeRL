using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("FactionTemplateCollection")]
public class FactionTemplateContainer  {

    [XmlArray("FactionTemplates")]
    [XmlArrayItem("FactionTemplate")]
    public List<FactionTemplate> FactionTemplates = new List<FactionTemplate>();

    public static FactionTemplateContainer Load(string path)
    {
        var xml = Resources.Load<TextAsset>(path);

        var serializer = new XmlSerializer(typeof(FactionTemplateContainer));

        var reader = new StringReader(xml.text);

        var FactionTemplates = serializer.Deserialize(reader) as FactionTemplateContainer;

        reader.Close();

        return FactionTemplates;
    }

}
