using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("EntityTemplatesCollection")]
public class CharacterBackgroundContainer 
{
    [XmlArray("BackgroundCollection")]
    [XmlArrayItem("Background")]
    public List<CharacterBackground> CharacterBackgrounds = new List<CharacterBackground>();

    public static CharacterBackgroundContainer Load(TextAsset file)
    {
        var serializer = new XmlSerializer(typeof(CharacterBackgroundContainer));

        var reader = new StringReader(file.text);

        var characterBackgrounds = serializer.Deserialize(reader) as CharacterBackgroundContainer;

        reader.Close();

        return characterBackgrounds;
    }
}
