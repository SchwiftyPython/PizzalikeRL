using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NameStore : MonoBehaviour
{
    #region FileInfo

    public TextAsset[] NameFiles;

    private readonly Dictionary<string, TextAsset> _nameFiles = new Dictionary<string, TextAsset>
    {
        {"human_male_first_names", null},
        {"human_female_first_names", null},
        {"human_last_names", null},
        {"bird_male_first_names", null},
        {"bird_female_first_names", null},
        {"bird_last_names", null},
        {"mammal_male_first_names", null},
        {"mammal_female_first_names", null},
        {"mammal_last_names", null},
        {"cow_female_first_names", null}
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

    private readonly List<List<string>> _nameLists = new List<List<string>>
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

    private  List<string> _firstNames;
    private  List<string> _lastNames;

    public static NameStore Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        LoadNamesFromFiles();
    }

    private  void LoadNamesFromFiles()
    {
        try
        {
            var nameFilesIndex = 0;
            foreach (var file in _nameFiles.Keys.ToArray())
            {
                _nameFiles[file] = NameFiles[nameFilesIndex];
                nameFilesIndex++;
            }

            var nameListIndex = 0;
            foreach (var file in _nameFiles.Values)
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

    private void FilterPossibleNameListsBySex(IEnumerable<string> nameFiles, string sex)
    {
        _firstNames = new List<string>();
        _lastNames = new List<string>();
        
        foreach (var nameFile in nameFiles)
        {
            if (nameFile.Contains(sex))
            {
                _firstNames.AddRange(_nameFiles[nameFile].text.Split("\r\n"[0]).ToList());
            }
            if (nameFile.Contains("last"))
            {
                _lastNames.AddRange(_nameFiles[nameFile].text.Split("\r\n"[0]).ToList());
            }
        }
    }

    public string GenerateFullName(List<string> nameFiles, string sex)
    {
        string firstName;
        string lastName;
        try
        {
            FilterPossibleNameListsBySex(nameFiles, sex);

            var index = Random.Range(0, _firstNames.Count);
            firstName = _firstNames[index].Trim('\n');

            index = Random.Range(0, _lastNames.Count);
            lastName = _lastNames[index].Trim('\n');
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            firstName = string.Empty;
            lastName = string.Empty;
        }

        return firstName + " " + lastName;
    }

    public  string GenerateFirstName(List<string> nameFiles, string sex)
    {
        FilterPossibleNameListsBySex(nameFiles, sex);

        var index = Random.Range(0, _firstNames.Count);
        return _firstNames[index].Trim('\n');
    }
}
