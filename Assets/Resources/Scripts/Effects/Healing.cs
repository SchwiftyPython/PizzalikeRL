using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Healing : Effect
{
    private int _amountToHealPerTurn;

    public Healing(int duration, int amount)
    {
        base._duration = duration;
        base._name = "healing";
        _amountToHealPerTurn = amount;
    }
}
