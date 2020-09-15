using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillsStore : MonoBehaviour
{
    private static Dictionary<string, SkillTemplate> _allSkillTemplates;
    private static Dictionary<string, List<SkillTemplate>> _skillsByBackgroundRequirement;
    private static Dictionary<string, List<SkillTemplate>> _backgroundStartingSkills;
    private static Dictionary<string, List<SkillTemplate>> _skillsByBodyPart;

    private static List<string> _streetRatStartingSkillNames;
    private static List<string> _arenaFighterStartingSkillNames;
    private static List<string> _militaryStartingSkillNames;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _allSkillTemplates = SkillsLoader.GetAllSkills();

        PopulateSkillsByBodyPart();
        PopulateSkillsByRequiredBackground();
        PopulateStartingSkillsForBackgrounds();
    }

    private static void PopulateSkillsByRequiredBackground()
    {
        _skillsByBackgroundRequirement = new Dictionary<string, List<SkillTemplate>>();

        var backgrounds = CharacterBackgroundLoader.GetCharacterBackgroundTypes();

        foreach (var background in backgrounds)
        {
            if (_skillsByBackgroundRequirement.ContainsKey(background))
            {
                continue;
            }

            _skillsByBackgroundRequirement.Add(background, new List<SkillTemplate>());

            foreach (var skill in _allSkillTemplates.Values.Where(skill =>
                skill.RequiresBackground.Trim().Equals(background, StringComparison.OrdinalIgnoreCase)))
            {
                _skillsByBackgroundRequirement[background].Add(skill);
            }
        }
    }

    private static void PopulateStartingSkillsForBackgrounds()
    {
        _backgroundStartingSkills = new Dictionary<string, List<SkillTemplate>>
        {
            {"street rat", new List<SkillTemplate>()},
            {"arena fighter", new List<SkillTemplate>()},
            {"military", new List<SkillTemplate>()}
        };

        _streetRatStartingSkillNames = new List<string>
        {
            "scavenger"
        };

        _arenaFighterStartingSkillNames = new List<string>
        {
            "cat-like reflexes"
        };

        _militaryStartingSkillNames = new List<string>
        {
            "axe mastery"
        };

        foreach (var skillTemplate in _allSkillTemplates.Values)
        {
            if (_streetRatStartingSkillNames.Contains(skillTemplate.Name.ToLower()))
            {
                _backgroundStartingSkills["street rat"].Add(skillTemplate);
            }
            if (_arenaFighterStartingSkillNames.Contains(skillTemplate.Name.ToLower()))
            {
                _backgroundStartingSkills["arena fighter"].Add(skillTemplate);
            }
            if (_militaryStartingSkillNames.Contains(skillTemplate.Name.ToLower()))
            {
                _backgroundStartingSkills["military"].Add(skillTemplate);
            }
        }
    }

    private static void PopulateSkillsByBodyPart()
    {
        _skillsByBodyPart = new Dictionary<string, List<SkillTemplate>>();

        var bodyParts = BodyPartLoader.BodyPartNames;

        foreach (var part in bodyParts)
        {
            if (_skillsByBodyPart.ContainsKey(part))
            {
                continue;
            }

            _skillsByBodyPart.Add(part, new List<SkillTemplate>());

            foreach (var ability in _allSkillTemplates.Values.Where(skill =>
                skill.RequiresBodyPart.Trim().Equals(part, StringComparison.OrdinalIgnoreCase)))
            {
                _skillsByBodyPart[part].Add(ability);
            }
        }
    }

    public static List<SkillTemplate> GetAllSkillsWithRequiredBackground(CharacterBackground background)
    {
        return new List<SkillTemplate>(_skillsByBackgroundRequirement[background.Name]);
    }

    public static List<SkillTemplate> GetStartingSkillsForBackground(CharacterBackground background)
    {
        if (!_backgroundStartingSkills.ContainsKey(background.Name))
        {
            return null;
        }

        return new List<SkillTemplate>(_backgroundStartingSkills[background.Name]);
    }

    public static SkillTemplate GetSkillByName(string name)
    {
        return _allSkillTemplates[name.ToLower()];
    }

    public static List<SkillTemplate> GetSkillsByBodyPart(string partName)
    {
        if (_skillsByBodyPart == null)
        {
            return null;
        }

        return _skillsByBodyPart.ContainsKey(partName) ? _skillsByBodyPart[partName] : null;
    }

    public static Skill CreateSkill(SkillTemplate template, Entity owner)
    {
        var skills = new Dictionary<string, Func<SkillTemplate, Entity, Skill>>
        {
            {"scavenger", (skillTemplate, skillOwner) => new Scavenger(skillTemplate, skillOwner)},
            {"axe mastery", (skillTemplate, skillOwner) => new AxeMastery(skillTemplate, skillOwner)},
            {"cat-like reflexes", (skillTemplate, skillOwner) => new CatlikeReflexes(skillTemplate, skillOwner)},
            {"steady hands", (skillTemplate, skillOwner) => new SteadyHands(skillTemplate, skillOwner)},
            {"run and gun", (skillTemplate, skillOwner) => new RunAndGun(skillTemplate, skillOwner)},
            {"backswing", (skillTemplate, skillOwner) => new Backswing(skillTemplate, skillOwner)},
            {"diehard", (skillTemplate, skillOwner) => new Diehard(skillTemplate, skillOwner)},
            {"toughness", (skillTemplate, skillOwner) => new Toughness(skillTemplate, skillOwner)},
            {"hardened", (skillTemplate, skillOwner) => new Hardened(skillTemplate, skillOwner)}
        };

        if (template == null)
        {
            return null;
        }

        if (!skills.ContainsKey(template.Name.ToLower()))
        {
            Debug.Log($"failed to create skill: {template.Name}!");
            return null;
        }

        return skills[template.Name.ToLower()].Invoke(template, owner);
    }
}
