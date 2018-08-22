using System.Xml.Serialization;
using UnityEngine;

public class CharacterBackground : MonoBehaviour
{
    [XmlAttribute("name")]
    public string Name;

    [XmlElement("Description")]
    public string Description;
}
