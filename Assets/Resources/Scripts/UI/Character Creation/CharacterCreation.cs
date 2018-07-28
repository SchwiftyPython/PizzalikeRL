using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCreation : MonoBehaviour
{
    private const string MainMenuScene = "MainMenu";

    public GameObject CurrentPage;

    public GameObject SpeciesSelectPage;
    public GameObject StatPointAllocationPage;

    private void Awake()
    {
        //todo load species list
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
}
