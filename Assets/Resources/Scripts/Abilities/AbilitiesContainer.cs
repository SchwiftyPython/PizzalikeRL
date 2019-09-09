using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("AbilitiesCollection")]
public class AbilitiesContainer 
{
    [XmlArray("Abilities")]
    [XmlArrayItem("Ability")]
    public List<AbilityTemplate> AbilitiesList = new List<AbilityTemplate>();

    public static AbilitiesContainer Load(TextAsset xml)
    {
        var serializer = new XmlSerializer(typeof(AbilitiesContainer));

        var reader = new StringReader(xml.text);

        var abilities = serializer.Deserialize(reader) as AbilitiesContainer;

        reader.Close();

        return abilities;
    }
}
