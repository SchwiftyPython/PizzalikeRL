using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuWindow : MonoBehaviour, ISubscriber
{
    private const string GameMenuEventName = "GameMenuPopup";
    private const string JournalEventName = "PizzaJournal";

    private readonly IList<string> _subscribedEvents = new List<string>
    {
        GameMenuEventName,
        JournalEventName
    };

    private readonly Color _inactiveTabColor = new Color(255, 255, 255, .5f);
    private readonly Color _activeTabColor = new Color(255, 255, 255, .9f);

    public GameObject CurrentWindow;
    public Button CurrentWindowTab;

    public GameObject MainWindow;
    public GameObject PizzaOrderJournal;
    public GameObject EquipmentWindow;
    public GameObject InventoryWindow;
    public GameObject FilteredInventoryWindow;
    public GameObject FilteredInventoryWindowTitleBar;
    public GameObject CharacterWindow;
    public GameObject SystemWindow;

    public Button PizzaOrderJournalTab;
    public Button EquipmentTab;
    public Button InventoryTab;
    public Button CharacterTab;
    public Button SystemTab;

    private void Start()
    {
        MainWindow.SetActive(false);
        PizzaOrderJournalTab.GetComponent<Image>().color = _inactiveTabColor;
        EquipmentTab.GetComponent<Image>().color = _inactiveTabColor;
        InventoryTab.GetComponent<Image>().color = _inactiveTabColor;
        CharacterTab.GetComponent<Image>().color = _inactiveTabColor;
        SystemTab.GetComponent<Image>().color = _inactiveTabColor;

        CurrentWindowTab.GetComponent<Image>().color = _activeTabColor;

        SubscribeToEvents();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideMainWindow();
            }
        }
    }

    private void ShowMainWindow()
    {
        MainWindow.SetActive(true);
        ShowInnerWindow(CurrentWindow, CurrentWindowTab);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void HideMainWindow()
    {
        MainWindow.SetActive(false);
        HideInnerWindow(CurrentWindow, CurrentWindowTab);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void ShowInnerWindow(GameObject window, Button tab)
    {
        if (window != CurrentWindow)
        {
            HideInnerWindow(CurrentWindow, CurrentWindowTab);
            CurrentWindow = window;
            CurrentWindowTab = tab;
        }
        CurrentWindowTab.GetComponent<Image>().color = _activeTabColor;

        CurrentWindow.SetActive(true);
    }

    private void HideInnerWindow(GameObject window, Button tab)
    {
        tab.GetComponent<Image>().color = _inactiveTabColor;
        window.SetActive(false);
    }

    public void OnTabSelected(Button tab)
    {
        switch (tab.name)
        {
            case "PizzaOrderJournalTab":
                ShowInnerWindow(PizzaOrderJournal, tab);
                break;
            case "EquipmentTab":
                ShowInnerWindow(EquipmentWindow, tab);
                break;
            case "InventoryTab":
                ShowInnerWindow(InventoryWindow, tab);
                break;
            case "CharacterTab":
                ShowInnerWindow(CharacterWindow, tab);
                break;
            case "SystemTab":
                ShowInnerWindow(SystemWindow, tab);
                break;
        }
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (GameManager.Instance.AnyActiveWindows())
        {
            return;
        }

        if (eventName.Equals(GameMenuEventName))
        {
            ShowMainWindow();
        }
        else if (eventName.Equals(JournalEventName))
        {
            ShowMainWindow();
            OnTabSelected(PizzaOrderJournalTab);
        }
        
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void SubscribeToEvents()
    {
        foreach (var eventName in _subscribedEvents)
        {
            EventMediator.Instance.SubscribeToEvent(eventName, this);
        }
    }

    private void UnsubscribeFromEvents()
    {
        EventMediator.Instance.UnsubscribeFromAllEvents(this);
    }
}
