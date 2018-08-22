using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBackgroundLoader : MonoBehaviour
{
    private static string[] _characterBackgroundTypes;
    private static CharacterBackgroundContainer _characterBackgroundContainer;

    public TextAsset CharacterBackgroundFile;
   
    private void Awake ()
    {
        _characterBackgroundContainer = CharacterBackgroundContainer.Load(CharacterBackgroundFile);

        _characterBackgroundTypes = new string[_characterBackgroundContainer.CharacterBackgrounds.Count];


        var index = 0;

        foreach (var cb in _characterBackgroundContainer.CharacterBackgrounds)
        {
            _characterBackgroundTypes[index] = cb.name;

            index++;
        }
    }

    public static string[] GetCharacterBackgroundTypes()
    {
        return _characterBackgroundTypes;
    }

    public static string GetCharacterBackgroundTypeAt(int index)
    {
        return _characterBackgroundTypes[index];
    }

    public static int GetCharacterBackgroundTypesLength()
    {
        return _characterBackgroundTypes.Length;
    }

    public static CharacterBackground GetCharacterBackground(string characterBackgroundType)
    {
        var index = _characterBackgroundContainer.CharacterBackgrounds.FindIndex(item => item.Name.Equals(characterBackgroundType.ToLower()));
        var cb = _characterBackgroundContainer.CharacterBackgrounds[index];

        return cb;
    }
}
