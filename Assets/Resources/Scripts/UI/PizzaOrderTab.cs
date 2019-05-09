using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class PizzaOrderTab : MonoBehaviour
{
    public void DisplayOrderDetails()
    {
        var buttonText = transform.GetComponentInChildren<TextMeshProUGUI>().text.Split();

        var orderIndex = Convert.ToInt32(buttonText.Last()) - 1;

        PizzaOrderJournalAreaView.Instance.DisplayOrderDetails(orderIndex);
    }
}
