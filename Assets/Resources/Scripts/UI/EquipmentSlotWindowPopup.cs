using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo might come back to this, but for now we'll just equip
public class EquipmentSlotWindowPopup : MonoBehaviour, ISubscriber
{
    public GameObject ButtonPrefab;
    public GameObject TitleBar;
    public GameObject ActionBar;

    private IDictionary<char, GameObject> _buttons;

    private Transform _buttonParent;

    private Item _selectedItem;

    private void Start()
    {
        //EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemSelectedEventName, this);

        Hide();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        TitleBar.SetActive(false);
        ActionBar.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        TitleBar.SetActive(true);
        ActionBar.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.ItemSelectedEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void OnItemSelected(Item item)
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (!eventName.Equals(GlobalHelper.ItemSelectedEventName))
        {
            return;
        }

        if (!(parameter is Item item))
        {
            return;
        }

        OnItemSelected(item);
    }
}
