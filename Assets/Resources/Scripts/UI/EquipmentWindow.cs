using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentWindow : MonoBehaviour
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
    }
    
    private void Update()
    {
        if (isActiveAndEnabled && !FilteredInventoryWindowPopUp.Instance.FilteredInventoryWindow.activeSelf)
        {
            if (EquipmentChanged)
            {
                EquipmentChanged = false;
                _playerEquipment = new Dictionary<BodyPart, Item>(GameManager.Instance.Player.Equipped);

                DestroyOldButtons();
                PopulateWindow();
            }
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

            var textFields = bodyPartButton.GetComponentsInChildren<Text>();
            textFields[0].text = "-  " + bodyPart.Type;
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
}
