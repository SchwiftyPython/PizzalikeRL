﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        //todo need dictionary

        if (template == null)
        {
            return null;
        }

        if (template.Effect.Contains("heal"))
        {
            return new Heal(template, owner);
        }

        if (template.Name.Equals("bash", StringComparison.OrdinalIgnoreCase))
        {
            return new Bash(template, owner);
        }

        if (template.Name.Equals("knockback", StringComparison.OrdinalIgnoreCase))
        {
            return new KnockBack(template, owner);
        }

        if (template.Name.Equals("stab", StringComparison.OrdinalIgnoreCase))
        {
            return new Stab(template, owner);
        }

        if (template.Name.Equals("spin web", StringComparison.OrdinalIgnoreCase))
        {
            return new SpinWeb(template, owner);
        }

        Debug.Log($"{template.Name} failed to create!");
        return null;
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

        var backgroundAbilities = GetAbilitiesByBackground(entity.Fluff.BackgroundType);

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
