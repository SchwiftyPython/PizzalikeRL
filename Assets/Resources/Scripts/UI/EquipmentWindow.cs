using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentWindow : MonoBehaviour
{
    public GameObject BodyPartPrefab;

    private List<GameObject> _bodyPartButtons;
    private Transform _parent;

    private IDictionary<BodyPart, Item> _playerEquipment;

    private char _keyMapLetter;

    private void Start()
    {
        _playerEquipment = GameManager.Instance.Player.Equipped;
        _bodyPartButtons = new List<GameObject>();
        _parent = transform;
        _keyMapLetter = 'a';
        PopulateWindow();
    }

    //todo detect and update changes in window
    /*private void Update()
    {
        if (isActiveAndEnabled)
        {
            if (_bodyPartButtons.Count > 0)
            {
                foreach (var button in _bodyPartButtons.ToArray())
                {
                    Destroy(button);
                }
                _bodyPartButtons = new List<GameObject>();
            }
            PopulateWindow();
        }
    }*/

    private void PopulateWindow()
    {
        _keyMapLetter = 'a';
        foreach (var bodyPart in _playerEquipment.Keys)
        {
            var bodyPartButton = Instantiate(BodyPartPrefab, new Vector3(0, 0), Quaternion.identity);
            bodyPartButton.transform.SetParent(_parent);
            var textFields = bodyPartButton.GetComponentsInChildren<Text>();
            textFields[0].text = "-  " + bodyPart.Type;
            textFields[1].text = _keyMapLetter.ToString();
            textFields[2].text = string.IsNullOrEmpty(_playerEquipment[bodyPart].ItemType) ? " :   -- " 
                : ":   " + _playerEquipment[bodyPart].ItemType;

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

    public void DisplayAvailableEquipmentForSelectedBodyPart()
    {
        
    }

    //todo need an equip method
}
