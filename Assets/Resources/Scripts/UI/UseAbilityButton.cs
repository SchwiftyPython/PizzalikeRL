using UnityEngine;
using UnityEngine.UI;

public class UseAbilityButton : MonoBehaviour, ISubscriber
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

        ability.AssignAbilityToButton(_button);

        EventMediator.Instance.SubscribeToEvent(GlobalHelper.EndTurnEventName, this);
    }

    private void EnableButton()
    {
        _button.interactable = true;
    }

    private void DisableButton()
    {
        _button.interactable = false;
    }

    public void RemoveAbility()
    {
        _ability = null;
        _button.interactable = false;
        _buttonIcon.color = _unassignedColor;
        SetIcon(DefaultSprite);

        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);
    }

    public void SetIcon(Sprite newIcon)
    {
        _buttonIcon.sprite = newIcon;
    }

    public void OnClick()
    {
        _remainingCooldownTurns = _ability.Cooldown;
        _ability.Use();
        DisableButton();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.EndTurnEventName)
        {
            if (_remainingCooldownTurns > 0)
            {
                _remainingCooldownTurns--;
            }
            else
            {
                EnableButton();
            }
        }
    }
}
