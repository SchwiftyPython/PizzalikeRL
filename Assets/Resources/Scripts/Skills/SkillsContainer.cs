using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("SkillsCollection")]
public class SkillsContainer
{
    [XmlArray("Skills")]
    [XmlArrayItem("Skill")]
    public List<SkillTemplate> SkillsList = new List<SkillTemplate>();

    public static SkillsContainer Load(TextAsset xml)
    {
        var serializer = new XmlSerializer(typeof(SkillsContainer));

        var reader = new StringReader(xml.text);

        var skills = serializer.Deserialize(reader) as SkillsContainer;

        reader.Close();

        return skills;
    }
}
