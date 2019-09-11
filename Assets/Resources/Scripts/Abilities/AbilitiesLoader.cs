using System.Collections.Generic;
using UnityEngine;

public class AbilitiesLoader : MonoBehaviour
{
    private static List<string> _abilityNames;
    private static AbilitiesContainer _ac;

    public static Dictionary<string, AbilityTemplate> AbilitiesDictionary;
    public TextAsset AbilitiesFile;

    private void Awake()
    {
        LoadAbilitiesFromFile();
        PopulateAbilitiesDictionary();
    }

    public AbilityTemplate GetAbilityByName(string abilityName)
    {
        if (_ac == null)
        {
            LoadAbilitiesFromFile();
        }
        if (_ac == null)
        {
            Debug.Log("Can't get ability! Ability container is null!");
            return null;
        }

        return AbilitiesDictionary[abilityName];
    }

    public List<string> GetAbilityNames()
    {
        if (_abilityNames == null)
        {
            LoadAbilitiesFromFile();
        }

        return _abilityNames != null ? new List<string>(_abilityNames) : null;
    }

    public static Dictionary<string, AbilityTemplate> GetAllAbilities()
    {
        return new Dictionary<string, AbilityTemplate>(AbilitiesDictionary);
    }

    private void LoadAbilitiesFromFile()
    {
        _ac = AbilitiesContainer.Load(AbilitiesFile);

        _abilityNames = new List<string>();

        foreach (var a in _ac.AbilitiesList)
        {
            _abilityNames.Add(a.Name);
        }
    }

    private static void PopulateAbilitiesDictionary()
    {
        AbilitiesDictionary = new Dictionary<string, AbilityTemplate>();

        foreach (var ability in _ac.AbilitiesList)
        {
            var key = ability.Name;

            if (AbilitiesDictionary.ContainsKey(key))
            {
                continue;
            }

            AbilitiesDictionary.Add(key, ability);
        }
    }
}
