using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Script Name: GameManager
 * Purpose: Controls player lives, score, game over behavior,
 *          scene restarting, and returning to the main menu.
 */

public class GameManager : MonoBehaviour
{
    #region Singleton

    // Allows other scripts to access the GameManager.
    public static GameManager Instance { get; private set; }

    #endregion

    #region Inspector Settings

    [Header("Player Settings")]

    // Number of lives the player starts with.
    [SerializeField] private int startingLives = 3;

    [Header("Lives UI")]

    // Stores the three heart images displayed on the HUD.
    [SerializeField] private Image[] heartImages;

    [Header("Score Settings")]

    // Number of points earned every second.
    [SerializeField] private float scorePerSecond = 10f;

    [Header("Score UI")]

    // Displays the score during gameplay.
    [SerializeField] private TMP_Text scoreText;

    [Header("Game Over UI")]

    // Panel displayed when the player loses all lives.
    [SerializeField] private GameObject gameOverPanel;

    // Displays the final score on the Game Over panel.
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Scene Settings")]

    // Exact name of the Main Menu scene.
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    #endregion

    #region Private Variables

    // Stores the player's current lives.
    private int currentLives;

    // Stores the player's score as a float for smooth time-based scoring.
    private float currentScore;

    // Tracks whether the game has ended.
    private bool isGameOver;

    #endregion

    #region Public Properties

    public int CurrentLives
    {
        get { return currentLives; }
    }

    public int CurrentScore
    {
        get { return Mathf.FloorToInt(currentScore); }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Prevent duplicate GameManagers.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Make sure normal time is restored when restarting the scene.
        Time.timeScale = 1f;

        // Set starting values.
        currentLives = startingLives;
        currentScore = 0f;
        isGameOver = false;

        // Hide the Game Over panel at the beginning.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateLivesUI();
        UpdateScoreUI();
    }

    private void Update()
    {
        // Stop score increases after Game Over.
        if (isGameOver)
        {
            return;
        }

        // Increase score based on survival time.
        currentScore += scorePerSecond * Time.deltaTime;

        UpdateScoreUI();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Removes one life from the player.
    /// </summary>
    public void LoseLife()
    {
        // Do nothing after Game Over or when no lives remain.
        if (isGameOver || currentLives <= 0)
        {
            return;
        }

        currentLives--;

        UpdateLivesUI();

        Debug.Log("Lives Remaining: " + currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Adds points to the player's score.
    /// This can later be used for coin collection.
    /// </summary>
    public void AddScore(int amount)
    {
        if (isGameOver)
        {
            return;
        }

        currentScore += amount;
        UpdateScoreUI();
    }

    /// <summary>
    /// Restarts the current gameplay scene.
    /// </summary>
    public void RestartGame()
    {
        // Restore normal time before loading.
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    /// <summary>
    /// Returns the player to the Main Menu.
    /// </summary>
    public void ReturnToMainMenu()
    {
        // Restore normal time before changing scenes.
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    #endregion

    #region UI Methods

    private void UpdateLivesUI()
    {
        // Stop if no heart images were assigned.
        if (heartImages == null)
        {
            return;
        }

        // Show only the hearts matching the remaining lives.
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null)
            {
                continue;
            }

            heartImages[i].gameObject.SetActive(i < currentLives);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = CurrentScore.ToString();
        }
    }

    #endregion

    #region Game State Methods

    private void GameOver()
    {
        // Prevent Game Over from running twice.
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        Debug.Log("Game Over");
        Debug.Log("Final Score: " + CurrentScore);

        // Update the final score display.
        if (finalScoreText != null)
        {
            finalScoreText.text =
                "Final Score: " + CurrentScore;
        }

        // Display the Game Over panel.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Freeze gameplay while keeping UI buttons usable.
        Time.timeScale = 0f;
    }

    #endregion
}