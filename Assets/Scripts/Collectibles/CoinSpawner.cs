using System.Collections.Generic;
using UnityEngine;

/*
 * Script Name: CoinSpawner
 * Purpose: Creates a reusable pool of animated coin prefabs,
 *          spawns them in one of the three gameplay lanes,
 *          and slightly increases coin frequency as difficulty rises.
 */

public class CoinSpawner : MonoBehaviour
{
    #region Inspector Settings

    [Header("Coin Settings")]

    // Drag the blue-cube Coin prefab into this field.
    [SerializeField] private GameObject coinPrefab;

    // Number of reusable coins created when the game begins.
    [SerializeField] private int poolSize = 8;

    [Header("Spawn Settings")]

    // Starting number of seconds between coin spawns.
    [SerializeField] private float startingSpawnRate = 3f;

    // World-space Y position where coins appear.
    [SerializeField] private float spawnY = 3.5f;

    [Header("Difficulty Settings")]

    // Amount the coin spawn interval decreases per difficulty level.
    [SerializeField] private float spawnRateDecreasePerLevel = 0.2f;

    // Fastest allowed coin spawn interval.
    [SerializeField] private float minimumSpawnRate = 2f;

    #endregion

    #region Private Variables

    // Horizontal positions for the three gameplay lanes.
    private readonly float[] lanePositions = { -2f, 0f, 2f };

    // Stores the reusable coin objects.
    private readonly List<GameObject> coinPool =
        new List<GameObject>();

    // Counts time until the next coin spawn.
    private float spawnTimer;

    // Current amount of time between coin spawns.
    private float currentSpawnRate;

    // Stores the last difficulty level used to update coin spawning.
    private int lastDifficultyLevel = 1;

    // Reference to the obstacle spawner so both systems share difficulty.
    private ObstacleSpawner obstacleSpawner;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Set the starting spawn interval.
        currentSpawnRate = startingSpawnRate;

        // Find the obstacle spawner once.
        obstacleSpawner =
            FindFirstObjectByType<ObstacleSpawner>();

        // Create all pooled coins when gameplay begins.
        CreateCoinPool();
    }

    private void Update()
    {
        // Stop spawning coins after Game Over.
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        // Check whether the game difficulty has increased.
        UpdateDifficulty();

        // Count time toward the next coin spawn.
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnRate)
        {
            SpawnCoin();

            // Preserve extra elapsed time.
            spawnTimer -= currentSpawnRate;
        }
    }

    #endregion

    #region Pool Methods

    private void CreateCoinPool()
    {
        // Stop and report the problem if no prefab is assigned.
        if (coinPrefab == null)
        {
            Debug.LogError(
                "Coin Spawner: Assign the blue Coin prefab.",
                this
            );

            return;
        }

        // Make sure at least one pooled coin is created.
        poolSize = Mathf.Max(1, poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            // Create a copy of the actual Coin prefab.
            GameObject newCoin = Instantiate(
                coinPrefab,
                transform
            );

            newCoin.name =
                "Pooled Coin " + (i + 1);

            // Keep the coin inactive until it is needed.
            newCoin.SetActive(false);

            coinPool.Add(newCoin);
        }
    }

    private GameObject GetAvailableCoin()
    {
        // Find an inactive coin that can be reused.
        for (int i = 0; i < coinPool.Count; i++)
        {
            if (coinPool[i] != null &&
                !coinPool[i].activeInHierarchy)
            {
                return coinPool[i];
            }
        }

        return null;
    }

    #endregion

    #region Spawn Methods

    private void SpawnCoin()
    {
        GameObject availableCoin =
            GetAvailableCoin();

        if (availableCoin == null)
        {
            return;
        }

        // Pick one of the three lanes.
        int randomLane =
            Random.Range(0, lanePositions.Length);

        Vector3 spawnPosition = new Vector3(
            lanePositions[randomLane],
            spawnY,
            0f
        );

        // Get the collectible component.
        CoinCollectible collectible =
            availableCoin.GetComponent<CoinCollectible>();

        if (collectible != null)
        {
            // Position and activate the pooled coin.
            collectible.ActivateCoin(spawnPosition);
        }
        else
        {
            // Fallback if the prefab is missing CoinCollectible.
            availableCoin.transform.position =
                spawnPosition;

            availableCoin.SetActive(true);
        }
    }

    #endregion

    #region Difficulty Methods

    private void UpdateDifficulty()
    {
        // Stop if the obstacle spawner could not be found.
        if (obstacleSpawner == null)
        {
            return;
        }

        int currentDifficultyLevel =
            obstacleSpawner.CurrentDifficultyLevel;

        // Only update when a new difficulty level begins.
        if (currentDifficultyLevel ==
            lastDifficultyLevel)
        {
            return;
        }

        lastDifficultyLevel =
            currentDifficultyLevel;

        /*
         * Make coins spawn slightly more frequently as
         * difficulty increases.
         */
        currentSpawnRate = Mathf.Max(
            startingSpawnRate -
            ((currentDifficultyLevel - 1) *
             spawnRateDecreasePerLevel),
            minimumSpawnRate
        );
    }

    #endregion
}