using System;
using System.Xml.Serialization;

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
    
    public AbilityTarget Target; 
    
    public string Effect; //todo make enum
    
    public int Range;
    
    public bool StartingAbility;

    public int RemainingCooldownTurns;

    public Ability(AbilityTemplate template)
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

        Target = (AbilityTarget) Enum.Parse(typeof(AbilityTarget), template.Target, true);

        RemainingCooldownTurns = 0;
    }
}
