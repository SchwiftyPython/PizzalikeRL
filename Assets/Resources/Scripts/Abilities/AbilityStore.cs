using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AbilityStore : MonoBehaviour,ISubscriber
{
    private static Dictionary<string, AbilityTemplate> _allAbilities;
    private static Dictionary<DamageType, List<AbilityTemplate>> _abilitiesByDamageType;
    private static Dictionary<string, List<AbilityTemplate>> _abilitiesByBackgroundRequirement;
    private static Dictionary<string, List<AbilityTemplate>> _backgroundStartingAbilities;    
    private static Dictionary<string, List<AbilityTemplate>> _abilitiesByBodyPart;

    private static List<string> _religiousStartingAbilityNames;
    private static List<string> _militaryStartingAbilityNames;
    private static List<string> _streetRatStartingAbilityNames;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _allAbilities = AbilitiesLoader.GetAllAbilities();

        PopulateAbilityDictionaries();
    }
    
    private static void PopulateAbilityDictionaries()
    {
        PopulateAbilitiesByDamageType();
        PopulateAbilitiesByRequiredBackground();
        PopulateStartingAbilitiesForBackgrounds();
        PopulateAbilitiesByBodyPart();
    }

    private static void PopulateAbilitiesByDamageType()
    {
        _abilitiesByDamageType = new Dictionary<DamageType, List<AbilityTemplate>>();

        var damageTypes = Enum.GetValues(typeof(DamageType));

        for (var i = 0; i < damageTypes.Length; i++)
        {
            var damageType = (DamageType)damageTypes.GetValue(i);

            if (_abilitiesByDamageType.ContainsKey(damageType))
            {
                continue;
            }

            _abilitiesByDamageType.Add(damageType, new List<AbilityTemplate>());

            foreach (var ability in _allAbilities.Values.Where(ability =>
                ability.RequiresProperty.Trim().Equals(damageType.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                _abilitiesByDamageType[damageType].Add(ability);
            }
        }
    }

    private static void PopulateAbilitiesByRequiredBackground()
    {
        _abilitiesByBackgroundRequirement = new Dictionary<string, List<AbilityTemplate>>();

        var backgrounds = CharacterBackgroundLoader.GetCharacterBackgroundTypes();

        foreach (var background in backgrounds)
        {
            if (_abilitiesByBackgroundRequirement.ContainsKey(background))
            {
                continue;
            }

            _abilitiesByBackgroundRequirement.Add(background, new List<AbilityTemplate>());
            
            foreach (var ability in _allAbilities.Values.Where(ability =>
                ability.RequiresBackground.Trim().Equals(background, StringComparison.OrdinalIgnoreCase)))
            {
                _abilitiesByBackgroundRequirement[background].Add(ability);
            }
        }

    }

    private static void PopulateStartingAbilitiesForBackgrounds()
    {
        _backgroundStartingAbilities = new Dictionary<string, List<AbilityTemplate>>
        {
            {"religious", new List<AbilityTemplate>()},
            {"military", new List<AbilityTemplate>()},
            {"street rat", new List<AbilityTemplate>()}
        };

        //populate religious background
        _religiousStartingAbilityNames = new List<string>
        {
            "divine aid",
            "intimidate",
            "meditate"
        };

        _militaryStartingAbilityNames = new List<string>
        {
            "dismember"
        };

        _streetRatStartingAbilityNames = new List<string>
        {
            "bandage wounds"
        };

        foreach (var abilityTemplate in _allAbilities.Values)
        {
            if (_religiousStartingAbilityNames.Contains(abilityTemplate.Name.ToLower()))
            {
                _backgroundStartingAbilities["religious"].Add(abilityTemplate);
            }
            if (_militaryStartingAbilityNames.Contains(abilityTemplate.Name.ToLower()))
            {
                _backgroundStartingAbilities["military"].Add(abilityTemplate);
            }
            if (_streetRatStartingAbilityNames.Contains(abilityTemplate.Name.ToLower()))
            {
                _backgroundStartingAbilities["street rat"].Add(abilityTemplate);
            }
        }
    }

    private static void PopulateAbilitiesByBodyPart()
    {
        _abilitiesByBodyPart = new Dictionary<string, List<AbilityTemplate>>();

        var bodyParts = BodyPartLoader.BodyPartNames;

        foreach (var part in bodyParts)
        {
            if (_abilitiesByBodyPart.ContainsKey(part))
            {
                continue;
            }

            _abilitiesByBodyPart.Add(part, new List<AbilityTemplate>());

            foreach (var ability in _allAbilities.Values.Where(ability =>
                ability.RequiresBodyPart.Trim().Equals(part, StringComparison.OrdinalIgnoreCase)))
            {
                _abilitiesByBodyPart[part].Add(ability);
            }
        }
    }

    public static List<AbilityTemplate> GetAllAbilitiesWithRequiredBackground(CharacterBackground background)
    {
        return new List<AbilityTemplate>(_abilitiesByBackgroundRequirement[background.Name]);
    }

    public static List<AbilityTemplate> GetStartingAbilitiesForBackground(CharacterBackground background)
    {
        if (!_backgroundStartingAbilities.ContainsKey(background.Name))
        {
            return null;
        }

        return new List<AbilityTemplate>(_backgroundStartingAbilities[background.Name]);
    }

    public static List<AbilityTemplate> GetAbilitiesByDamageType(DamageType dt)
    {
        return new List<AbilityTemplate>(_abilitiesByDamageType[dt]);
    }

    public static AbilityTemplate GetAbilityByName(string name)
    {
        return _allAbilities[name.ToLower()];
    }

    public static List<AbilityTemplate> GetAbilitiesByBodyPart(string partName)
    {
        if (_abilitiesByBodyPart == null)
        {
            return null;
        }

        return _abilitiesByBodyPart.ContainsKey(partName) ? _abilitiesByBodyPart[partName] : null;
    }

    public static Dictionary<DamageType, List<AbilityTemplate>> GetAllDamageTypeAbilities()
    {
        return new Dictionary<DamageType, List<AbilityTemplate>>(_abilitiesByDamageType);
    }

    public static Ability CreateAbility(AbilityTemplate template, Entity owner)
    {
        Dictionary<string, Func<AbilityTemplate, Entity, Ability>> abilities = new Dictionary<string, Func<AbilityTemplate, Entity, Ability>>
        {
            {"divine aid", (abilityTemplate, abilityOwner) => new Heal(abilityTemplate, abilityOwner)},
            {"bash", (abilityTemplate, abilityOwner) => new Bash(abilityTemplate, abilityOwner)},
            {"knockback", (abilityTemplate, abilityOwner) => new KnockBack(abilityTemplate, abilityOwner)},
            {"stab", (abilityTemplate, abilityOwner) => new Stab(abilityTemplate, abilityOwner)},
            {"spin web", (abilityTemplate, abilityOwner) => new SpinWeb(abilityTemplate, abilityOwner)},
            {"intimidate", (abilityTemplate, abilityOwner) => new Intimidate(abilityTemplate, abilityOwner)},
            {"dismember", (abilityTemplate, abilityOwner) => new Dismember(abilityTemplate, abilityOwner)},
            {"meditate", (abilityTemplate, abilityOwner) => new Meditate(abilityTemplate, abilityOwner)},
            {"bandage wounds", (abilityTemplate, abilityOwner) => new BandageWounds(abilityTemplate, abilityOwner)}
        };

        if (template == null)
        {
            return null;
        }

        if (!abilities.ContainsKey(template.Name.ToLower()))
        {
            Debug.Log($"failed to create ability: {template.Name}!");
            return null;
        }

        return abilities[template.Name.ToLower()].Invoke(template, owner);
    }

    public static Ability ChooseRandomFreeAbility(Entity entity)
    {
        var abilities = new List<AbilityTemplate>();

        foreach (var bodyPart in entity.Body.Values)
        {
            var partAbilities = GetAbilitiesByBodyPart(bodyPart.Name);

            if (partAbilities == null)
            {
                continue;
            }

            foreach (var ability in partAbilities)
            {
                if (ability.StartingAbility)
                {
                    continue;
                }

                abilities.Add(ability);
            }
        }

        var backgroundAbilities = GetAllAbilitiesWithRequiredBackground(entity.Fluff.BackgroundType);

        foreach (var ability in backgroundAbilities.ToArray().Where(ability => ability.StartingAbility))
        {
            backgroundAbilities.Remove(ability);
        }

        abilities.AddRange(backgroundAbilities);

        var damageTypeAbilities = GetAllDamageTypeAbilities();

        foreach (var damageType in damageTypeAbilities)
        {
            var abilityList = new List<AbilityTemplate>(damageType.Value);

            foreach (var ability in abilityList.ToArray().Where(ability => ability.StartingAbility))
            {
                abilityList.Remove(ability);
            }

            abilities.AddRange(abilityList);
        }

        var template = abilities[Random.Range(0, abilities.Count)];

        return CreateAbility(template, entity);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        throw new System.NotImplementedException();
    }
}
