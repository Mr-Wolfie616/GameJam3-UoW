using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Elements")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject creditsText;
    [SerializeField] private GameObject thirdPartyText;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject winPanel;
    bool toggle;
    private void Start()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        creditsText.SetActive(true);
        thirdPartyText.SetActive(false);
        toggle = false;
    }

    public void StartGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        //Start Game
    }

    public void Credits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void Back()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void LeaveGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        mainMenuPanel.SetActive(true);
        winPanel.SetActive(false);
        deathPanel.SetActive(false);
        //Reset Game
    }

    public void ToggleShownCredits()
    {
        if (toggle == true)
        {
            creditsText.SetActive(false);
            thirdPartyText.SetActive(true);
            toggle = false;
        }
        else if (toggle == false)
        {
            creditsText.SetActive(true);
            thirdPartyText.SetActive(false);
            toggle = true;
        }
    }
}