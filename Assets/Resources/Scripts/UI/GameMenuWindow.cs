using UnityEngine;
using UnityEngine.UI;

public class GameMenuWindow : MonoBehaviour
{
    public GameObject CurrentWindow;
    public Button CurrentWindowTab;

    public GameObject MainWindow;
    public GameObject PizzaOrderJournal;

    public Button PizzaOrderJournalTab;

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
        HideInnerWindow(CurrentWindow, CurrentWindowTab);
        CurrentWindow = window;
        CurrentWindowTab = tab;

        var tabColor = CurrentWindowTab.GetComponent<Image>().color;
        tabColor.a = 0.5f;
        CurrentWindowTab.GetComponent<Image>().color = tabColor;

        CurrentWindow.SetActive(true);
    }

    public void HideInnerWindow(GameObject window, Button tab)
    {
        var tabColor = tab.GetComponent<Image>().color;
        tabColor.a = 0.5f;
        tab.GetComponent<Image>().color = tabColor;
        window.SetActive(false);
    }

    public void OnTabSelected(Button tab)
    {
        switch (tab.name)
        {
            case "PizzaOrderJournalTab":
                ShowInnerWindow(PizzaOrderJournal, tab);
                break;
        }
    }
}
