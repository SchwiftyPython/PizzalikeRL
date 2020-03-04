using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInfoPopup : MonoBehaviour, ISubscriber
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Modifiers;
    public TextMeshProUGUI Rarity;

    public void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.InspectItemEventName, this);

        if (gameObject.activeSelf)
        {
            Hide();
        }
    }

    public void Hide()
    {
        throw new System.NotImplementedException();
    }

    public void Show()
    {
        throw new System.NotImplementedException();
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.InspectItemEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (!eventName.Equals(GlobalHelper.InspectItemEventName) || GameManager.Instance.AnyActiveWindows() ||
            parameter == null)
        {
            return;
        }

        if (!(parameter is Item item))
        {
            return;
        }

        Show();
    }
}
