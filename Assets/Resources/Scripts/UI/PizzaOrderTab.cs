using System;
using TMPro;
using UnityEngine;

public class PizzaOrderTab : MonoBehaviour
{
    public void DisplayOrderDetails()
    {
        var orderIndex = Convert.ToInt32(transform.GetComponentInChildren<TextMeshProUGUI>().text);

        PizzaOrderJournalAreaView.Instance.DisplayOrderDetails(orderIndex);
    }
}
