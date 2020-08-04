//Passive abilities.
//Players won't know the distinction aside from active vs passive abilities.
public class Skill : ISubscriber
{
    public string Name;

    public Entity Owner;

    public string Attribute; //todo make enum

    public string AttributePrereq;

    public string RequiresBackground;

    public string Description;

    public string RequiresBodyPart;

    public string RequiresProperty; //todo make enum

    public string Dice;

    public bool StartingSkill;

    public Skill(SkillTemplate template, Entity owner)
    {
        Name = template.Name;
        Owner = owner;
        Attribute = template.Attribute;
        AttributePrereq = template.AttributePrereq;
        RequiresBackground = template.RequiresBackground;
        Description = template.Description;
        RequiresBodyPart = template.RequiresBodyPart;
        RequiresProperty = template.RequiresProperty;
        Dice = template.Dice;
        StartingSkill = template.StartingSkill;
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        throw new System.NotImplementedException();
    }
}
