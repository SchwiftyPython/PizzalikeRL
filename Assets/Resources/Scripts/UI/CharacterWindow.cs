using UnityEngine;
using UnityEngine.UI;

public class CharacterWindow : MonoBehaviour
{
    public Text NameValue;

    public Text StrengthValue;
    public Text AgilityValue;
    public Text ConstitutionValue;
    public Text IntelligenceValue;

    public Text SpeedValue;
    public Text DefenseValue;

    public Text LevelValue;
    public Text XpValue;
    public Text CurrentHpValue;
    public Text MaxHpValue;

    private Entity _player;

    private void Start()
    {
        _player = GameManager.Instance.Player;

        PopulateWindow();
    }


    private void Update()
    {
        if (isActiveAndEnabled)
        {
            PopulateWindow();
        }
    }

    private void PopulateWindow()
    {
        NameValue.text = _player.Fluff.Name;

        StrengthValue.text = _player.Strength.ToString();
        AgilityValue.text = _player.Agility.ToString();
        ConstitutionValue.text = _player.Constitution.ToString();
        IntelligenceValue.text = _player.Intelligence.ToString();

        LevelValue.text = _player.Level.ToString();
        XpValue.text = _player.Xp.ToString();
        CurrentHpValue.text = _player.CurrentHp.ToString();
        MaxHpValue.text = "/" + _player.MaxHp;
    }
}
