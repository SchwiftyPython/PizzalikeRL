using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    private readonly List<string> _nonPlayableSpecies = new List<string>
    {
        "pepperoni worm",
        "giant moth",
        "beetle"
    };

    private const string MainMenuScene = "MainMenu";
    private const string WorldGenerationSetupScene = "WorldGenerationSetup";

    private List<string> _playableSpecies;

    private EntityTemplate _playerTemplate;
    private Entity _player;
    private CharacterBackground _selectedBackground;

    private int _strength;
    private int _agility;
    private int _constitution;
    private int _intelligence;

    public GameObject SpeciesSelectPage;
    public GameObject StatPointAllocationPage;
    public GameObject ChooseBackgroundPage;

    public GameObject SpeciesOptionPrefab;
    public RectTransform SpeciesOptionParent;
    public GameObject SpeciesDescription;

    public GameObject BackgroundOptionPrefab;
    public RectTransform BackgroundOptionParent;
    public GameObject BackgroundDescription;

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
    }

    private void Start()
    {
        LoadPlayableSpeciesList();

        LoadCharacterBackgrounds();

        _playerTemplate = EntityTemplateLoader.GetEntityTemplate(_playableSpecies.First());

        DisplaySpeciesDescription(_playerTemplate.Description);
    }

    #region Navigation Buttons

    public void OnBackFromSpeciesSelectPage()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

    public void OnNextFromSpeciesSelectPage()
    {
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
        _strength = Convert.ToInt32(StrengthValueBox.GetComponent<Text>().text);
        _agility = Convert.ToInt32(AgilityValueBox.GetComponent<Text>().text);
        _constitution = Convert.ToInt32(ConstitutionValueBox.GetComponent<Text>().text);
        _intelligence = Convert.ToInt32(IntelligenceValueBox.GetComponent<Text>().text);

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
        _player = new Entity(_playerTemplate, null, true);
        _player.SetStats(_strength, _agility, _constitution, _intelligence);

        _player.CreateFluff();
        _player.Fluff.BackgroundType = _selectedBackground;
        _player.Fluff.Background = BackgroundGenerator.Instance.GenerateBackground();
        _player.Fluff.Age = 16 + DiceRoller.Instance.RollDice(new Dice(2, 6));

        GameManager.Instance.Player = _player;

        SceneManager.LoadScene(WorldGenerationSetupScene);
    }

    #endregion Navigation Buttons

    public void SelectSpeciesOption(EntityTemplate template)
    {
        _playerTemplate = template;

        DisplaySpeciesDescription(template.Description);
    }

    public void SelectCharacterBackgroundOption(CharacterBackground background)
    {
        _selectedBackground = background;

        DisplayCharacterBackgroundDescription(background.Description);
    }

    #region Stat Buttons

    public void OnClickIncreaseStrengthButton()
    {
        var strengthValue = int.Parse(StrengthValueBox.GetComponent<Text>().text);
        strengthValue++;
        StrengthValueBox.GetComponent<Text>().text = strengthValue.ToString();
    }

    public void OnClickDecreaseStrengthButton()
    {
        var strengthValue = int.Parse(StrengthValueBox.GetComponent<Text>().text);
        strengthValue--;
        StrengthValueBox.GetComponent<Text>().text = strengthValue.ToString();
    }

    public void OnClickIncreaseAgilityButton()
    {
        var agilityValue = int.Parse(AgilityValueBox.GetComponent<Text>().text);
        agilityValue++;
        AgilityValueBox.GetComponent<Text>().text = agilityValue.ToString();
    }

    public void OnClickDecreaseAgilityButton()
    {
        var agilityValue = int.Parse(AgilityValueBox.GetComponent<Text>().text);
        agilityValue--;
        AgilityValueBox.GetComponent<Text>().text = agilityValue.ToString();
    }

    public void OnClickIncreaseConstitutionButton()
    {
        var constitutionValue = int.Parse(ConstitutionValueBox.GetComponent<Text>().text);
        constitutionValue++;
        ConstitutionValueBox.GetComponent<Text>().text = constitutionValue.ToString();
    }

    public void OnClickDecreaseConstitutionButton()
    {
        var constitutionValue = int.Parse(ConstitutionValueBox.GetComponent<Text>().text);
        constitutionValue--;
        ConstitutionValueBox.GetComponent<Text>().text = constitutionValue.ToString();
    }

    public void OnClickIncreaseIntelligenceButton()
    {
        var intelligenceValue = int.Parse(IntelligenceValueBox.GetComponent<Text>().text);
        intelligenceValue++;
        IntelligenceValueBox.GetComponent<Text>().text = intelligenceValue.ToString();
    }

    public void OnClickDecreaseIntelligenceButton()
    {
        var intelligenceValue = int.Parse(IntelligenceValueBox.GetComponent<Text>().text);
        intelligenceValue--;
        IntelligenceValueBox.GetComponent<Text>().text = intelligenceValue.ToString();
    }

    #endregion Stat Buttons

    private void DisplaySpeciesDescription(string description)
    {
        SpeciesDescription.GetComponent<Text>().text = description;
    }

    private void LoadPlayableSpeciesList()
    {
        var allSpecies = EntityTemplateLoader.GetAllEntityTemplateTypes().ToList();

        _playableSpecies = allSpecies.Where(species => !_nonPlayableSpecies.Contains(species)).ToList();

        foreach (var species in _playableSpecies)
        {
            var option = Instantiate(SpeciesOptionPrefab, SpeciesOptionPrefab.transform.position, Quaternion.identity);

            option.transform.SetParent(SpeciesOptionParent);

            option.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            option.transform.GetChild(0).GetComponent<Text>().text = species;
        }
    }

    private void DisplayCharacterBackgroundDescription(string description)
    {
        BackgroundDescription.GetComponent<Text>().text = description;
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

            option.transform.GetChild(0).GetComponent<Text>().text = background;
        }
    }
}
