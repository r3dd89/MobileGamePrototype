using UnityEngine;
using UnityEngine.UI;

/*
 * Script Name: GameManager
 * Purpose: Keeps track of the player's lives, updates the heart icons,
 *          and controls the overall game state.
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

    #endregion

    #region Private Variables

    // Stores the player's current number of lives.
    private int currentLives;

    // Tracks whether the game has ended.
    private bool isGameOver;

    #endregion

    #region Public Properties

    // Allows other scripts to read the current number of lives.
    public int CurrentLives
    {
        get { return currentLives; }
    }

    // Allows other scripts to check whether the game is over.
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

        // Give the player their starting number of lives.
        currentLives = startingLives;

        // Make sure the game begins in an active state.
        isGameOver = false;

        // Display all available hearts.
        UpdateLivesUI();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Removes one life from the player.
    /// </summary>
    public void LoseLife()
    {
        // Do not remove lives after the game has ended.
        if (isGameOver)
        {
            return;
        }

        // Prevent the life count from going below zero.
        if (currentLives <= 0)
        {
            return;
        }

        // Remove one life.
        currentLives--;

        // Update the hearts displayed on the HUD.
        UpdateLivesUI();

        Debug.Log("Lives Remaining: " + currentLives);

        // End the game when no lives remain.
        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    #endregion

    #region UI Methods

    /// <summary>
    /// Shows or hides heart icons based on the current life count.
    /// </summary>
    private void UpdateLivesUI()
    {
        // Stop if the heart array has not been assigned.
        if (heartImages == null)
        {
            return;
        }

        // Check every heart image.
        for (int i = 0; i < heartImages.Length; i++)
        {
            // Skip any empty array slots.
            if (heartImages[i] == null)
            {
                continue;
            }

            // Show hearts that represent remaining lives.
            heartImages[i].gameObject.SetActive(i < currentLives);
        }
    }

    #endregion

    #region Game State Methods

    /// <summary>
    /// Ends the game when the player loses all lives.
    /// </summary>
    private void GameOver()
    {
        isGameOver = true;

        Debug.Log("Game Over");

        // We will connect the Game Over panel here next.
    }

    #endregion
}