using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UseAbilityButton : MonoBehaviour, ISubscriber, IPointerDownHandler
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
        if (ability == null)
        {
            return;
        }

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

        if (!string.IsNullOrEmpty(ability.RequiresProperty))
        {
            CheckEquippedItemsForRequiredProperty();

            EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemEquippedEventName, this);
            EventMediator.Instance.SubscribeToEvent(GlobalHelper.ItemUnequippedEventName, this);
        }
        else
        {
            EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.ItemEquippedEventName, this);
        }

        InputController.Instance.UpdateAbilityBar(ability, gameObject);
    }

    public void CheckEquippedItemsForRequiredProperty()
    {
        DisableButton();

        foreach (var equippedItem in GameManager.Instance.Player.Equipped.Values)
        {
            if (equippedItem?.Properties != null && equippedItem.Properties.Contains(_ability.RequiresProperty))
            {
                EnableButton();
                break;
            }
        }
    }

    public void EnableButton()
    {
        _button.interactable = true;
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.EndTurnEventName, this);
    }

    public void DisableButton()
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

        InputController.Instance.UpdateAbilityBar(null, gameObject);
    }

    public bool AbilityAssigned()
    {
        return _ability != null;
    }

    public void SetIcon(Sprite newIcon)
    {
        _buttonIcon.sprite = newIcon;
    }

    public void OnClick()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("Left click");
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right click");
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (!_button.interactable || GameManager.Instance.WorldMapSceneActive())
        {
            return;
        }

        _remainingCooldownTurns = _ability.Cooldown;
        _ability.Use();
    }

    public void OnRightClick()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.AbilityButtonActionPopupEventName, this.gameObject, _button);
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
        else if (eventName == GlobalHelper.ItemEquippedEventName || eventName == GlobalHelper.ItemUnequippedEventName)
        {
            if (!((Entity) broadcaster).IsPlayer())
            {
                return;
            }

            CheckEquippedItemsForRequiredProperty();
        }
    }
}
