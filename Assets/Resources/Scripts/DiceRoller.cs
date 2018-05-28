using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public static DiceRoller Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public int RollDice(Dice dice)
    {
        var sum = 0;
        for (var i = 0; i < dice.NumDice; i++)
        {
            sum += Random.Range(1, dice.NumSides + 1);
        }
        return sum;
    }
}
