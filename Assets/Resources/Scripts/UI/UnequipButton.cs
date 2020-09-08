using UnityEngine;

public class UnequipButton : MonoBehaviour
{
    public void ButtonPressed()
    {
        var player = GameManager.Instance.Player;

        player.UnEquipEquipmentSlot(FilteredInventoryWindowPopUp.Instance.EquipmentSlotFilter);

        FilteredInventoryWindowPopUp.Instance.Hide();
    }
}
