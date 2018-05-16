using UnityEngine;
using UnityEngine.UI;

public class GameMenuWindow : MonoBehaviour
{
    private GameObject _currentWindow;
    private Button _currentWindowTab;

    public GameObject MainWindow;
    public GameObject PizzaOrderJournal;

    public Button PizzaOrderJournalTab;

    public void Show(GameObject window, Button tab)
    {
        Hide(_currentWindow, _currentWindowTab);
        _currentWindow = window;
        _currentWindowTab = tab;

        var tabColor = _currentWindowTab.GetComponent<Image>().color;
        tabColor.a = 0.5f;
        _currentWindowTab.GetComponent<Image>().color = tabColor;

        _currentWindow.SetActive(true);
    }

    public void Hide(GameObject window, Button tab)
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
            case "Orders":
                Show(PizzaOrderJournal, tab);
                break;
        }
    }
}
