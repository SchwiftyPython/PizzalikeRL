using System.Collections.Generic;
using UnityEngine;

public class SkillsLoader : MonoBehaviour
{
    private static List<string> _skillNames;
    private static SkillsContainer _sc;

    public static Dictionary<string, SkillTemplate> SkillsDictionary;
    public TextAsset SkillsFile;

    private void Awake()
    {
        LoadSkillsFromFile();
        PopulateSkillsDictionary();
    }

    public SkillTemplate GetSkillByName(string skillName)
    {
        if (_sc == null)
        {
            LoadSkillsFromFile();
        }

        if (_sc == null)
        {
            Debug.Log("Can't get skill! Skill container is null!");
            return null;
        }

        if (SkillsDictionary.ContainsKey(skillName))
        {
            return SkillsDictionary[skillName];
        }

        Debug.Log("Can't get skill! Skill name doesn't exist!");
        return null;
    }

    public List<string> GetSkillNames()
    {
        if (_skillNames == null)
        {
            LoadSkillsFromFile();
        }

        return _skillNames != null ? new List<string>(_skillNames) : null;
    }

    public static Dictionary<string, SkillTemplate> GetAllSkills()
    {
        return new Dictionary<string, SkillTemplate>(SkillsDictionary);
    }

    private void LoadSkillsFromFile()
    {
        _sc = SkillsContainer.Load(SkillsFile);

        _skillNames = new List<string>();

        foreach (var s in _sc.SkillsList)
        {
            _skillNames.Add(s.Name);
        }
    }

    private static void PopulateSkillsDictionary()
    {
        SkillsDictionary = new Dictionary<string, SkillTemplate>();

        foreach (var skill in _sc.SkillsList)
        {
            var key = skill.Name;

            if (SkillsDictionary.ContainsKey(key))
            {
                continue;
            }

            SkillsDictionary.Add(key, skill);
        }
    }
}
