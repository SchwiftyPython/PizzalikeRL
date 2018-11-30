using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NameStore : MonoBehaviour
{
    #region FileInfo

    public static TextAsset HumanMaleFirstNamesFile;
    public static TextAsset HumanFemaleFirstNamesFile;
    public static TextAsset HumanLastNamesFile;
    public static TextAsset BirdMaleFirstNamesFile;
    public static TextAsset BirdFemaleFirstNamesFile;
    public static TextAsset BirdLastNamesFile;
    public static TextAsset MammalMaleFirstNamesFile;
    public static TextAsset MammalFemaleFirstNamesFile;
    public static TextAsset MammalLastNamesFile;
    public static TextAsset CowFirstNamesFile;

    private static readonly List<TextAsset> _nameFiles = new List<TextAsset>
    {
        HumanMaleFirstNamesFile,
        HumanFemaleFirstNamesFile,
        HumanLastNamesFile,
        BirdMaleFirstNamesFile,
        BirdFemaleFirstNamesFile,
        BirdLastNamesFile,
        MammalMaleFirstNamesFile,
        MammalFemaleFirstNamesFile,
        MammalLastNamesFile,
        CowFirstNamesFile
    };

    #endregion FileInfo

    private static List<string> _humanMaleFirstNames;
    private static List<string> _humanFemaleFirstNames;
    private static List<string> _humanLastNames;
    private static List<string> _birdMaleFirstNames;
    private static List<string> _birdFemaleFirstNames;
    private static List<string> _birdLastNames;
    private static List<string> _mammalMaleFirstNames;
    private static List<string> _mammalFemaleFirstNames;
    private static List<string> _mammalLastNames;
    private static List<string> _cowFirstNames;

    private static List<List<string>> _nameLists = new List<List<string>>
    {
        _humanMaleFirstNames,
        _humanFemaleFirstNames,
        _humanLastNames,
        _birdMaleFirstNames,
        _birdFemaleFirstNames,
        _birdLastNames,
        _mammalMaleFirstNames,
        _mammalFemaleFirstNames,
        _mammalLastNames,
        _cowFirstNames
    };

    private static List<string> _firstNames;
    private static List<string> _lastNames;

    private void Awake()
    {
        LoadNamesFromFiles();
    }

    public static string GenerateName(List<string> nameFiles)
    {
        string firstName;
        string lastName;
        try
        {
            _firstNames = PickFirstNameList();

            var index = Random.Range(0, _firstNames.Count);
            firstName = _firstNames[index];

            _lastNames = PickLastNameList();

            index = Random.Range(0, _lastNames.Count);
            lastName = _lastNames[index];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            firstName = string.Empty;
            lastName = string.Empty;
        }

        return firstName + " " + lastName;
    }

    private static List<string> PickFirstNameList()
    {
        return new List<string>();
    }

    private static List<string> PickLastNameList()
    {
        return new List<string>();
    }

    private static void LoadNamesFromFiles()
    {
        try
        {
            var nameListIndex = 0;
            foreach (var file in _nameFiles)
            {
                _nameLists[nameListIndex] = file.text.Split("\r\n"[0]).ToList();
                nameListIndex++;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error processing name file" + e.Message);
        }
        
    }
}
