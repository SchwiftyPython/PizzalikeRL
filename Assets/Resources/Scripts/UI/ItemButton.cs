using System;
using TMPro;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public void ButtonPressed()
    {
        if (FilteredInventoryWindowPopUp.Instance.FilteredInventoryWindow.activeSelf)
        {
            Guid.TryParse(transform.GetComponentsInChildren<TextMeshProUGUI>(true)[2].text, out var itemToEquipId);

            var player = GameManager.Instance.Player;

            var itemToEquip = player.Inventory[itemToEquipId];

            player.EquipItem(itemToEquip, FilteredInventoryWindowPopUp.Instance.EquipmentSlotFilter);

            FilteredInventoryWindowPopUp.Instance.Hide();
        }
        else
        {
            //todo bring up context menu to equip, look at, or drop item
            //todo we can probably eliminate a look action if we load the description as part of the window and place action buttons 
            //todo on bottom or side or show on hover

            Guid.TryParse(transform.GetComponentsInChildren<TextMeshProUGUI>(true)[2].text, out var itemToEquipId);

            var player = GameManager.Instance.Player;

            var item = player.Inventory[itemToEquipId];

            EventMediator.Instance.Broadcast(GlobalHelper.ItemSelectedEventName, this, item);
        }
    }
}
