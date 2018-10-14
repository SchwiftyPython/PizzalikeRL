using System;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartButton : MonoBehaviour
{
    public void DisplayAvailableEquipmentForSelectedBodyPart()
    {
        var bodyPartTypeClicked = transform.GetComponentsInChildren<Text>()[0].text.Remove(0, 3);
        var bodyPart = GameManager.Instance.Player.Body[bodyPartTypeClicked];

        FilteredInventoryWindowPopUp.Instance.DisplayAvailableEquipmentForSelectedBodyPart(bodyPart);
    }
}
