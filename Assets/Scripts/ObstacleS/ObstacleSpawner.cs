using System.Collections.Generic;
using UnityEngine;

/*
 * Script Name: ObstacleSpawner
 * Purpose: Creates a reusable obstacle pool, spawns obstacles,
 *          moves all active obstacles, and gradually increases
 *          gameplay difficulty over time.
 *
 * Optimizations:
 * 1. Obstacles are created once and reused.
 * 2. Instantiate and Destroy are not used during normal gameplay.
 * 3. Obstacle movement is handled by one centralized Update method.
 * 4. Difficulty values are updated only when a new difficulty level begins.
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

    // Starting amount of time between obstacle spawns.
    [SerializeField] private float spawnRate = 1.5f;

    // Vertical position where obstacles appear.
    [SerializeField] private float spawnY = 5.5f;

    [Header("Movement Settings")]

    // Starting speed applied to every active obstacle.
    [SerializeField] private float obstacleMoveSpeed = 3.5f;

    // Obstacles are returned to the pool below this position.
    [SerializeField] private float returnToPoolY = -6f;

    [Header("Difficulty Settings")]

    // Number of seconds before the difficulty increases.
    [SerializeField] private float difficultyIncreaseInterval = 20f;

    // Amount added to obstacle speed each difficulty level.
    [SerializeField] private float speedIncreasePerLevel = 0.5f;

    // Amount removed from the spawn interval each difficulty level.
    [SerializeField] private float spawnRateDecreasePerLevel = 0.1f;

    // Fastest allowed obstacle spawn interval.
    [SerializeField] private float minimumSpawnRate = 1.1f;

    // Maximum obstacle movement speed.
    [SerializeField] private float maximumObstacleSpeed = 5.5f;

    // Highest difficulty level the game can reach.
    [SerializeField] private int maximumDifficultyLevel = 5;

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

    // Counts time until the next obstacle spawn.
    private float spawnTimer;

    // Counts time until the next difficulty increase.
    private float difficultyTimer;

    // Current difficulty level.
    private int currentDifficultyLevel = 1;

    #endregion

    #region Public Properties

    // Allows other scripts to read the current difficulty level later.
    public int CurrentDifficultyLevel
    {
        get { return currentDifficultyLevel; }
    }

    // Allows other systems, such as coins, to match obstacle speed later.
    public float CurrentObstacleSpeed
    {
        get { return obstacleMoveSpeed; }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Create all reusable obstacles before gameplay begins.
        CreateObstaclePool();
    }

    private void Update()
    {
        // Stop processing after Game Over.
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        // Check whether the difficulty should increase.
        UpdateDifficulty();

        // Handle obstacle spawning.
        UpdateSpawnTimer();

        // Move all active obstacles.
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

            // Keep pooled objects organized under the spawner.
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

        // Count time toward the next spawn.
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnObstacle();

            // Preserve extra elapsed time.
            spawnTimer -= spawnRate;
        }
    }

    private void SpawnObstacle()
    {
        // Do not create extra objects if every pooled obstacle is active.
        if (availableObstacles.Count == 0)
        {
            return;
        }

        // Choose one of the three lanes.
        int randomLane =
            Random.Range(0, lanePositions.Length);

        Vector3 spawnPosition = new Vector3(
            lanePositions[randomLane],
            spawnY,
            0f
        );

        // Take an inactive obstacle from the pool.
        ObstacleMovement obstacle =
            availableObstacles.Dequeue();

        // Position and display it.
        obstacle.ActivateObstacle(spawnPosition);

        // Add it to the active movement list.
        activeObstacles.Add(obstacle);
    }

    #endregion

    #region Movement Methods

    private void MoveActiveObstacles()
    {
        // Move backward because obstacles may be removed from the list.
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            ObstacleMovement obstacle =
                activeObstacles[i];

            // Move the obstacle using the current difficulty speed.
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

    #region Difficulty Methods

    private void UpdateDifficulty()
    {
        // Stop increasing difficulty after reaching the maximum level.
        if (currentDifficultyLevel >= maximumDifficultyLevel)
        {
            return;
        }

        // Count survival time.
        difficultyTimer += Time.deltaTime;

        // Wait until enough time has passed.
        if (difficultyTimer < difficultyIncreaseInterval)
        {
            return;
        }

        // Reset the timer for the next difficulty level.
        difficultyTimer -= difficultyIncreaseInterval;

        IncreaseDifficulty();
    }

    private void IncreaseDifficulty()
    {
        // Increase the difficulty level.
        currentDifficultyLevel++;

        // Increase obstacle movement speed.
        obstacleMoveSpeed = Mathf.Min(
            obstacleMoveSpeed + speedIncreasePerLevel,
            maximumObstacleSpeed
        );

        // Reduce the time between obstacle spawns.
        spawnRate = Mathf.Max(
            spawnRate - spawnRateDecreasePerLevel,
            minimumSpawnRate
        );

        // Display testing information in the Console.
        Debug.Log(
            "Difficulty Level: " + currentDifficultyLevel +
            " | Obstacle Speed: " + obstacleMoveSpeed +
            " | Spawn Rate: " + spawnRate
        );
    }

    #endregion
}