using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    private readonly List<string> _nonPlayableSpecies;

    private const string MainMenuScene = "MainMenu";
    private const string WorldGenerationScene = "WorldGeneration";

    private const int MinBaseStat = 8;
    private const int MaxBaseStat = 16;
    private const int StartingBaseStat = 12;
    private const int StartingPoints = 4;

    private List<string> _playableSpecies;

    private EntityTemplate _playerTemplate;
    private Entity _player;
    private CharacterBackground _selectedBackground;

    private int _strength;
    private int _agility;
    private int _constitution;
    private int _intelligence;
    private int _remainingPoints;

    public GameObject SpeciesSelectPage;
    public GameObject StatPointAllocationPage;
    public GameObject ChooseBackgroundPage;
    public GameObject AbilitySelectPage;
    public GameObject SummaryPage;

    public GameObject SpeciesOptionPrefab;
    public RectTransform SpeciesOptionParent;
    public GameObject SpeciesDescription;
    public GameObject SpeciesStartingAbilities;

    public GameObject BackgroundOptionPrefab;
    public RectTransform BackgroundOptionParent;
    public GameObject BackgroundDescription;
    public GameObject BackgroundStartingAbilities;

    public GameObject StartingAbilityPrefab;
    public RectTransform BackgroundStartingAbilityParent;
    public RectTransform SpeciesStartingAbilityParent;

    public GameObject AvailableAbilityPrefab;
    public GameObject AbilityCategoryPrefab;
    public RectTransform AbilityCategoryParent;
    public GameObject AbilityDescription;

    public GameObject StrengthValueBox;
    public GameObject IncreaseStrengthButton;
    public GameObject DecreaseStrengthButton;

    public GameObject AgilityValueBox;
    public GameObject IncreaseAgilityButton;
    public GameObject DecreaseAgilityButton;

    public GameObject ConstitutionValueBox;
    public GameObject IncreaseConstitutionButton;
    public GameObject DecreaseConstitutionButton;

    public GameObject IntelligenceValueBox;
    public GameObject IncreaseIntelligenceButton;
    public GameObject DecreaseIntelligenceButton;

    public GameObject RemainingPointsValue;

    public GameObject NameBox;
    public GameObject SpeciesBox;
    public GameObject BackgroundBox;

    public GameObject SummaryStrengthBox;
    public GameObject SummaryAgilityBox;
    public GameObject SummaryIntelligenceBox;
    public GameObject SummaryConstitutionBox;

    public GameObject WorldSeedPopup;
    public TMP_InputField SeedInputField;

    public GameObject SelectedButton;

    public static CharacterCreation Instance;

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

        SpeciesSelectPage.SetActive(true);
        StatPointAllocationPage.SetActive(false);
        ChooseBackgroundPage.SetActive(false);
        AbilitySelectPage.SetActive(false);
        SummaryPage.SetActive(false);
        WorldSeedPopup.SetActive(false);
    }

    private void Start()
    {
        LoadPlayableSpeciesList();

        LoadCharacterBackgrounds();

        SetStatsToStartingBaseValue();

        DisplaySpeciesDescription(_playerTemplate.Description);

        _remainingPoints = StartingPoints;

        RemainingPointsValue.GetComponent<TextMeshProUGUI>().text = _remainingPoints.ToString();
    }

    #region Navigation Buttons

    public void OnBackFromSpeciesSelectPage()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

    public void OnNextFromSpeciesSelectPage()
    {
        if (_playerTemplate == null)
        {
            return;
        }

        _player = new Entity(_playerTemplate, null, true);

        StatPointAllocationPage.SetActive(true);

        SpeciesSelectPage.SetActive(false);
    }

    public void OnBackFromStatPointAllocationPage()
    {
        SpeciesSelectPage.SetActive(true);

        StatPointAllocationPage.SetActive(false);
    }

    public void OnNextFromStatPointAllocationPage()
    {
        _strength = Convert.ToInt32(StrengthValueBox.GetComponent<TextMeshProUGUI>().text);
        _agility = Convert.ToInt32(AgilityValueBox.GetComponent<TextMeshProUGUI>().text);
        _constitution = Convert.ToInt32(ConstitutionValueBox.GetComponent<TextMeshProUGUI>().text);
        _intelligence = Convert.ToInt32(IntelligenceValueBox.GetComponent<TextMeshProUGUI>().text);

        SelectCharacterBackgroundOption(_selectedBackground);

        BackgroundDescription.SetActive(true);
        BackgroundStartingAbilities.SetActive(false);

        ChooseBackgroundPage.SetActive(true);

        StatPointAllocationPage.SetActive(false);
    }

    public void OnBackFromChooseBackgroundPage()
    {
        StatPointAllocationPage.SetActive(true);

        ChooseBackgroundPage.SetActive(false);
    }

    public void OnNextFromChooseBackgroundPage()
    {
        LoadAbilitiesForAbilitySelectScreen();

        AbilitySelectPage.SetActive(true);

        ChooseBackgroundPage.SetActive(false);
    }

    public void OnBackFromAbilitySelectPage()
    {
        ChooseBackgroundPage.SetActive(true);

        AbilitySelectPage.SetActive(false);
    }

    public void OnNextFromAbilitySelectPage()
    {
        SpeciesBox.GetComponent<TextMeshProUGUI>().text = GlobalHelper.Capitalize(_player.EntityType);
        BackgroundBox.GetComponent<TextMeshProUGUI>().text = GlobalHelper.Capitalize(_selectedBackground.Name);

        SummaryStrengthBox.GetComponent<TextMeshProUGUI>().text = _strength.ToString();
        SummaryAgilityBox.GetComponent<TextMeshProUGUI>().text = _agility.ToString();
        SummaryIntelligenceBox.GetComponent<TextMeshProUGUI>().text = _intelligence.ToString();
        SummaryConstitutionBox.GetComponent<TextMeshProUGUI>().text = _constitution.ToString();

        AbilitySelectPage.SetActive(false);

        SummaryPage.SetActive(true);
    }

    public void OnBackFromSummaryPage()
    {
        AbilitySelectPage.SetActive(true);

        SummaryPage.SetActive(false);
    }

    public void OnNextFromSummaryPage()
    {
        WorldSeedPopup.SetActive(true);
    }

    public void OnBackFromSeedPopup()
    {
        WorldSeedPopup.SetActive(false);
    }

    public void OnStartGame()
    {
        PreparePlayerForPlay();

        if (string.IsNullOrEmpty(SeedInputField.text) || string.IsNullOrWhiteSpace(SeedInputField.text))
        {
            WorldData.Instance.Seed = (UnityEngine.Random.Range(int.MinValue, int.MaxValue) +
                                       (int)DateTime.Now.Ticks).ToString();
        }
        else
        {
            WorldData.Instance.Seed = SeedInputField.text;
        }

        SceneManager.LoadScene(WorldGenerationScene);
    }

    #endregion Navigation Buttons

    public void SelectSpeciesOption(EntityTemplate template)
    {
        _playerTemplate = template;

        LoadStartingAbilitiesForSpecies(template);

        if (SpeciesDescription.activeSelf)
        {
            DisplaySpeciesDescription(template.Description);
        }
        else
        {
            DisplaySpeciesStartingAbilities();
        }
    }

    public void DisplaySpeciesStartingAbilities()
    {
        SpeciesDescription.SetActive(false);

        SpeciesStartingAbilities.SetActive(true);
    }

    public void DisplaySpeciesDescription(string description)
    {
        SpeciesDescription.GetComponent<TextMeshProUGUI>().text = description.Trim();

        SpeciesDescription.SetActive(true);

        SpeciesStartingAbilities.SetActive(false);
    }

    public void DisplaySpeciesDescription()
    {
        var description = _playerTemplate.Description;

        DisplaySpeciesDescription(description);
    }

    public void SelectCharacterBackgroundOption(CharacterBackground background)
    {
        _selectedBackground = background;

        var description = background.Description;

        LoadStartingAbilitiesForBackground(background);

        if (BackgroundDescription.activeSelf)
        {
            DisplayCharacterBackgroundDescription(description);
        }
        else
        {
            DisplayBackgroundStartingAbilities();
        }
    }

    public void DisplayBackgroundStartingAbilities()
    {
        BackgroundDescription.SetActive(false);

        BackgroundStartingAbilities.SetActive(true);
    }

    #region Stat Buttons

    public void OnClickIncreaseStrengthButton()
    {
        var strengthValue = int.Parse(StrengthValueBox.GetComponent<TextMeshProUGUI>().text);
        strengthValue++;
        StrengthValueBox.GetComponent<TextMeshProUGUI>().text = strengthValue.ToString();

        if (strengthValue >= MaxBaseStat)
        {
            IncreaseStrengthButton.GetComponent<Button>().interactable = false;
        }
        if (strengthValue > MinBaseStat)
        {
            DecreaseStrengthButton.GetComponent<Button>().interactable = true;
        }

        DecreaseRemainingPointsValue();
    }

    public void OnClickDecreaseStrengthButton()
    {
        var strengthValue = int.Parse(StrengthValueBox.GetComponent<TextMeshProUGUI>().text);
        strengthValue--;
        StrengthValueBox.GetComponent<TextMeshProUGUI>().text = strengthValue.ToString();

        if (strengthValue <= MinBaseStat)
        {
            DecreaseStrengthButton.GetComponent<Button>().interactable = false;
        }
        if (strengthValue < MaxBaseStat)
        {
            IncreaseStrengthButton.GetComponent<Button>().interactable = true;
        }

        IncreaseRemainingPointsValue();
    }

    public void OnClickIncreaseAgilityButton()
    {
        var agilityValue = int.Parse(AgilityValueBox.GetComponent<TextMeshProUGUI>().text);
        agilityValue++;
        AgilityValueBox.GetComponent<TextMeshProUGUI>().text = agilityValue.ToString();

        if (agilityValue >= MaxBaseStat)
        {
            IncreaseAgilityButton.GetComponent<Button>().interactable = false;
        }
        if (agilityValue > MinBaseStat)
        {
            DecreaseAgilityButton.GetComponent<Button>().interactable = true;
        }

        DecreaseRemainingPointsValue();
    }

    public void OnClickDecreaseAgilityButton()
    {
        var agilityValue = int.Parse(AgilityValueBox.GetComponent<TextMeshProUGUI>().text);
        agilityValue--;
        AgilityValueBox.GetComponent<TextMeshProUGUI>().text = agilityValue.ToString();

        if (agilityValue <= MinBaseStat)
        {
            DecreaseAgilityButton.GetComponent<Button>().interactable = false;
        }
        if (agilityValue < MaxBaseStat)
        {
            IncreaseAgilityButton.GetComponent<Button>().interactable = true;
        }

        IncreaseRemainingPointsValue();
    }

    public void OnClickIncreaseConstitutionButton()
    {
        var constitutionValue = int.Parse(ConstitutionValueBox.GetComponent<TextMeshProUGUI>().text);
        constitutionValue++;
        ConstitutionValueBox.GetComponent<TextMeshProUGUI>().text = constitutionValue.ToString();

        if (constitutionValue >= MaxBaseStat)
        {
            IncreaseConstitutionButton.GetComponent<Button>().interactable = false;
        }
        if (constitutionValue > MinBaseStat)
        {
            DecreaseConstitutionButton.GetComponent<Button>().interactable = true;
        }

        DecreaseRemainingPointsValue();
    }

    public void OnClickDecreaseConstitutionButton()
    {
        var constitutionValue = int.Parse(ConstitutionValueBox.GetComponent<TextMeshProUGUI>().text);
        constitutionValue--;
        ConstitutionValueBox.GetComponent<TextMeshProUGUI>().text = constitutionValue.ToString();

        if (constitutionValue <= MinBaseStat)
        {
            DecreaseConstitutionButton.GetComponent<Button>().interactable = false;
        }
        if (constitutionValue < MaxBaseStat)
        {
            IncreaseConstitutionButton.GetComponent<Button>().interactable = true;
        }

        IncreaseRemainingPointsValue();
    }

    public void OnClickIncreaseIntelligenceButton()
    {
        var intelligenceValue = int.Parse(IntelligenceValueBox.GetComponent<TextMeshProUGUI>().text);
        intelligenceValue++;
        IntelligenceValueBox.GetComponent<TextMeshProUGUI>().text = intelligenceValue.ToString();

        if (intelligenceValue >= MaxBaseStat)
        {
            IncreaseIntelligenceButton.GetComponent<Button>().interactable = false;
        }
        if (intelligenceValue > MinBaseStat)
        {
            DecreaseIntelligenceButton.GetComponent<Button>().interactable = true;
        }

        DecreaseRemainingPointsValue();
    }

    public void OnClickDecreaseIntelligenceButton()
    {
        var intelligenceValue = int.Parse(IntelligenceValueBox.GetComponent<TextMeshProUGUI>().text);
        intelligenceValue--;
        IntelligenceValueBox.GetComponent<TextMeshProUGUI>().text = intelligenceValue.ToString();

        if (intelligenceValue <= MinBaseStat)
        {
            DecreaseIntelligenceButton.GetComponent<Button>().interactable = false;
        }
        if (intelligenceValue < MaxBaseStat)
        {
            IncreaseIntelligenceButton.GetComponent<Button>().interactable = true;
        }

        IncreaseRemainingPointsValue();
    }

    #endregion Stat Buttons

    

    private void LoadPlayableSpeciesList()
    {
        var allSpecies = EntityTemplateLoader.GetAllEntityTemplateTypes().OrderBy(s => s).ToList();

        foreach (var species in allSpecies)
        {
            if (!EntityTemplateLoader.GetEntityTemplate(species).Playable)
            {
                continue;
            }

            var option = Instantiate(SpeciesOptionPrefab, SpeciesOptionPrefab.transform.position, Quaternion.identity);

            option.transform.SetParent(SpeciesOptionParent);

            option.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            option.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = species;

            if (species != allSpecies.First())
            {
                continue;
            }

            SelectButton(option);
        }

        SelectSpeciesOption(EntityTemplateLoader.GetEntityTemplate(allSpecies.First()));
    }

    public void DisplayCharacterBackgroundDescription(string description)
    {
        BackgroundDescription.GetComponent<TextMeshProUGUI>().text = description.Trim();

        BackgroundDescription.SetActive(true);

        BackgroundStartingAbilities.SetActive(false);
    }

    public void DisplayCharacterBackgroundDescription()
    {
        var description = _selectedBackground.Description;

        DisplayCharacterBackgroundDescription(description);
    }

    private void LoadCharacterBackgrounds()
    {
        var allBackgrounds = CharacterBackgroundLoader.GetCharacterBackgroundTypes().ToList();

        foreach (var background in allBackgrounds)
        {
            var option = Instantiate(BackgroundOptionPrefab, BackgroundOptionPrefab.transform.position,
                Quaternion.identity);

            option.transform.SetParent(BackgroundOptionParent);

            option.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            option.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = background;

            if (background != allBackgrounds.First())
            {
                continue;
            }

            SelectButton(option);
        }

        _selectedBackground = CharacterBackgroundLoader.GetCharacterBackground(allBackgrounds.First());
    }

    private void LoadStartingAbilitiesForBackground(CharacterBackground background)
    {
        GlobalHelper.DestroyAllChildren(BackgroundStartingAbilityParent);

        var backgroundAbilities = AbilityStore.GetAbilitiesByBackground(background);

        foreach (var ability in backgroundAbilities)
        {
            var instance = Instantiate(StartingAbilityPrefab, StartingAbilityPrefab.transform.position,
                Quaternion.identity);

            instance.transform.SetParent(BackgroundStartingAbilityParent);

            instance.transform.GetComponentsInChildren<TextMeshProUGUI>()[0].text = GlobalHelper.CapitalizeAllWords(ability.Name);

            instance.transform.GetComponentsInChildren<TextMeshProUGUI>()[1].text = ability.Description;
        }
    }

    private void LoadStartingAbilitiesForSpecies(EntityTemplate species)
    {
        GlobalHelper.DestroyAllChildren(SpeciesStartingAbilityParent);
        
        var startingAbilities = new List<Ability>();

        foreach (var bodyPartName in species.Parts)
        {
            var partAbilities = AbilityStore.GetAbilitiesByBodyPart(bodyPartName);

            if(partAbilities == null)
            {
                continue;
            }

            startingAbilities.AddRange(partAbilities);
        }

        foreach (var ability in startingAbilities)
        {
            var instance = Instantiate(StartingAbilityPrefab, StartingAbilityPrefab.transform.position,
                Quaternion.identity);

            instance.transform.SetParent(SpeciesStartingAbilityParent);

            instance.transform.GetComponentsInChildren<TextMeshProUGUI>()[0].text = GlobalHelper.CapitalizeAllWords(ability.Name);

            instance.transform.GetComponentsInChildren<TextMeshProUGUI>()[1].text = ability.Description;
        }
    }

    private void LoadAbilitiesForAbilitySelectScreen()
    {
        //todo Filter out starting abilities

        GlobalHelper.DestroyAllChildren(AbilityCategoryParent);

        var bodyPartAbilities = new List<Ability>();

        foreach (var bodyPartName in _playerTemplate.Parts)
        {
            var partAbilities = AbilityStore.GetAbilitiesByBodyPart(bodyPartName);

            if (partAbilities == null)
            {
                continue;
            }

            bodyPartAbilities.AddRange(partAbilities);
        }

        var categoryInstance = Instantiate(AbilityCategoryPrefab, AbilityCategoryPrefab.transform.position,
            Quaternion.identity);

        categoryInstance.transform.SetParent(AbilityCategoryParent);

        categoryInstance.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Species Abilities";

        var abilityParent = categoryInstance.GetComponentsInChildren<RectTransform>(true)[7];

        abilityParent.GetComponent<LayoutElement>().preferredHeight = 30 * bodyPartAbilities.Count;

        foreach (var ability in bodyPartAbilities)
        {
            var instance = Instantiate(AvailableAbilityPrefab, StartingAbilityPrefab.transform.position,
                Quaternion.identity);

            instance.transform.SetParent(abilityParent);

            instance.transform.GetComponentsInChildren<TextMeshProUGUI>()[0].text = GlobalHelper.CapitalizeAllWords(ability.Name);
        }


        var backgroundAbilities = AbilityStore.GetAbilitiesByBackground(_selectedBackground);

        var damageTypeAbilities = AbilityStore.GetAllDamageTypeAbilities();
    }

    public void DisplaySelectedAbilityDescription(string abilityName)
    {
        var selectedAbility = AbilityStore.GetAbilityByName(abilityName);

        AbilityDescription.GetComponent<TextMeshProUGUI>().text = selectedAbility.Description;
    }

    public void SelectButton(GameObject button)
    {
        if (SelectedButton != null)
        {
            SelectedButton.GetComponent<Button>().interactable = true;
        }

        EventSystem.current.SetSelectedGameObject(button, null);
        button.GetComponent<Button>().interactable = false;

        SelectedButton = button;
    }
    

    private void IncreaseRemainingPointsValue()
    {
        IncreaseStrengthButton.GetComponent<Button>().interactable = true;
        IncreaseAgilityButton.GetComponent<Button>().interactable = true;
        IncreaseConstitutionButton.GetComponent<Button>().interactable = true;
        IncreaseIntelligenceButton.GetComponent<Button>().interactable = true;

        _remainingPoints++;
        RemainingPointsValue.GetComponent<TextMeshProUGUI>().text = _remainingPoints.ToString();
    }

    private void DecreaseRemainingPointsValue()
    {
        _remainingPoints--;

        if (_remainingPoints <= 0)
        {
            _remainingPoints = 0;

            IncreaseStrengthButton.GetComponent<Button>().interactable = false;
            IncreaseAgilityButton.GetComponent<Button>().interactable = false;
            IncreaseConstitutionButton.GetComponent<Button>().interactable = false;
            IncreaseIntelligenceButton.GetComponent<Button>().interactable = false;
        }

        RemainingPointsValue.GetComponent<TextMeshProUGUI>().text = _remainingPoints.ToString();
    }

    private void SetStatsToStartingBaseValue()
    {
        _strength = StartingBaseStat;
        _agility = StartingBaseStat;
        _constitution = StartingBaseStat;
        _intelligence = StartingBaseStat;

        StrengthValueBox.GetComponent<TextMeshProUGUI>().text = _strength.ToString();
        AgilityValueBox.GetComponent<TextMeshProUGUI>().text = _agility.ToString();
        ConstitutionValueBox.GetComponent<TextMeshProUGUI>().text = _constitution.ToString();
        IntelligenceValueBox.GetComponent<TextMeshProUGUI>().text = _intelligence.ToString();
    }

    private void PreparePlayerForPlay()
    {
        _player = new Entity(_playerTemplate, null, true);
        _player.SetStats(_strength, _agility, _constitution, _intelligence);

        _player.CreateFluff(_playerTemplate);
        _player.Fluff.BackgroundType = _selectedBackground;
        _player.Fluff.Background = BackgroundGenerator.Instance.GenerateBackground();
        _player.Fluff.Age = 16 + DiceRoller.Instance.RollDice(new Dice(2, 6));

        var enteredName = NameBox.GetComponent<TextMeshProUGUI>().text.Trim();

        if (enteredName.Length > 1 && !string.IsNullOrEmpty(enteredName) && !string.IsNullOrWhiteSpace(enteredName))
        {
            _player.Fluff.Name = enteredName;
        }

        GameManager.Instance.Player = _player;

        WorldData.Instance.Entities.Add(_player.Id, _player);
    }
}
