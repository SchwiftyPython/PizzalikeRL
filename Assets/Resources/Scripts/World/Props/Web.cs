using System.Collections.Generic;

public class Web : Prop
{
   public Web() : base(WorldData.Instance.SpiderWeb)
   {
       EventsTriggeredBy = new List<string>
       {
           GlobalHelper.EntityEnteredTileEventName
       };
    }

   public override void Trigger(string eventName, object parameter)
   {
       if (eventName.Equals(GlobalHelper.EntityEnteredTileEventName))
       {
           if (!(parameter is Entity targetEntity))
           {
               return;
           }

           targetEntity.ApplyEffect(new Immobilize(7));
       }
   }
}
