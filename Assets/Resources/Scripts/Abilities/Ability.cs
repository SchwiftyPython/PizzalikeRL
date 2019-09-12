using System;

public class Ability
{
    public string Name;
    
    public string Attribute; //todo make enum
    
    public string AttributePrereq;
    
    public string RequiresBackground;
    
    public string Description;
    
    public string RequiresBodyPart;
    
    public string RequiresProperty; //todo make enum
    
    public string Dice;
    
    public int Duration;
    
    public int Cooldown;
    
    public AbilityTarget TargetType; 
    
    public string Effect; //todo make enum
    
    public int Range;
    
    public bool StartingAbility;

    public int RemainingCooldownTurns;

    public Entity Owner;

    public object Target;

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
        Duration = template.Duration;
        Cooldown = template.Cooldown;
        Effect = template.Effect;
        StartingAbility = template.StartingAbility;
        RemainingCooldownTurns = 0;
        Owner = owner;

        TargetType = (AbilityTarget) Enum.Parse(typeof(AbilityTarget), template.Target.Replace(" ", ""), true);
    }

    public void Use()
    {
        switch (TargetType)
        {
            case AbilityTarget.Self:
                Target = Owner;

                if (Effect != null || Effect != string.Empty)
                {

                }

                break;
        }
    }
}
