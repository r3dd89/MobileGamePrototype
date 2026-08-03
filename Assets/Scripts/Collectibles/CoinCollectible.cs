using UnityEngine;

/*
 * Script Name: CoinCollectible
 * Purpose: Handles coin movement and collection.
 * When the player touches the coin, the score increases
 * and the coin returns to an inactive state.
 */

public class CoinCollectible : MonoBehaviour
{
    #region Inspector Settings

    [Header("Movement Settings")]

    // Controls how quickly the coin moves down the screen.
    [SerializeField] private float moveSpeed = 3.5f;

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

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache the Transform once.
        cachedTransform = transform;
    }

    private void Update()
    {
        // Stop moving after Game Over.
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        // Move the coin downward.
        cachedTransform.Translate(
            Vector3.down * moveSpeed * Time.deltaTime,
            Space.World
        );

        // Remove the coin after it leaves the screen.
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

        // Add points to the score.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        Debug.Log("Coin Collected: +" + scoreValue);

        // Hide the coin after collection.
        gameObject.SetActive(false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Positions and activates the coin for spawning.
    /// </summary>
    public void ActivateCoin(Vector3 spawnPosition)
    {
        cachedTransform.position = spawnPosition;
        gameObject.SetActive(true);
    }

    #endregion
}