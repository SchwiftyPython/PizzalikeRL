using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    private readonly List<string> _nonPlayableSpecies = new List<string>
    {
        "pepperoni worm"
    };

    private const string MainMenuScene = "MainMenu";

    private List<string> _playableSpecies;

    private EntityTemplate _playerTemplate;
    private Entity _player;

    public GameObject SpeciesSelectPage;
    public GameObject StatPointAllocationPage;
    public GameObject ChooseBackgroundPage;

    public GameObject SpeciesOptionPrefab;
    public RectTransform SpeciesOptionParent;
    public GameObject SpeciesDescription;

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
        //Create world scene?
    }

    #endregion Navigation Buttons

    public void SelectSpeciesOption(EntityTemplate template)
    {
        _playerTemplate = template;

        DisplaySpeciesDescription(template.Description);
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
        var allSpecies = EntityTemplateLoader.GetEntityTemplateTypes().ToList();

        _playableSpecies = allSpecies.Where(species => !_nonPlayableSpecies.Contains(species)).ToList();

        foreach (var species in _playableSpecies)
        {
            var option = Instantiate(SpeciesOptionPrefab, SpeciesOptionPrefab.transform.position, Quaternion.identity);

            option.transform.SetParent(SpeciesOptionParent);

            option.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            option.transform.GetChild(0).GetComponent<Text>().text = species;
        }
    }
}
