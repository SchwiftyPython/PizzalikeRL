using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DroppedItemPopup : MonoBehaviour
{
    private bool _processingInput;
    public IDictionary<char, GameObject> Buttons { get; private set; }
    private char _keyMapLetter;

    private List<Item> _items;

    public GameObject DroppedItemWindow;
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
        ActionBar.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown && !_processingInput)
        {
            _processingInput = true;
            char.TryParse(Input.inputString, out var keyPressed);
            if (Buttons.ContainsKey(keyPressed))
            {
                var pressedButton = Buttons[keyPressed].transform.GetComponent<Button>();
                pressedButton.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
            Debug.Log("Button Pressed: " + Input.inputString);
            Debug.Log("_buttons count: " + Buttons.Count);
            _processingInput = false;
        }
    }

    public void Hide()
    {
        DestroyOldItemButtons();
        DroppedItemWindow.SetActive(false);
        ActionBar.SetActive(false);
    }

    public void Refresh()
    {
        DestroyOldItemButtons();
        DisplayDroppedItems();
    }

    public void DisplayDroppedItems()
    {
        if (GameManager.Instance.CurrentTile.PresentItems == null ||
            GameManager.Instance.CurrentTile.PresentItems.Count < 1)
        {
            return;
        }

        Buttons = new Dictionary<char, GameObject>();
        _items = new List<Item>();

        var itemParent = transform;

        _keyMapLetter = 'a';
        foreach (var item in GameManager.Instance.CurrentTile.PresentItems)
        {
            var itemButton = Instantiate(DroppedItemButtonPrefab, new Vector3(0, 0), Quaternion.identity);
            itemButton.transform.SetParent(itemParent);
            Buttons.Add(_keyMapLetter, itemButton);

            var textFields = itemButton.GetComponentsInChildren<Text>(true);

            //todo come up with some kind of naming system based on material or legend
            if (item.ItemCategory.Equals("weapon"))
            {
                textFields[0].text = "-  " + item.ItemType + "     [ " + item.ItemDice.NumDice + "d" + item.ItemDice.NumSides + " ]"; //todo add a sword icon
                textFields[1].text = _keyMapLetter.ToString();
            }
            else if (item.ItemCategory.Equals("armor"))
            {
                var defense = ((Armor)item).Defense;
                textFields[0].text = "-  " + item.ItemType + "     [ " + defense + " def ]"; //todo replace def with a shield icon
                textFields[1].text = _keyMapLetter.ToString();
            }
            textFields[2].text = item.Id.ToString();
            NextKeyMapLetter();
        }

        DroppedItemWindow.SetActive(true);
        ActionBar.SetActive(true);
    }

    public void DisplayItemsInTargetTile(Tile target)
    {
        if ((target.PresentItems == null ||
            target.PresentItems.Count < 1) &&
            (target.PresentProp == null ||
            !target.PresentProp.IsContainer))
        {
            return;
        }

        Buttons = new Dictionary<char, GameObject>();
        _items = new List<Item>();

        var container = (Chest)target.PresentProp;

        if (container != null)
        {
            _items.AddRange(container.GetContents());
        }

        if (target.PresentItems != null)
        {
            _items.AddRange(target.PresentItems);
        }

        var itemParent = transform;

        _keyMapLetter = 'a';
        foreach (var item in _items)
        {
            var itemButton = Instantiate(DroppedItemButtonPrefab, new Vector3(0, 0), Quaternion.identity);
            itemButton.transform.SetParent(itemParent);
            Buttons.Add(_keyMapLetter, itemButton);

            var textFields = itemButton.GetComponentsInChildren<Text>(true);

            //todo come up with some kind of naming system based on material or legend
            if (item.ItemCategory.Equals("weapon"))
            {
                textFields[0].text = "-  " + item.ItemType + "     [ " + item.ItemDice.NumDice + "d" + item.ItemDice.NumSides + " ]"; //todo add a sword icon
                textFields[1].text = _keyMapLetter.ToString();
            }
            else if (item.ItemCategory.Equals("armor"))
            {
                var defense = ((Armor)item).Defense;
                textFields[0].text = "-  " + item.ItemType + "     [ " + defense + " def ]"; //todo replace def with a shield icon
                textFields[1].text = _keyMapLetter.ToString();
            }
            textFields[2].text = item.Id.ToString();
            NextKeyMapLetter();
        }

        DroppedItemWindow.SetActive(true);
        ActionBar.SetActive(true);
    }

    private void NextKeyMapLetter()
    {
        if (_keyMapLetter == 'z')
        {
            _keyMapLetter = 'a';
        }
        else if (_keyMapLetter == 'Z')
        {
            _keyMapLetter = 'A';
        }
        else
        {
            _keyMapLetter = (char)(_keyMapLetter + 1);
        }
    }

    private void DestroyOldItemButtons()
    {
        foreach (var button in Buttons.Values.ToArray())
        {
            Destroy(button);
        }
        Buttons.Clear();
    }
}
