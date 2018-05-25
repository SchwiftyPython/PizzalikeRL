using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentWindow : MonoBehaviour
{
    public GameObject BodyPartPrefab;

    private List<GameObject> _bodyPartButtons;
    private Transform _parent;

    private IDictionary<string, BodyPart> _playerBodyParts;

    private char _keyMapLetter;

    private void Start()
    {
        _playerBodyParts = GameManager.Instance.Player.Body;
        _bodyPartButtons = new List<GameObject>();
        _parent = transform;
        _keyMapLetter = 'a';
        PopulateWindow();
    }

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
        foreach (var bodyPart in _playerBodyParts)
        {
            var bodyPartButton = Instantiate(BodyPartPrefab, new Vector3(0, 0), Quaternion.identity);
            bodyPartButton.transform.SetParent(_parent);
            var textFields = bodyPartButton.GetComponentsInChildren<Text>();
            textFields[0].text = "-  " + bodyPart.Key;
            textFields[1].text = _keyMapLetter.ToString();
            //textFields[2].text = todo display equipped item

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
}
