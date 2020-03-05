using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActionWindow : MonoBehaviour, ISubscriber
{
    private Item _selectedItem;

    public GameObject GetButton;
    public GameObject EquipButton;
    public GameObject LookButton;
    public GameObject ReadButton;

    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemSelectedEventName, this);

        gameObject.SetActive(false);
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

    public void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnLookButtonClicked()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.InspectItemEventName, this, _selectedItem);
    }

    private void OnItemSelected(Item item)
    {
        _selectedItem = item;

        var pos = Input.mousePosition;

        //todo check if window is near edge of game area
        //todo possibly make window draggable so player can adjust if needed
        gameObject.transform.position = new Vector2(pos.x + 90f, pos.y + 80f);

        gameObject.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.ItemSelectedEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
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
