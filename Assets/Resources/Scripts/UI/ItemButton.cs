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
            Guid.TryParse(transform.GetComponentsInChildren<TextMeshProUGUI>(true)[2].text, out var itemToEquipId);

            var player = GameManager.Instance.Player;

            var item = player.Inventory[itemToEquipId];

            EventMediator.Instance.Broadcast(GlobalHelper.ItemSelectedEventName, this, item);
        }
    }
}
