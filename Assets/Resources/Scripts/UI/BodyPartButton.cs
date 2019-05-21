using System;
using TMPro;
using UnityEngine;

public class BodyPartButton : MonoBehaviour
{
    public void DisplayAvailableEquipmentForSelectedBodyPart()
    {
        var bodyPartIdClicked = Guid.Parse(transform.GetComponentsInChildren<TextMeshProUGUI>(true)[3].text);
        var bodyPart = GameManager.Instance.Player.Body[bodyPartIdClicked];

        FilteredInventoryWindowPopUp.Instance.DisplayAvailableEquipmentForSelectedBodyPart(bodyPart);
    }
}
