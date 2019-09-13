using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityStore : MonoBehaviour,ISubscriber
{
    private static Dictionary<string, AbilityTemplate> _allAbilities;
    private static Dictionary<DamageType, List<AbilityTemplate>> _abilitiesByDamageType;
    private static Dictionary<string, List<AbilityTemplate>> _abilitiesByBackground;
    private static Dictionary<string, List<AbilityTemplate>> _abilitiesByBodyPart;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _allAbilities = AbilitiesLoader.GetAllAbilities();

        PopulateAbilityDictionaries();
    }
    
    private static void PopulateAbilityDictionaries()
    {
        PopulateAbilitiesByDamageType();
        PopulateAbilitiesByBackground();
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

    private static void PopulateAbilitiesByBackground()
    {
        _abilitiesByBackground = new Dictionary<string, List<AbilityTemplate>>();

        var backgrounds = CharacterBackgroundLoader.GetCharacterBackgroundTypes();

        foreach (var background in backgrounds)
        {
            if (_abilitiesByBackground.ContainsKey(background))
            {
                continue;
            }

            _abilitiesByBackground.Add(background, new List<AbilityTemplate>());
            
            foreach (var ability in _allAbilities.Values.Where(ability =>
                ability.RequiresBackground.Trim().Equals(background, StringComparison.OrdinalIgnoreCase)))
            {
                _abilitiesByBackground[background].Add(ability);
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

    public static List<AbilityTemplate> GetAbilitiesByBackground(CharacterBackground background)
    {
        return new List<AbilityTemplate>(_abilitiesByBackground[background.Name]);
    }

    public static List<AbilityTemplate> GetAbilitiesByDamageType(DamageType dt)
    {
        return _abilitiesByDamageType[dt];
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
        return _abilitiesByDamageType;
    }

    public static Ability CreateAbility(AbilityTemplate template, Entity owner)
    {
        if (template.Effect.Contains("heal"))
        {
            return new Heal(template, owner);
        }

        return null;
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        throw new System.NotImplementedException();
    }
}
