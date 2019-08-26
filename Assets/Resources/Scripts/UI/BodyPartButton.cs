using System;
using TMPro;
using UnityEngine;

public class BodyPartButton : MonoBehaviour
{
    public void DisplayAvailableEquipmentForSelectedBodyPart()
    {
        var slotClicked = Enum.Parse(typeof(Entity.EquipmentSlot),
            transform.GetComponentsInChildren<TextMeshProUGUI>(true)[3].text.TrimStart('-').Replace(" ", ""));

        FilteredInventoryWindowPopUp.Instance.DisplayAvailableEquipmentForSelectedEquipmentSlot(
            (Entity.EquipmentSlot) slotClicked);
    }
}
