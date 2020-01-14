using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectPopup : MonoBehaviour, ISubscriber
{
    private Button _selectedButton;

    public GameObject AvailableAbilityPrefab;
    public GameObject AbilityCategoryPrefab;
    public RectTransform AbilityCategoryParent;
    public GameObject AbilityDescription; //todo make tooltip

    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilitySelectPopupEventName, this);
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilitySelectedEventName, this);

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

    private void PopulateAllAbilities()
    {
        GlobalHelper.DestroyAllChildren(AbilityCategoryParent);

        var abilities = GameManager.Instance.Player.Abilities;

        foreach (var ability in abilities)
        {
            var abilityParent = AbilityCategoryParent;

            //abilityParent.GetComponent<LayoutElement>().preferredHeight = 30 * abilities.Count;

            var instance = Instantiate(AvailableAbilityPrefab, abilityParent.transform.position,
                Quaternion.identity);

            instance.transform.SetParent(abilityParent);

            instance.transform.GetComponentsInChildren<TextMeshProUGUI>()[0].text =
                GlobalHelper.CapitalizeAllWords(ability.Key);
        }
    }

    private void AbilitySelected(string abilityName)
    {
        var ability = GameManager.Instance.Player.Abilities[abilityName];

        AbilityManager.AssignAbilityToButton(ability, _selectedButton);

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void Show()
    {
        PopulateAllAbilities();
        gameObject.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromAllEvents(this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (eventName.Equals(GlobalHelper.AbilitySelectedEventName))
        {
            _selectedButton = parameter as Button;

            if (_selectedButton == null)
            {
                return;
            }

            var abilityName = _selectedButton.GetComponentInChildren<TextMeshProUGUI>().text;

            AbilitySelected(abilityName);
        }
        else if (eventName.Equals(GlobalHelper.AbilitySelectPopupEventName))
        {
            Show();
        }
    }
}
