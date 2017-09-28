using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("BodyPartCollection")]
public class BodyPartContainer{

    [XmlArray("BodyParts")]
    [XmlArrayItem("BodyPart")]
    public List<BodyPart> bodyParts = new List<BodyPart>();

    public static BodyPartContainer Load(string path){
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(BodyPartContainer));

        StringReader reader = new StringReader(_xml.text);

        BodyPartContainer bodyParts = serializer.Deserialize(reader) as BodyPartContainer;

        reader.Close();

        return bodyParts;
    }
}