using UnityEngine;

//<Summary>
// A class of functions to aid in debugging
//</Summary>
public class DebugHelper : MonoBehaviour
{
    public static DebugHelper Instance;

    private void Awake()
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
    }

    //<Summary>
    // Sets player's current hp to zero
    //</Summary>
    public void KillPlayer()
    {
        GameManager.Instance.Player.CurrentHp = 0;
    }
}
