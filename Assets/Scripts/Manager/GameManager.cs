using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Script Name: GameManager
 * Purpose: Controls player lives, score, game over behavior,
 *          scene restarting, returning to the main menu,
 *          and score pulse feedback.
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

    [Header("Score Pulse Settings")]

    // Controls how large the score becomes during the pulse.
    [SerializeField] private float scorePulseScale = 1.3f;

    // Controls how quickly the score returns to its normal size.
    [SerializeField] private float scorePulseSpeed = 8f;

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

    // Stores the normal score text scale.
    private Vector3 scoreNormalScale;

    // Tracks whether the score is currently pulsing.
    private bool isScorePulsing;

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
        isScorePulsing = false;

        // Save the score text's normal scale.
        if (scoreText != null)
        {
            scoreNormalScale = scoreText.transform.localScale;
        }

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

        // Return the score to its normal size after a pulse.
        if (isScorePulsing)
        {
            UpdateScorePulse();
        }
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
    /// Used when collecting coins or other score bonuses.
    /// </summary>
    public void AddScore(int amount)
    {
        if (isGameOver)
        {
            return;
        }

        // Add the bonus points.
        currentScore += amount;

        // Update the number immediately.
        UpdateScoreUI();

        // Play the score pulse feedback.
        StartScorePulse();
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

    /// <summary>
    /// Makes the score temporarily grow after bonus points are added.
    /// </summary>
    private void StartScorePulse()
    {
        if (scoreText == null)
        {
            return;
        }

        // Make the score larger immediately.
        scoreText.transform.localScale =
            scoreNormalScale * scorePulseScale;

        isScorePulsing = true;
    }

    /// <summary>
    /// Smoothly returns the score back to its normal size.
    /// </summary>
    private void UpdateScorePulse()
    {
        if (scoreText == null)
        {
            isScorePulsing = false;
            return;
        }

        // Move the score scale back toward normal.
        scoreText.transform.localScale = Vector3.Lerp(
            scoreText.transform.localScale,
            scoreNormalScale,
            scorePulseSpeed * Time.deltaTime
        );

        // Stop updating once the score is almost normal size.
        if (Vector3.Distance(
            scoreText.transform.localScale,
            scoreNormalScale
        ) < 0.01f)
        {
            scoreText.transform.localScale = scoreNormalScale;
            isScorePulsing = false;
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverSound();
        }

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