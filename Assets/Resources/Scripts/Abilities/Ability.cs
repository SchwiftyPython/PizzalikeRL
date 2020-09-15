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

    public bool UsesConsumables;

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
        UsesConsumables = template.UsesConsumables;

        TargetType = (AbilityTarget) Enum.Parse(typeof(AbilityTarget), template.Target.Replace(" ", ""), true);

        if (!string.IsNullOrEmpty(RequiresProperty) && Owner.IsPlayer())
        {
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemEquippedEventName, this);
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemUnequippedEventName, this);
        }

        if (UsesConsumables)
        {
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.ConsumableUsedEventName, this);
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
        foreach (var equippedItem in GameManager.Instance.Player.Equipped.Values)
        {
            if (equippedItem != null && equippedItem.Properties.Contains(RequiresProperty))
            {
                Enable();
                return;
            }
        }
        Disable();
    }

    public void CheckConsumablesForRequiredProperty()
    {
        if (!UsesConsumables)
        {
            return;
        }

        foreach (var item in GameManager.Instance.Player.Inventory.Values)
        {
            if (item.EquipmentSlotType == EquipmentSlotType.Consumable && item.Properties.Contains(RequiresProperty))
            {
                Enable();
                return;
            }
        }
        Disable();
    }

    public virtual void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (!(parameter is Entity entity) || entity.Id != Owner.Id)
            {
                return;
            }

            if (RemainingCooldownTurns > 0)
            {
                RemainingCooldownTurns--;
            }
            else
            {
                if (!string.IsNullOrEmpty(RequiresProperty))
                {
                    CheckEquippedItemsForRequiredProperty();

                    if (UsesConsumables)
                    {
                        CheckConsumablesForRequiredProperty();
                    }
                }
                else
                {
                    Enable();
                }
                EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);
            }
        }
        else if (eventName == GlobalHelper.ItemEquippedEventName || eventName == GlobalHelper.ItemUnequippedEventName)
        {
            CheckEquippedItemsForRequiredProperty();
        }
        else if (eventName == GlobalHelper.ConsumableUsedEventName)
        {
            CheckConsumablesForRequiredProperty();
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
