using System;
using TMPro;
using UnityEngine;

public class DroppedItemButton : MonoBehaviour
{
    //<Summary>
    // Places selected item in player inventory and removes from tile
    //</Summary>
    public void Pressed()
    {
        OpenItemActionMenu();

        /*Guid.TryParse(transform.GetComponentsInChildren<TextMeshProUGUI>(true)[2].text, out var itemId);

        var player = GameManager.Instance.Player;
        var selectedTile = player.CurrentTile;

        var item = WorldData.Instance.Items[itemId];

        player.Inventory.Add(itemId, item);

        selectedTile.PresentItems.Remove(item);

        var message = $"Picked up {item.ItemType}"; //todo change to item name

        EventMediator.Instance.Broadcast(GlobalHelper.SendMessageToConsoleEventName, this, message);

        if (selectedTile.PresentItems.Count <= 0)
        {
            Destroy(item.WorldSprite);
        }
        
        EventMediator.Instance.Broadcast("GetItem", this, selectedTile);*/
    }

    private void OpenItemActionMenu()
    {
        Guid.TryParse(transform.GetComponentsInChildren<TextMeshProUGUI>(true)[2].text, out var itemId);

        var item = WorldData.Instance.Items[itemId];

        EventMediator.Instance.Broadcast(GlobalHelper.DroppedItemSelectedEventName, this, item);
    }
}
