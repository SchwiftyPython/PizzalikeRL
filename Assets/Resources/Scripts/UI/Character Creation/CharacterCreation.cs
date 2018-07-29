using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreation : MonoBehaviour
{
    private readonly List<string> _nonPlayableSpecies = new List<string>
    {
        "pepperoni worm"
    };

    private const string MainMenuScene = "MainMenu";

    private List<string> _playableSpecies;

    public GameObject CurrentPage;

    public GameObject SpeciesSelectPage;
    public GameObject StatPointAllocationPage;

    private void Awake()
    {
        LoadPlayableSpeciesList();
    }

    public void OnBackFromSpeciesSelectPage()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

    public void OnNextFromSpeciesSelectPage()
    {
        StatPointAllocationPage.SetActive(true);

        SpeciesSelectPage.SetActive(false);
    }

    private void LoadPlayableSpeciesList()
    {
        var allSpecies = EntityTemplateLoader.GetEntityTemplateTypes().ToList();

        _playableSpecies = allSpecies.Where(species => !_nonPlayableSpecies.Contains(species)).ToList();
    }
}
