using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour, ISubscriber
{
    private const int MaxMessagesOnScreen = 45;

    private Queue<GameObject> _messagesOnScreen;

    private static Messenger _instance;

    public GameObject Message;

    public RectTransform MessageParent;

    public ScrollRect MessageScrollRect;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        _messagesOnScreen = new Queue<GameObject>();

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.SendMessageToConsoleEventName, this);
    }

    //todo store all messages in some log that can be accessed via menu
    private void ClearExcessMessages()
    {
        while (_messagesOnScreen.Count >= MaxMessagesOnScreen)
        {
            var messageToClear = _messagesOnScreen.Dequeue();
            Destroy(messageToClear);
        }
    }

    private void ScrollToBottom()
    {
        MessageScrollRect.verticalScrollbar.direction = Scrollbar.Direction.BottomToTop;
        MessageScrollRect.normalizedPosition = new Vector2(0, 0.0f);
    }

    public static Messenger GetInstance()
    {
        return _instance;
    }

    public void CreateMessage(string myMessage)
    {
        ClearExcessMessages();

        var cm = Instantiate(Message, Message.transform.position, Quaternion.identity);

        cm.transform.SetParent(MessageParent);

        cm.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        cm.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myMessage;

        _messagesOnScreen.Enqueue(cm);

        ScrollToBottom();
    }

    public void CreateMessage(string myMessage, Color messageColor)
    {
        ClearExcessMessages();

        var cm = Instantiate(Message, Message.transform.position, Quaternion.identity);

        cm.transform.SetParent(MessageParent);

        cm.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        cm.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myMessage;

        cm.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = messageColor;

        _messagesOnScreen.Enqueue(cm);

        ScrollToBottom();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.SendMessageToConsoleEventName))
        {
            var message = parameter as string;

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            CreateMessage(message);
        }
    }
}
