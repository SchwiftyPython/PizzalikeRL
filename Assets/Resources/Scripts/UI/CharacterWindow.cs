using TMPro;
using UnityEngine;

public class CharacterWindow : MonoBehaviour
{
    public TextMeshProUGUI NameValue;

    public TextMeshProUGUI StrengthValue;
    public TextMeshProUGUI AgilityValue;
    public TextMeshProUGUI ConstitutionValue;
    public TextMeshProUGUI IntelligenceValue;

    public TextMeshProUGUI SpeedValue;
    public TextMeshProUGUI DefenseValue;

    public TextMeshProUGUI LevelValue;
    public TextMeshProUGUI XpValue;
    public TextMeshProUGUI HpValue;

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
        HpValue.text = _player.CurrentHp + "/" + _player.MaxHp;
    }
}
