using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NameStore
{
    #region FileInfo
    private const string NamesPath = "Assets\\Resources\\Scripts\\World\\Entities\\Names";
    private const string HumanMaleFirstNamesFile = "human_male_first_names.csv";
    private const string HumanFemaleFirstNamesFile = "human_female_first_names.csv";
    private const string HumanLastNamesFile = "human_last_names.csv";
    private const string DwarfMaleFirstNamesFile = "dwarf_male_first_names.csv";
    private const string DwarfFemaleFirstNamesFile = "dwarf_female_first_names.csv";
    private const string DwarfLastNamesFile = "dwarf_last_names.csv";
    #endregion FileInfo

    private readonly List<string> _firstNames;
    private readonly List<string> _lastNames;

    public NameStore(string entityType, string sex)
    {
        if (entityType.Equals("human"))
        {
            _firstNames = LoadNamesFromFile(sex.ToLower().Equals("male") ? HumanMaleFirstNamesFile : HumanFemaleFirstNamesFile);
            _lastNames = LoadNamesFromFile(HumanLastNamesFile);
        }
        if (entityType.Equals("dwarf"))
        {
            _firstNames = LoadNamesFromFile(sex.ToLower().Equals("male") ? DwarfMaleFirstNamesFile : DwarfFemaleFirstNamesFile);
            _lastNames = LoadNamesFromFile(DwarfLastNamesFile);
        }
    }

    public string GenerateName()
    {
        var firstName = String.Empty;
        var lastName = String.Empty;
        try
        {
            var index = Random.Range(0, _firstNames.Count);
            firstName = _firstNames[index];

            index = Random.Range(0, _lastNames.Count);
            lastName = _lastNames[index];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            firstName = String.Empty;
            lastName = String.Empty;
        }

        return firstName + " " + lastName;
    }

    private static List<string> LoadNamesFromFile(string filePath)
    {
        var basePath = Environment.CurrentDirectory;

        var fullPath = Path.Combine(basePath, NamesPath.TrimStart('\\', '/'), filePath);

        var names = new List<string>();
        try
        {
            using (var reader = new StreamReader(fullPath))
            {
                string line;
                while (null != (line = reader.ReadLine()?.Trim()))
                {
                    var tempNames = line.Split(',');
                    names.AddRange(from n in tempNames
                                   where n != string.Empty
                                   select n);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error processing file: " + fullPath + " " + e.Message);
        }
        return names;
    }
}
