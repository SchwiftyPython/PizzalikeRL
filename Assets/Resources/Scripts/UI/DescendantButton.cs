using System;
using UnityEngine;
using UnityEngine.UI;

public class DescendantButton : MonoBehaviour
{
    public void DisplayDescendantDetails()
    {
        var id = Guid.Parse(transform.GetComponentInChildren<Text>(true).text);

        DelivererSelectWindow.Instance.DisplayDescendantDetails(id);
    }
}
