using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Script Name: GameManager
 * Purpose: Controls lives, score, and the overall game state.
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

    // Number of score points earned every second.
    [SerializeField] private float scorePerSecond = 10f;

    [Header("Score UI")]

    // Text that displays the player's current score.
    [SerializeField] private TMP_Text scoreText;

    #endregion

    #region Private Variables

    // Stores the player's current number of lives.
    private int currentLives;

    // Stores the player's current score.
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
        // Prevent more than one GameManager from existing.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Reset the game values.
        currentLives = startingLives;
        currentScore = 0f;
        isGameOver = false;

        UpdateLivesUI();
        UpdateScoreUI();
    }

    private void Update()
    {
        // Stop increasing score after the game ends.
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
        // Do not remove lives after game over.
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
    /// Adds a specific amount to the player's score.
    /// This can be used later for coins or avoided obstacles.
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

    #endregion

    #region UI Methods

    private void UpdateLivesUI()
    {
        if (heartImages == null)
        {
            return;
        }

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
        isGameOver = true;

        Debug.Log("Game Over");
        Debug.Log("Final Score: " + CurrentScore);

        // Game Over panel will be added next.
    }

    #endregion
}