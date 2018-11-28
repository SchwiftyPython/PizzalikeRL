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

            //Debug.Log(transform.GetComponentsInChildren<Text>(true)[2].text + "    " + itemToEquipId);

            var player = GameManager.Instance.Player;

            var itemToEquip = player.Inventory[itemToEquipId];

            player.EquipItem(itemToEquip, FilteredInventoryWindowPopUp.Instance.BodyPartFilterId);

            FilteredInventoryWindowPopUp.Instance.Hide();
        }
        else
        {
            //todo bring up context menu to equip, look at, or drop item
        }
    }
}
