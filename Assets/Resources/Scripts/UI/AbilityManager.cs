using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    private static Dictionary<string, Sprite> _abilityIcons;

    public static GameObject AbilityButton1;
    public static GameObject AbilityButton2;
    public static GameObject AbilityButton3;
    public static GameObject AbilityButton4;
    public static GameObject AbilityButton5;
    public static GameObject AbilityButton6;
    public static GameObject AbilityButton7;
    public static GameObject AbilityButton8;
    public static GameObject AbilityButton9;
    public static GameObject AbilityButton0;

    public static Sprite BashIcon;

    private void Start()
    {
        InputController.Instance.LoadStartingAbilitiesIntoAbilityBar();

        _abilityIcons = new Dictionary<string, Sprite>
        {
            {"bash", BashIcon}
        };
    }

    private static Sprite GetIconForAbility(Ability ability)
    {
        return !_abilityIcons.ContainsKey(ability.Name) ? null : _abilityIcons[ability.Name];
    }

    public static void AssignAbilityToButton(Ability ability, GameObject buttonParent)
    {
        var buttonScript = buttonParent.GetComponent<Button>().GetComponent<UseAbilityButton>();

        var icon = GetIconForAbility(ability);

        buttonScript.AssignAbility(ability, icon);
    }
}
