using UnityEngine;

public class KillPlayerButton : MonoBehaviour
{
    public void OnClick()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.KillPlayerEventName, this);
    }
}
