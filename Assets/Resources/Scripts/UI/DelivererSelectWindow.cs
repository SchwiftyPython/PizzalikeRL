using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DelivererSelectWindow : MonoBehaviour
{
    private IDictionary<Guid, Entity> _descendants;

    private const int MinDescendants = 2;
    private const int MaxDescendants = 4;

    public Color ActiveTabColor;
    public Color InactiveTabColor;

    public GameObject DescendantPrefab;
    public GameObject DescendantButtonParent;

    public GameObject StrengthValueBox;
    public GameObject AgilityValueBox;
    public GameObject ConstitutionValueBox;
    public GameObject IntelligenceValueBox;
    public GameObject HpValueBox;
    public GameObject DefenseValueBox;
    public GameObject SpeedValueBox;

    public GameObject StatsWindow;
    public GameObject SkillsWindow;

    public GameObject StatsTab;
    public GameObject SkillsTab;

    public GameObject CurrentWindow;
    public Button CurrentWindowTab;

    public static DelivererSelectWindow Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        StatsWindow.SetActive(false);
        SkillsWindow.SetActive(false);

        StatsTab.GetComponent<Image>().color = InactiveTabColor;
        SkillsTab.GetComponent<Image>().color = InactiveTabColor;

        ShowInnerWindow(CurrentWindow, CurrentWindowTab);
    }

    private void Start()
    {
        PopulateWindow();
    }

    private void PopulateWindow()
    {
        _descendants = new Dictionary<Guid, Entity>();

        var numDescendants = Random.Range(MinDescendants, MaxDescendants + 1);

        for (var i = 0; i < numDescendants; i++)
        {
            var descendant = new Entity(GameManager.Instance.Player, null, true);

            _descendants.Add(descendant.Id, descendant);

            var descendantButton = Instantiate(DescendantPrefab, new Vector3(0, 0), Quaternion.identity);
            descendantButton.transform.SetParent(DescendantButtonParent.transform);

            var descendantTitle = descendantButton.GetComponentInChildren<Text>();
            descendantTitle.text = $"{descendant.Fluff.Name}, {descendant.Fluff.BackgroundType}";

            var playerSprite = descendantButton.GetComponentsInChildren<Image>()[1];
            playerSprite.sprite = descendant.GetSpritePrefab().GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void ShowInnerWindow(GameObject window, Button tab)
    {
        if (window != CurrentWindow)
        {
            HideInnerWindow(CurrentWindow, CurrentWindowTab);
            CurrentWindow = window;
            CurrentWindowTab = tab;
        }
        CurrentWindowTab.GetComponent<Image>().color = ActiveTabColor;

        CurrentWindow.SetActive(true);
    }

    public void HideInnerWindow(GameObject window, Button tab)
    {
        tab.GetComponent<Image>().color = InactiveTabColor;
        window.SetActive(false);
    }

    public void OnTabSelected(Button tab)
    {
        switch (tab.name)
        {
            case "Stats":
                ShowInnerWindow(StatsWindow, tab);
                break;
            case "Skills":
                ShowInnerWindow(SkillsWindow, tab);
                break;
        }
    }
}
