using UnityEngine;

public class EquipmentSlotButton : MonoBehaviour
{
    public Item ItemToEquip;
    public Entity.EquipmentSlot EquipmentSlot;

    public void OnClick()
    {
        var player = GameManager.Instance.Player;
        
        player.EquipItem(ItemToEquip, EquipmentSlot);

        EventMediator.Instance.Broadcast(GlobalHelper.EquipmentSlotSelectedEventName, this);
    }
}
