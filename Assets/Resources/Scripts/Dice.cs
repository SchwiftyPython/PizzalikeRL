public class Dice
{
    public int NumDice { get; set; }
    public int NumSides { get; set; }

    public Dice() { }

    public Dice(int numDice, int numSides)
    {
        NumDice = numDice;
        NumSides = numSides;
    }
}
