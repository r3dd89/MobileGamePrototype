using System.Collections.Generic;
using UnityEngine;

/*
 * Script Name: ObstacleSpawner
 * Purpose: Creates a reusable obstacle pool, spawns obstacles,
 *          and moves all active obstacles from one Update method.
 *
 * Optimizations:
 * 1. Obstacles are created once and reused.
 * 2. Instantiate and Destroy are not used during normal gameplay.
 * 3. Obstacle movement is handled by one centralized Update method.
 */

public class ObstacleSpawner : MonoBehaviour
{
    #region Inspector Settings

    [Header("Obstacle Settings")]

    // Prefab used to create the obstacle pool.
    [SerializeField] private GameObject obstaclePrefab;

    // Number of obstacles created before gameplay begins.
    [SerializeField] private int poolSize = 8;

    [Header("Spawn Settings")]

    // Amount of time between obstacle spawns.
    [SerializeField] private float spawnRate = 1.5f;

    // Vertical position where obstacles appear.
    [SerializeField] private float spawnY = 5.5f;

    [Header("Movement Settings")]

    // Speed applied to every active obstacle.
    [SerializeField] private float obstacleMoveSpeed = 3.5f;

    // Obstacles are returned to the pool below this position.
    [SerializeField] private float returnToPoolY = -6f;

    #endregion

    #region Private Variables

    // The three horizontal lane positions.
    private readonly float[] lanePositions = { -2f, 0f, 2f };

    // Stores inactive obstacles that are ready to be reused.
    private readonly Queue<ObstacleMovement> availableObstacles =
        new Queue<ObstacleMovement>();

    // Stores obstacles that are currently moving on the screen.
    private readonly List<ObstacleMovement> activeObstacles =
        new List<ObstacleMovement>();

    private float spawnTimer;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        CreateObstaclePool();
    }

    private void Update()
    {
        UpdateSpawnTimer();
        MoveActiveObstacles();
    }

    #endregion

    #region Pool Methods

    private void CreateObstaclePool()
    {
        // Stop if the prefab was not assigned.
        if (obstaclePrefab == null)
        {
            Debug.LogError(
                "ObstacleSpawner requires an obstacle prefab.",
                this
            );

            return;
        }

        // Make sure the pool contains at least one obstacle.
        poolSize = Mathf.Max(1, poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            // Obstacles are created here once before regular gameplay.
            GameObject obstacleObject = Instantiate(
                obstaclePrefab,
                transform
            );

            // Look for the required movement component.
            ObstacleMovement obstacleMovement =
                obstacleObject.GetComponent<ObstacleMovement>();

            if (obstacleMovement == null)
            {
                Debug.LogError(
                    "The obstacle prefab needs an ObstacleMovement component.",
                    obstacleObject
                );

                Destroy(obstacleObject);
                continue;
            }

            // Keep the pooled objects organized under the spawner.
            obstacleObject.name = $"Pooled Obstacle {i + 1}";

            // Hide the obstacle until the spawner needs it.
            obstacleMovement.DeactivateObstacle();

            // Add it to the available pool.
            availableObstacles.Enqueue(obstacleMovement);
        }
    }

    private void ReturnObstacleToPool(
        ObstacleMovement obstacle,
        int activeListIndex
    )
    {
        // Remove the obstacle from the active list.
        activeObstacles.RemoveAt(activeListIndex);

        // Hide it instead of destroying it.
        obstacle.DeactivateObstacle();

        // Make it available for a future spawn.
        availableObstacles.Enqueue(obstacle);
    }

    #endregion

    #region Spawn Methods

    private void UpdateSpawnTimer()
    {
        // Stop if the pool was not created successfully.
        if (availableObstacles.Count == 0 &&
            activeObstacles.Count == 0)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnObstacle();

            // Preserve extra elapsed time instead of always resetting to zero.
            spawnTimer -= spawnRate;
        }
    }

    private void SpawnObstacle()
    {
        // Do not instantiate during gameplay if every pooled obstacle is active.
        if (availableObstacles.Count == 0)
        {
            return;
        }

        // Choose one of the three lanes.
        int randomLane = Random.Range(0, lanePositions.Length);

        Vector3 spawnPosition = new Vector3(
            lanePositions[randomLane],
            spawnY,
            0f
        );

        // Take an inactive obstacle from the pool.
        ObstacleMovement obstacle = availableObstacles.Dequeue();

        // Position and display it.
        obstacle.ActivateObstacle(spawnPosition);

        // Add it to the active movement list.
        activeObstacles.Add(obstacle);
    }

    #endregion

    #region Movement Methods

    private void MoveActiveObstacles()
    {
        // Move backward through the list because obstacles may be removed.
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            ObstacleMovement obstacle = activeObstacles[i];

            obstacle.MoveObstacle(
                obstacleMoveSpeed,
                Time.deltaTime
            );

            // Return the obstacle after it leaves the screen.
            if (obstacle.HasPassedDestroyPoint(returnToPoolY))
            {
                ReturnObstacleToPool(obstacle, i);
            }
        }
    }

    #endregion
}