using System;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartButton : MonoBehaviour
{
    public void DisplayAvailableEquipmentForSelectedBodyPart()
    {
        var bodyPartIdClicked = Guid.Parse(transform.GetComponentsInChildren<Text>(true)[3].text);
        var bodyPart = GameManager.Instance.Player.Body[bodyPartIdClicked];

        FilteredInventoryWindowPopUp.Instance.DisplayAvailableEquipmentForSelectedBodyPart(bodyPart);
    }
}
