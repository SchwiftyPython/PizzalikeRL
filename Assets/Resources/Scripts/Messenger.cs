using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour, ISubscriber
{
    private const int MaxMessagesOnScreen = 45;
    private const int MaxMessagesInCache = 90; //may not need this, but something to think about if _allMessages has massive amount

    private Queue<GameObject> _messagesOnScreen;

    private List<string> _allMessages;

    public static Messenger Instance { get; private set; }

    public GameObject Message;

    public RectTransform MessageParent;

    public ScrollRect MessageScrollRect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        _messagesOnScreen = new Queue<GameObject>();

        if (_allMessages == null || _allMessages.Count < 1)
        {
            _allMessages = new List<string>(GameManager.Instance.AllMessages);
        }

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.SendMessageToConsoleEventName, this);
    }

    //todo store all messages in some log that can be accessed via menu
    private void ClearExcessOnScreenMessages()
    {
        while (_messagesOnScreen.Count >= MaxMessagesOnScreen)
        {
            var messageToClear = _messagesOnScreen.Dequeue();
            Destroy(messageToClear);
        }
    }

    private void ClearAllOnScreenMessages()
    {
        foreach (var messageObject in _messagesOnScreen)
        {
            Destroy(messageObject);
        }

        _messagesOnScreen = new Queue<GameObject>();
    }

    private void ScrollToBottom()
    {
        MessageScrollRect.verticalScrollbar.direction = Scrollbar.Direction.BottomToTop;
        MessageScrollRect.normalizedPosition = new Vector2(0, 0.0f);
    }

    public static Messenger GetInstance()
    {
        return Instance;
    }

    public void CreateOnScreenMessage(string myMessage)
    {
        ClearExcessOnScreenMessages();

        var cm = Instantiate(Message, Message.transform.position, Quaternion.identity);

        cm.transform.SetParent(MessageParent);

        cm.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        cm.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myMessage;

        _messagesOnScreen.Enqueue(cm);

        ScrollToBottom();
    }

    public void CreateOnScreenMessage(string myMessage, Color messageColor)
    {
        ClearExcessOnScreenMessages();

        var cm = Instantiate(Message, Message.transform.position, Quaternion.identity);

        cm.transform.SetParent(MessageParent);

        cm.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        cm.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = myMessage;

        cm.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = messageColor;

        _messagesOnScreen.Enqueue(cm);

        ScrollToBottom();
    }

    public List<string> GetAllMessages()
    {
        return new List<string>(_allMessages);
    }

    public void LoadMessages(List<string> messages)
    {
        _allMessages = new List<string>(messages);
        
        GameManager.Instance.AllMessages = _allMessages; //backup location because singletons are a bitch

        if (_allMessages.Count < 1)
        {
            Debug.Log("No Messages to Load.");
        }
    }

    public void LoadOnScreenMessages()
    {
        if (_allMessages.Count < 1)
        {
            if (GameManager.Instance.AllMessages != null && GameManager.Instance.AllMessages.Count > 0)
            {
                _allMessages = GameManager.Instance.AllMessages;
            }
            else
            {
                Debug.Log("No Messages to Load on screen.");
            }
        }

        ClearAllOnScreenMessages();

        var startingIndex = _allMessages.Count - MaxMessagesOnScreen;

        if (startingIndex < 0)
        {
            startingIndex = 0;
        }

        for (var i = startingIndex; i < MaxMessagesOnScreen && i < _allMessages.Count; i++)
        {
            CreateOnScreenMessage(_allMessages[i]);
        }
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

            _allMessages.Add(message);
            GameManager.Instance.AllMessages = _allMessages;

            CreateOnScreenMessage(message);
        }
    }
}
