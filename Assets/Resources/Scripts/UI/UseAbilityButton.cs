using UnityEngine;
using UnityEngine.UI;

public class UseAbilityButton : MonoBehaviour
{
    //todo cooldown mask
    private readonly Color _unassignedColor = new Color(1, 1, 1, 0);
    private readonly Color _assignedColor = new Color(1, 1, 1, 1);

    private Image _buttonIcon;
    private Button _button;
    private Ability _ability;

    private int _remainingCooldownTurns;

    public Sprite DefaultSprite;

    private void Awake()
    {
        _buttonIcon = gameObject.GetComponent<Image>();
        _button = gameObject.GetComponent<Button>();

        _button.interactable = false;
        _buttonIcon.color = _unassignedColor;
    }

    public void AssignAbility(Ability ability, Sprite icon)
    {
        if (_button == null)
        {
            _button = gameObject.GetComponent<Button>();
        }

        if (_buttonIcon == null)
        {
            _buttonIcon = gameObject.GetComponent<Image>();
        }

        _ability = ability;
        _button.interactable = true;
        _buttonIcon.color = _assignedColor;
        SetIcon(icon);
        _remainingCooldownTurns = ability.RemainingCooldownTurns;
    }

    public void RemoveAbility()
    {
        _ability = null;
        _button.interactable = false;
        _buttonIcon.color = _unassignedColor;
        SetIcon(DefaultSprite);
    }

    public void SetIcon(Sprite newIcon)
    {
        _buttonIcon.sprite = newIcon;
    }

    public void OnClick()
    {

    }
}
