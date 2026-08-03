using UnityEngine;

/*
 * Script Name: GameManager
 * Purpose: Keeps track of the player's lives and overall game state.
 */

public class GameManager : MonoBehaviour
{
    #region Singleton

    // Allows other scripts to easily access the GameManager.
    public static GameManager Instance;

    #endregion

    #region Inspector Settings

    [Header("Player Settings")]

    // Number of lives the player starts with.
    [SerializeField] private int startingLives = 3;

    #endregion

    #region Private Variables

    // Current number of lives remaining.
    private int currentLives;

    #endregion

    #region Public Properties

    // Allows other scripts to read the current lives.
    public int CurrentLives
    {
        get { return currentLives; }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Create the singleton.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLives = startingLives;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Removes one life from the player.
    /// </summary>
    public void LoseLife()
    {
        currentLives--;

        Debug.Log("Lives Remaining: " + currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    #endregion

    #region Private Methods

    private void GameOver()
    {
        Debug.Log("Game Over");

        // We'll add the Game Over screen later.
    }

    #endregion
}