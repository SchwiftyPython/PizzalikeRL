using System;
using UnityEngine.UI;

public class Ability : ISubscriber
{
    private UseAbilityButton _buttonScript;

    private bool _enabled;

    public string Name;
    
    public string Attribute; //todo make enum
    
    public string AttributePrereq;
    
    public string RequiresBackground;
    
    public string Description;
    
    public string RequiresBodyPart;
    
    public string RequiresProperty; //todo make enum
    
    public string Dice;
    
    public int Cooldown;
    
    public AbilityTarget TargetType; 
    
    public string Effect; //todo make enum
    
    public int Range;
    
    public bool StartingAbility;

    public int RemainingCooldownTurns;

    public Entity Owner;

    public Button AbilityButton;

    public Ability(AbilityTemplate template, Entity owner)
    {
        Name = template.Name;
        Attribute = template.Attribute;
        AttributePrereq = template.AttributePrereq;
        RequiresBackground = template.RequiresBackground;
        Description = template.Description;
        RequiresBodyPart = template.RequiresBodyPart;
        RequiresProperty = template.RequiresProperty.Trim();
        Dice = template.Dice;
        Cooldown = template.Cooldown;
        Effect = template.Effect;
        Range = template.Range;
        StartingAbility = template.StartingAbility;
        RemainingCooldownTurns = 0;
        Owner = owner;

        TargetType = (AbilityTarget) Enum.Parse(typeof(AbilityTarget), template.Target.Replace(" ", ""), true);

        if (!string.IsNullOrEmpty(RequiresProperty) && Owner.IsPlayer())
        {
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemEquippedEventName, this);
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemUnequippedEventName, this);
        }
    }

    public virtual void Use()
    {
        
    }

    public void Enable()
    {
        _enabled = true;
    }

    public void Disable()
    {
        _enabled = false;
    }

    public bool IsEnabled()
    {
        return _enabled;
    }

    public void AssignAbilityToButton(Button abilityButton)
    {
        AbilityButton = abilityButton;
        _buttonScript = abilityButton.GetComponent<Button>().GetComponent<UseAbilityButton>();
    }

    public void CheckEquippedItemsForRequiredProperty()
    {
        Disable();

        foreach (var equippedItem in GameManager.Instance.Player.Equipped.Values)
        {
            if (equippedItem != null && equippedItem.Properties.Contains(RequiresProperty))
            {
                Enable();
                break;
            }
        }
    }

    public virtual void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (RemainingCooldownTurns > 0)
            {
                RemainingCooldownTurns--;
            }
            else
            {
                Enable();
                EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);
            }
        }
        else if (eventName == GlobalHelper.ItemEquippedEventName || eventName == GlobalHelper.ItemUnequippedEventName)
        {
            CheckEquippedItemsForRequiredProperty();
        }
    }

    public void UseAbilitySuccess()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.ActionTakenEventName, this);

        if (Cooldown > 0)
        {
            if (_buttonScript != null)
            {
                _buttonScript.DisableButton();
                EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, _buttonScript);

            }

            Disable();
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
        }
    }
}
