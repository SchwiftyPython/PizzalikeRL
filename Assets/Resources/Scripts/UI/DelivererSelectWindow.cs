using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//todo add modified character creation for player to spend points
public class DelivererSelectWindow : MonoBehaviour
{
    private const int MinDescendants = 2;
    private const int MaxDescendants = 4;

    private IDictionary<Guid, Entity> _descendants;
    private Entity _selectedDescendant;

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
        DisplayDescendantDetails(_descendants.First().Key);
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

    public void DisplayDescendantDetails(Guid id)
    {
        if (id == Guid.Empty || !_descendants.ContainsKey(id))
        {
            return;
        }

        _selectedDescendant = _descendants[id];

        StrengthValueBox.GetComponent<TextMeshProUGUI>().text = _selectedDescendant.Strength.ToString();
        AgilityValueBox.GetComponent<TextMeshProUGUI>().text = _selectedDescendant.Agility.ToString();
        ConstitutionValueBox.GetComponent<TextMeshProUGUI>().text = _selectedDescendant.Constitution.ToString();
        IntelligenceValueBox.GetComponent<TextMeshProUGUI>().text = _selectedDescendant.Intelligence.ToString();
        HpValueBox.GetComponent<TextMeshProUGUI>().text = _selectedDescendant.MaxHp.ToString();
        DefenseValueBox.GetComponent<TextMeshProUGUI>().text = _selectedDescendant.Defense.ToString();
        SpeedValueBox.GetComponent<TextMeshProUGUI>().text = _selectedDescendant.Speed.ToString();
    }

    public void OnContinueClick()
    {
        GameManager.Instance.ContinueGame(_selectedDescendant);
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

            if (i == 0)
            {
                descendantButton.GetComponent<Button>().Select();
            }

            var descendantTitle = descendantButton.GetComponentInChildren<TextMeshProUGUI>();
            descendantTitle.text = $"{descendant.Fluff.Name}, {GlobalHelper.Capitalize(descendant.Fluff.BackgroundType.Name)}";

            var id = descendantButton.GetComponentInChildren<Text>(true);
            id.text = descendant.Id.ToString();

            var playerSprite = descendantButton.GetComponentsInChildren<Image>()[1];
            playerSprite.sprite = descendant.GetSpritePrefab().GetComponent<SpriteRenderer>().sprite;
        }
    }
}
