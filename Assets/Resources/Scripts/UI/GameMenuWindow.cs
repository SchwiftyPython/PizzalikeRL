using UnityEngine;
using UnityEngine.UI;

public class GameMenuWindow : MonoBehaviour
{
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

    public Button PizzaOrderJournalTab;
    public Button EquipmentTab;
    public Button InventoryTab;
    public Button CharacterTab;

    public static GameMenuWindow Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        MainWindow.SetActive(false);
        PizzaOrderJournalTab.GetComponent<Image>().color = _inactiveTabColor;
        EquipmentTab.GetComponent<Image>().color = _inactiveTabColor;
        InventoryTab.GetComponent<Image>().color = _inactiveTabColor;
        CharacterTab.GetComponent<Image>().color = _inactiveTabColor;

        CurrentWindowTab.GetComponent<Image>().color = _activeTabColor;
    }

    public void ShowMainWindow()
    {
        MainWindow.SetActive(true);
        ShowInnerWindow(CurrentWindow, CurrentWindowTab);
    }

    public void HideMainWindow()
    {
        MainWindow.SetActive(false);
        HideInnerWindow(CurrentWindow, CurrentWindowTab);
    }

    public void ShowInnerWindow(GameObject window, Button tab)
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

    public void HideInnerWindow(GameObject window, Button tab)
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
        }
    }
}
