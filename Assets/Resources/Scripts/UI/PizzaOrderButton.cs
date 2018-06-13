using UnityEngine;
using UnityEngine.UI;

public class PizzaOrderButton : MonoBehaviour
{
    public void DisplayOrderDetails()
    {
       var customerName = transform.GetComponentInChildren<Text>().text;

       PizzaOrderJournalWindow.Instance.DisplayOrderDetails(customerName);
    }
}
