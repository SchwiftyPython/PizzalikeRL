using UnityEngine;

public class UnequipButton : MonoBehaviour
{
    public void ButtonPressed()
    {
        var player = GameManager.Instance.Player;

        player.UnequipItem(FilteredInventoryWindowPopUp.Instance.BodyPartFilterId);

        FilteredInventoryWindowPopUp.Instance.Hide();
    }
}
