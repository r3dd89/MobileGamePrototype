using System.Collections.Generic;
using UnityEngine;

/*
 * Script Name: CoinSpawner
 * Purpose: Creates a reusable pool of animated coin prefabs
 *          and spawns them in one of the three gameplay lanes.
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

    // Number of seconds between each coin spawn.
    [SerializeField] private float spawnRate = 3f;

    // World-space Y position where coins appear.
    [SerializeField] private float spawnY = 3.5f;

    #endregion

    #region Private Variables

    // Horizontal positions for the three gameplay lanes.
    private readonly float[] lanePositions = { -2f, 0f, 2f };

    // Stores the reusable coin objects.
    private readonly List<GameObject> coinPool =
        new List<GameObject>();

    // Counts time until the next coin spawn.
    private float spawnTimer;

    #endregion

    #region Unity Methods

    private void Start()
    {
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

        // Count time toward the next spawn.
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnCoin();
            spawnTimer = 0f;
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

            newCoin.name = "Pooled Coin " + (i + 1);

            // Keep the coin inactive until it is needed.
            newCoin.SetActive(false);

            coinPool.Add(newCoin);
        }

        Debug.Log(
            "Coin pool created: " + coinPool.Count,
            this
        );
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
        GameObject availableCoin = GetAvailableCoin();

        if (availableCoin == null)
        {
            Debug.LogWarning("Coin Spawner: No pooled coin is available.", this);
            return;
        }

        int randomLane = Random.Range(0, lanePositions.Length);
        Vector3 spawnPosition = new Vector3(
            lanePositions[randomLane],
            spawnY,
            0f
        );

        // Call ActivateCoin directly on the collectible component
        CoinCollectible collectible = availableCoin.GetComponent<CoinCollectible>();
        if (collectible != null)
        {
            collectible.ActivateCoin(spawnPosition);
        }
        else
        {
            // Fallback if missing component
            availableCoin.transform.position = spawnPosition;
            availableCoin.SetActive(true);
        }

        Debug.Log("Coin spawned at: " + spawnPosition, availableCoin);
    }

    #endregion
}