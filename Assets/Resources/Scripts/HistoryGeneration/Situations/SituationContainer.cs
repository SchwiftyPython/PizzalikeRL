using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class SituationContainer  {

    [XmlArray("Situations")]
    [XmlArrayItem("Situation")]
    public List<Situation> Situations = new List<Situation>();

    public static SituationContainer Load(string path)
    {
        var xml = Resources.Load<TextAsset>(path);

        var serializer = new XmlSerializer(typeof(SituationContainer));

        var reader = new StringReader(xml.text);

        var situations = serializer.Deserialize(reader) as SituationContainer;

        reader.Close();

        return situations;
    }
}
