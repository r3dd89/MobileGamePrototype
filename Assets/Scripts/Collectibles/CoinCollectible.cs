using UnityEngine;

/*
 * Script Name: CoinCollectible
 * Purpose: Handles coin movement and collection.
 *          Coins match the current obstacle speed so they stay
 *          synchronized with the game's difficulty progression.
 *          When collected, the coin adds score and returns to the pool.
 */

public class CoinCollectible : MonoBehaviour
{
    #region Inspector Settings

    [Header("Movement Settings")]

    // Fallback movement speed used if the ObstacleSpawner cannot be found.
    [SerializeField] private float fallbackMoveSpeed = 3.5f;

    [Header("Score Settings")]

    // Number of points awarded when the coin is collected.
    [SerializeField] private int scoreValue = 10;

    [Header("Despawn Settings")]

    // Position where the coin is removed after leaving the screen.
    [SerializeField] private float despawnY = -6f;

    #endregion

    #region Private Variables

    // Cached Transform reference used for movement.
    private Transform cachedTransform;

    // Reference to the obstacle spawner so coins can match game speed.
    private ObstacleSpawner obstacleSpawner;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache the Transform once.
        cachedTransform = transform;

        /*
         * Find the ObstacleSpawner once when the coin is created.
         * This lets the coin use the same movement speed as the obstacles
         * without searching for the spawner every frame.
         */
        obstacleSpawner =
            FindFirstObjectByType<ObstacleSpawner>();
    }

    private void Update()
    {
        // Stop moving after Game Over.
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        // Start with the fallback speed.
        float currentMoveSpeed = fallbackMoveSpeed;

        /*
         * If the obstacle spawner exists, use its current movement speed.
         * This automatically makes coins move faster as difficulty increases.
         */
        if (obstacleSpawner != null)
        {
            currentMoveSpeed =
                obstacleSpawner.CurrentObstacleSpeed;
        }

        // Move the coin downward.
        cachedTransform.Translate(
            Vector3.down * currentMoveSpeed * Time.deltaTime,
            Space.World
        );

        // Return the coin to the pool after it leaves the screen.
        if (cachedTransform.position.y <= despawnY)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only respond to the Player.
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Add points to the player's score.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinSound();
        }

        // Return the coin to the pool after collection.
        gameObject.SetActive(false);

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Positions and activates the coin for spawning.
    /// </summary>
    public void ActivateCoin(Vector3 spawnPosition)
    {
        // Move the pooled coin to its new lane position.
        cachedTransform.position = spawnPosition;

        // Activate the coin.
        gameObject.SetActive(true);
    }

    #endregion
}