using System;
using UnityEngine;
using UnityEngine.UI;

public class TakeAllButton : MonoBehaviour
{
    //<Summary>
    // Places all items in player inventory and removes from tile
    //</Summary>
    public void Pressed()
    {
        var player = GameManager.Instance.Player;
        var selectedTile = player.CurrentTile;

        foreach (var itemButton in DroppedItemPopup.Instance.Buttons.Values)
        {
            Guid itemId;
            Guid.TryParse(itemButton.transform.GetComponentsInChildren<Text>(true)[2].text, out itemId);

            var item = WorldData.Instance.Items[itemId];

            player.Inventory.Add(itemId, item);

            selectedTile.PresentItems.Remove(item);
        }

        DroppedItemPopup.Instance.Hide();
    }
}
