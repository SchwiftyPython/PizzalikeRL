using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    public void Clicked()
    {
        EventMediator.Instance.Broadcast("Clicked", this);
    }
}
