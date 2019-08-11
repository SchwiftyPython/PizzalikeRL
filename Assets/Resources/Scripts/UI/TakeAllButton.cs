using UnityEngine;

public class TakeAllButton : MonoBehaviour
{
    //<Summary>
    // Broadcasts TakeAll event
    //</Summary>
    public void Pressed()
    {
        EventMediator.Instance.Broadcast("TakeAll", this);
    }
}
