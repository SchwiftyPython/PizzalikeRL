using UnityEngine;

public class Wait : Goal
{
    private const int MaxTurns = 9;
    private int _turnsLeft;

    public Wait()
    {
        _turnsLeft = Random.Range(1, MaxTurns);
    }

    public override bool Finished()
    {
        return _turnsLeft <= 0;
    }

    public override void Create()
    {
        Debug.Log($"{Self} is going to wait {_turnsLeft} turns.");
    }

    public override void TakeAction()
    {
        _turnsLeft--;
        if (_turnsLeft <= 0)
        {
            Pop();
        }
    }
}
