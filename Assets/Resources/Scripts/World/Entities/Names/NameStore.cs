﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private List<string> _firstNames;
    private List<string> _lastNames;

    public NameStore(string entityType, string sex)
    {
        if (entityType.Equals("human"))
        {
            _firstNames = LoadNamesFromFile(sex.Equals("male") ? HumanMaleFirstNamesFile : HumanFemaleFirstNamesFile);
            _lastNames = LoadNamesFromFile(HumanLastNamesFile);
        }
        if (entityType.Equals("dwarf"))
        {
            _firstNames = LoadNamesFromFile(sex.Equals("male") ? DwarfMaleFirstNamesFile : DwarfFemaleFirstNamesFile);
            _lastNames = LoadNamesFromFile(DwarfLastNamesFile);
        }
    }

    public string GenerateName()
    {
        var firstName = _firstNames[Random.Range(0, _firstNames.Count)];
        var lastName = _lastNames[Random.Range(0, _lastNames.Count)];

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
                    names.AddRange(line.Split(','));
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
