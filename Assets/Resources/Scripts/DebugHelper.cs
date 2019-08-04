using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

    //<Summary>
    // Returns a random enum value of type T
    //</Summary>
    public T GetRandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(Random.Range(0, values.Length));
    }
}
