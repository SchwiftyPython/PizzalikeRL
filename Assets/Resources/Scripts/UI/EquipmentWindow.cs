using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentWindow : MonoBehaviour, ISubscriber
{
    public GameObject BodyPartPrefab;

    private IDictionary<char, GameObject> _bodyPartButtons;
    private Transform _parent;

    private IDictionary<BodyPart, Item> _playerEquipment;

    private char _keyMapLetter;

    public bool EquipmentChanged;

    public static EquipmentWindow Instance;

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

        _playerEquipment = new Dictionary<BodyPart, Item>(GameManager.Instance.Player.Equipped);
        _bodyPartButtons = new Dictionary<char, GameObject>();
        _parent = transform;
        _keyMapLetter = 'a';
        PopulateWindow();
        EventMediator.Instance.SubscribeToEvent("EquipmentChanged", this);
    }
    
    private void Update()
    {
        if (isActiveAndEnabled && !FilteredInventoryWindowPopUp.Instance.FilteredInventoryWindow.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                if (Input.inputString.Length < 1 || Input.inputString.Length > 1)
                {
                    return;
                }

                var keyPressed = Convert.ToChar(Input.inputString);
                if (!_bodyPartButtons.ContainsKey(keyPressed))
                {
                    return;
                }

                var pressedButton = _bodyPartButtons[keyPressed].transform.GetComponent<Button>();
                pressedButton.onClick.Invoke();
            }
        }
    }

    private void PopulateWindow()
    {
        _keyMapLetter = 'a';
        foreach (var bodyPart in _playerEquipment.Keys)
        {
            var bodyPartButton = Instantiate(BodyPartPrefab, new Vector3(0, 0), Quaternion.identity);
            _bodyPartButtons.Add(_keyMapLetter, bodyPartButton);
            bodyPartButton.transform.SetParent(_parent);

            var textFields = bodyPartButton.GetComponentsInChildren<TextMeshProUGUI>(true);

            var typeField = bodyPart.Type.Equals("special") ? bodyPart.Name : bodyPart.Type;

            textFields[0].text = "-  " + GlobalHelper.Capitalize(typeField);
            textFields[1].text = _keyMapLetter.ToString();
            textFields[2].text = string.IsNullOrEmpty(_playerEquipment[bodyPart].ItemType) ? " :   -- " 
                : ":   " + _playerEquipment[bodyPart].ItemType; //todo make this more descriptive of weapon like inventory screen
            textFields[3].text = bodyPart.Id.ToString();

            NextKeyMapLetter();
        }
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

    private void DestroyOldButtons()
    {
        if (_bodyPartButtons.Count > 0)
        {
            foreach (var button in _bodyPartButtons.Values.ToArray())
            {
                Destroy(button);
            }
            _bodyPartButtons = new Dictionary<char, GameObject>();
        }
    }

    private void Refresh()
    {
        _playerEquipment = new Dictionary<BodyPart, Item>(GameManager.Instance.Player.Equipped);
        DestroyOldButtons();
        PopulateWindow();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals("EquipmentChanged"))
        {
            Refresh();
        }
    }

    public void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent("EquipmentChanged", this);
    }
}
