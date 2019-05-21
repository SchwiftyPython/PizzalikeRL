using TMPro;
using UnityEngine;

public class PizzaOrderButton : MonoBehaviour
{
    public void DisplayOrderDetails()
    {
       var customerName = transform.GetComponentInChildren<TextMeshProUGUI>().text;

       PizzaOrderJournalWindow.Instance.DisplayOrderDetails(customerName);
    }
}
