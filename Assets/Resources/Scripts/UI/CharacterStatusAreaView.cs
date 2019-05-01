using TMPro;
using UnityEngine;

public class CharacterStatusAreaView : MonoBehaviour
{
    private Entity _player;

    public GameObject CharacterNameLabel;
    public GameObject HpLabel;
    public GameObject DefenseLabel;
    public GameObject SpeedLabel;

    private void Start ()
    {
        _player = GameManager.Instance.Player;
    }

    //todo hook into eventsystem if this proves to be expensive
    private void Update ()
    {
        if (_player == null || _player != GameManager.Instance.Player)
        {
            _player = GameManager.Instance.Player;
        }

        CharacterNameLabel.GetComponent<TextMeshProUGUI>().text = _player.Fluff.Name;
        HpLabel.GetComponent<TextMeshProUGUI>().text = $@"{_player.CurrentHp}/{_player.MaxHp}";
        DefenseLabel.GetComponent<TextMeshProUGUI>().text = _player.Defense.ToString();
        SpeedLabel.GetComponent<TextMeshProUGUI>().text = _player.Speed.ToString();
    }
}
