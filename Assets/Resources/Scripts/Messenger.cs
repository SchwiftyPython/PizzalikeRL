using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour {

    private static Messenger _instance;

    public GameObject Message;

    public RectTransform MessageParent;

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        }
        else if (_instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public static Messenger GetInstance() {
        return _instance;
    }

    public void CreateMessage(string myMessage, Color messageColor) {
        var cm = Instantiate(Message, Message.transform.position, Quaternion.identity);

        cm.transform.SetParent(MessageParent);

        cm.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        cm.transform.GetChild(0).GetComponent<Text>().text = myMessage;

        cm.transform.GetChild(0).GetComponent<Text>().color = messageColor;
    }
}
