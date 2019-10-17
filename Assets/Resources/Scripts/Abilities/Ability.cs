using System;

public class Ability : ISubscriber
{
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

    public Ability(AbilityTemplate template, Entity owner)
    {
        Name = template.Name;
        Attribute = template.Attribute;
        AttributePrereq = template.AttributePrereq;
        RequiresBackground = template.RequiresBackground;
        Description = template.Description;
        RequiresBodyPart = template.RequiresBodyPart;
        RequiresProperty = template.RequiresProperty;
        Dice = template.Dice;
        Cooldown = template.Cooldown;
        Effect = template.Effect;
        Range = template.Range;
        StartingAbility = template.StartingAbility;
        RemainingCooldownTurns = 0;
        Owner = owner;

        TargetType = (AbilityTarget) Enum.Parse(typeof(AbilityTarget), template.Target.Replace(" ", ""), true);
    }

    public virtual void Use()
    {
        if (Cooldown > 0)
        {
            //todo disable ability
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
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
                //todo enable ability
                EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);
            }
        }
    }
}
