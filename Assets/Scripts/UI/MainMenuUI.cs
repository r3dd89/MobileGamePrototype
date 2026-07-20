using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script controls the buttons on the main menu.
/// It lets the player start the game, open settings, or quit.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    #region Inspector Variables

    [Header("Menu Panels")]

    // This is the panel that holds the main menu buttons.
    [SerializeField] private GameObject mainMenuPanel;

    // This is the panel that holds the settings options.
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Settings")]

    // This stores the name of the gameplay scene that will be loaded.
    [SerializeField] private string gameplaySceneName = "Main";

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Shows the main menu when the scene first starts.
        mainMenuPanel.SetActive(true);

        // Hides the settings panel until the player opens it.
        settingsPanel.SetActive(false);
    }

    #endregion

    #region Button Methods

    public void PlayGame()
    {
        // Loads the gameplay scene when the Play button is pressed.
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OpenSettings()
    {
        // Hides the main menu buttons.
        mainMenuPanel.SetActive(false);

        // Shows the settings panel.
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        // Hides the settings panel.
        settingsPanel.SetActive(false);

        // Shows the main menu buttons again.
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        // Closes the game when it is running as a build.
        Application.Quit();
    }

    #endregion
}