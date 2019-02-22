using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItemButton : MonoBehaviour
{
    //<Summary>
    // Places selected item in player inventory and removes from tile
    //</Summary>
    public void Pressed()
    {
        //todo context menu to look at or take item

        //Just take item for now 

        Guid itemId;
        Guid.TryParse(transform.GetComponentsInChildren<Text>(true)[2].text, out itemId);

        var player = GameManager.Instance.Player;
        var selectedTile = player.CurrentTile;

        var item = WorldData.Instance.Items[itemId];

        player.Inventory.Add(itemId, item);

        selectedTile.PresentItems.Remove(item);

        if (selectedTile.PresentItems.Count <= 0)
        {
            DroppedItemPopup.Instance.Hide();
        }
        else
        {
            //todo refresh DroppedItemPoup
        }
    }
}
