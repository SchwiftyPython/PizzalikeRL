public class BuildingPrefab
{
    public char[,] Blueprint;
    public int Height;
    public int Width;

    public BuildingPrefab(char[,] blueprint)
    {
        Blueprint = blueprint;
        Width = blueprint.GetLength(0);
        Height = blueprint.GetLength(1);
    }

    public bool WillFitInLot(Lot lot)
    {
        return Height <= lot.Height && Width <= lot.Width;
    }
}
