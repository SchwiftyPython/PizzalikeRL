﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    private static Dictionary<string, Sprite> _abilityIcons;

    private Dictionary<KeyCode, GameObject> _abilityMap;

    public GameObject AbilityButton1;
    public GameObject AbilityButton2;
    public GameObject AbilityButton3;
    public GameObject AbilityButton4;
    public GameObject AbilityButton5;
    public GameObject AbilityButton6;
    public GameObject AbilityButton7;
    public GameObject AbilityButton8;
    public GameObject AbilityButton9;
    public GameObject AbilityButton0;

    public Sprite BashIcon;
    public Sprite KnockBackIcon;
    public Sprite StabIcon;
    public Sprite DivineAidIcon;
    public Sprite SpinWebIcon;

    public static AbilityManager Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _abilityIcons = new Dictionary<string, Sprite>
        {
            {"bash", BashIcon},
            {"knockback", KnockBackIcon},
            {"stab", StabIcon},
            {"divine aid", DivineAidIcon},
            {"spin web", SpinWebIcon}
        };

        if (_abilityMap == null)
        {
            PrepareAbilityMap();
        }
    }

    private static Sprite GetIconForAbility(Ability ability)
    {
        if (ability == null)
        {
            return null;
        }

        return !_abilityIcons.ContainsKey(ability.Name) ? null : _abilityIcons[ability.Name];
    }

    public static void AssignAbilityToButton(Ability ability, GameObject buttonParent) 
    {
        var buttonScript = buttonParent.GetComponent<Button>().GetComponent<UseAbilityButton>();

        var icon = GetIconForAbility(ability);

        buttonScript.AssignAbility(ability, icon);
    }

    public static void AssignAbilityToButton(Ability ability, Button button)
    {
        var buttonScript = button.GetComponent<UseAbilityButton>();

        var icon = GetIconForAbility(ability);

        buttonScript.AssignAbility(ability, icon);
    }

    public void PrepareAbilityMap()
    {
        _abilityMap = new Dictionary<KeyCode, GameObject>
        {
            { KeyCode.Alpha1, AbilityButton1 },
            { KeyCode.Alpha2, AbilityButton2 },
            { KeyCode.Alpha3, AbilityButton3 },
            { KeyCode.Alpha4, AbilityButton4 },
            { KeyCode.Alpha5, AbilityButton5 },
            { KeyCode.Alpha6, AbilityButton6 },
            { KeyCode.Alpha7, AbilityButton7 },
            { KeyCode.Alpha8, AbilityButton8 },
            { KeyCode.Alpha9, AbilityButton9 },
            { KeyCode.Alpha0, AbilityButton0 }
        };

        EventMediator.Instance.Broadcast(GlobalHelper.LoadAbilityBarEventName, this, _abilityMap);
    }

    public Dictionary<KeyCode, GameObject> GetAbilityMap()
    {
        return _abilityMap;
    }

    public static void InstantiateAbilityPrefab(Tile target, GameObject prefab)
    {
        target.AbilityTexture = Instantiate(prefab,
            new Vector2(target.Y, target.X), Quaternion.identity);
    }
}
