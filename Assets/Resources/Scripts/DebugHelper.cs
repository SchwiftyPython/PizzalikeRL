using System;
using UnityEngine;

//<Summary>
// A class of functions to aid in debugging
//</Summary>
public class DebugHelper : MonoBehaviour, ISubscriber
{
    public static DebugHelper Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.KillPlayerEventName, this);
    }

    //<Summary>
    // Sets player's current hp to zero
    //</Summary>
    public void KillPlayer()
    {
        GameManager.Instance.Player.CurrentHp = 0;
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.KillPlayerEventName, StringComparison.OrdinalIgnoreCase))
        {
            KillPlayer();
        }
    }
}
