using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItemPopup : MonoBehaviour
{
    private bool _processingInput;
    private IDictionary<char, GameObject> _buttons;
    private char _keyMapLetter;

    private List<Item> _items;

    public GameObject DroppedItemWindow;
    public GameObject TitleBar;
    public GameObject ActionBar;

    public GameObject CloseButton;
    public GameObject DroppedItemButtonPrefab;
    public GameObject TakeAllButton;

    public static DroppedItemPopup Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DroppedItemWindow.SetActive(false);
        TitleBar.SetActive(false);
        ActionBar.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown && !_processingInput)
        {
            _processingInput = true;
            char keyPressed;
            char.TryParse(Input.inputString, out keyPressed);
            if (_buttons.ContainsKey(keyPressed))
            {
                var pressedButton = _buttons[keyPressed].transform.GetComponent<Button>();
                pressedButton.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
            Debug.Log("Button Pressed: " + Input.inputString);
            Debug.Log("_buttons count: " + _buttons.Count);
            _processingInput = false;
        }
    }

    public void Hide()
    {
        DroppedItemWindow.SetActive(false);
        TitleBar.SetActive(false);
        ActionBar.SetActive(false);
    }

    public void DisplayDroppedItems()
    {
        _buttons = new Dictionary<char, GameObject>();
        _items = new List<Item>();

        var itemParent = transform;

        foreach (var item in GameManager.Instance.CurrentTile.PresentItems)
        {
            var itemButton = Instantiate(DroppedItemButtonPrefab, new Vector3(0, 0), Quaternion.identity);
            itemButton.transform.SetParent(itemParent);
            _buttons.Add(_keyMapLetter, itemButton);

            var textFields = itemButton.GetComponentsInChildren<Text>(true);
        }

        _keyMapLetter = 'a';
    }
}
