using System.Collections.Generic;

public class Web : Prop
{
    //todo maybe rethink this as prop and make some ability texture thing
    //todo plays like shit if they're on headstone or something

    public Web() : base(WorldData.Instance.SpiderWebPrefab)
    {
//        EventsTriggeredBy = new List<string>
//        {
//            GlobalHelper.EntityEnteredTileEventName
//        };
    }

    public override void Trigger(string eventName, object parameter)
    {
//        if (eventName.Equals(GlobalHelper.EntityEnteredTileEventName))
//        {
//            if (!(parameter is Entity targetEntity))
//            {
//                return;
//            }
//
//            targetEntity.ApplyEffect(new Immobilize(7));
//        }
    }
}
