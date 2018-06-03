using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public void ButtonPressed()
    {
        if (FilteredInventoryWindowPopUp.Instance.FilteredInventoryWindow.activeSelf)
        {
            Guid itemToEquipId;
            Guid.TryParse(transform.GetComponentsInChildren<Text>(true)[2].text, out itemToEquipId);

            var player = GameManager.Instance.Player;

            var itemToEquip = player.Inventory[itemToEquipId];

            player.EquipItem(itemToEquip, FilteredInventoryWindowPopUp.Instance.BodyPartFilter);

            FilteredInventoryWindowPopUp.Instance.Hide();
        }
        else
        {
            //todo bring up context menu to equip, look at, or drop item
        }
    }
}
