using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonActionWindow : MonoBehaviour, ISubscriber
{
    private Button _selectedButton;
    private GameObject _buttonParent;

    public Button AssignButton;
    public Button RemoveButton;

    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilityButtonActionPopupEventName, this);

        Hide();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
    }

    public void OnAssignButtonClick()
    {
        EventMediator.Instance.Broadcast(GlobalHelper.AbilitySelectPopupEventName, _buttonParent, _selectedButton);
        Hide();
    }

    public void OnRemoveButtonClick()
    {
        _selectedButton.GetComponent<UseAbilityButton>().RemoveAbility();
        Hide();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName == GlobalHelper.AbilityButtonActionPopupEventName)
        {
            _buttonParent = broadcaster as GameObject;

            if (_buttonParent == null)
            {
                return;
            }

            _selectedButton = parameter as Button;

            if (_selectedButton == null)
            {
                return;
            }

            Show(((GameObject)broadcaster).GetComponent<Button>().GetComponent<UseAbilityButton>().AbilityAssigned());
        }
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityButtonActionPopupEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void Show(bool abilityAssigned)
    {
        RemoveButton.interactable = abilityAssigned;

        gameObject.SetActive(true);

        var pos = Input.mousePosition;

        gameObject.transform.position = new Vector2(pos.x + 60f, pos.y + 50f);

        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void Hide()
    {
        gameObject.SetActive(false);

        GameManager.Instance.RemoveActiveWindow(gameObject);
    }
}
