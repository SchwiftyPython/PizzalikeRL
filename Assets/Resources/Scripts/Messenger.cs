using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour
{
    private const int MaxMessagesOnScreen = 3;

    private Queue<GameObject> _messagesOnScreen;

    private static Messenger _instance;

    public GameObject Message;

    public RectTransform MessageParent;

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
        DontDestroyOnLoad(gameObject);

        _messagesOnScreen = new Queue<GameObject>();
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

        cm.transform.GetChild(0).GetComponent<Text>().text = myMessage;

        _messagesOnScreen.Enqueue(cm);
    }

    public void CreateMessage(string myMessage, Color messageColor)
    {
        ClearExcessMessages();

        var cm = Instantiate(Message, Message.transform.position, Quaternion.identity);

        cm.transform.SetParent(MessageParent);

        cm.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        cm.transform.GetChild(0).GetComponent<Text>().text = myMessage;

        cm.transform.GetChild(0).GetComponent<Text>().color = messageColor;

        _messagesOnScreen.Enqueue(cm);
    }
}
