using System;
using TMPro;
using UnityEngine;

public class BodyPartButton : MonoBehaviour
{
    public void DisplayAvailableEquipmentForSelectedBodyPart()
    {
        //var slotName = transform.GetComponentsInChildren<TextMeshProUGUI>(true)[3].text.TrimStart('-').Replace(" ", "");
        var slotName = transform.GetComponentsInChildren<TextMeshProUGUI>(true)[3].text.TrimStart('-', ' ');

        if (string.IsNullOrEmpty(slotName))
        {
            return;
        }

        var trimmedSlotName = slotName.Replace(" ", "");

        if (!Enum.TryParse<Entity.EquipmentSlot>(trimmedSlotName, true, out var slotClicked))
        {
            slotClicked = GlobalHelper.GetEnumValueFromDescription<Entity.EquipmentSlot>(slotName);
        }

        FilteredInventoryWindowPopUp.Instance.DisplayAvailableEquipmentForSelectedEquipmentSlot(slotClicked);
    }
}
