using System.Collections.Generic;
using UnityEngine;

/*
 * Script Name: ObstacleSpawner
 * Purpose: Creates a reusable obstacle pool, spawns obstacles,
 *          increases difficulty over time, prevents excessive
 *          lane repeats, and introduces double-obstacle patterns.
 *
 * Optimizations:
 * 1. Obstacles are created once and reused.
 * 2. Instantiate and Destroy are not used during normal gameplay.
 * 3. Obstacle movement is handled by one centralized Update method.
 * 4. Difficulty values are updated only when a new difficulty level begins.
 * 5. Spawn patterns use pooled obstacles only.
 */

public class ObstacleSpawner : MonoBehaviour
{
    #region Inspector Settings

    [Header("Obstacle Settings")]

    // Prefab used to create the obstacle pool.
    [SerializeField] private GameObject obstaclePrefab;

    // Number of obstacles created before gameplay begins.
    [SerializeField] private int poolSize = 10;

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

    [Header("Lane Variety Settings")]

    // Maximum number of times the same lane may be used in a row.
    [SerializeField] private int maximumSameLaneSpawns = 2;

    [Header("Difficulty Settings")]

    // Number of seconds before the difficulty increases.
    [SerializeField] private float difficultyIncreaseInterval = 15f;

    // Amount added to obstacle speed each difficulty level.
    [SerializeField] private float speedIncreasePerLevel = 0.6f;

    // Amount removed from the spawn interval each difficulty level.
    [SerializeField] private float spawnRateDecreasePerLevel = 0.12f;

    // Fastest allowed obstacle spawn interval.
    [SerializeField] private float minimumSpawnRate = 0.8f;

    // Maximum obstacle movement speed.
    [SerializeField] private float maximumObstacleSpeed = 6.5f;

    // Highest difficulty level the game can reach.
    [SerializeField] private int maximumDifficultyLevel = 6;

    [Header("Pattern Settings")]

    // Chance of spawning a double obstacle at level 2.
    [Range(0f, 1f)]
    [SerializeField] private float startingDoubleObstacleChance = 0.15f;

    // Additional chance added per difficulty level.
    [Range(0f, 1f)]
    [SerializeField] private float doubleObstacleChanceIncrease = 0.10f;

    // Maximum chance of spawning a double obstacle.
    [Range(0f, 1f)]
    [SerializeField] private float maximumDoubleObstacleChance = 0.55f;

    #endregion

    #region Private Variables

    // The three horizontal lane positions.
    private readonly float[] lanePositions = { -2f, 0f, 2f };

    // Stores inactive obstacles ready for reuse.
    private readonly Queue<ObstacleMovement> availableObstacles =
        new Queue<ObstacleMovement>();

    // Stores obstacles currently moving on screen.
    private readonly List<ObstacleMovement> activeObstacles =
        new List<ObstacleMovement>();

    // Counts time until the next obstacle spawn.
    private float spawnTimer;

    // Counts time until the next difficulty increase.
    private float difficultyTimer;

    // Current difficulty level.
    private int currentDifficultyLevel = 1;

    // Stores the previously selected lane.
    private int lastSpawnedLane = -1;

    // Counts repeated single-lane spawns.
    private int sameLaneSpawnCount = 0;

    #endregion

    #region Public Properties

    public int CurrentDifficultyLevel
    {
        get { return currentDifficultyLevel; }
    }

    public float CurrentObstacleSpeed
    {
        get { return obstacleMoveSpeed; }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        CreateObstaclePool();
    }

    private void Update()
    {
        // Stop gameplay processing after Game Over.
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        UpdateDifficulty();
        UpdateSpawnTimer();
        MoveActiveObstacles();
    }

    #endregion

    #region Pool Methods

    private void CreateObstaclePool()
    {
        if (obstaclePrefab == null)
        {
            return;
        }

        poolSize = Mathf.Max(1, poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obstacleObject = Instantiate(
                obstaclePrefab,
                transform
            );

            ObstacleMovement obstacleMovement =
                obstacleObject.GetComponent<ObstacleMovement>();

            if (obstacleMovement == null)
            {
                Destroy(obstacleObject);
                continue;
            }

            obstacleObject.name =
                $"Pooled Obstacle {i + 1}";

            obstacleMovement.DeactivateObstacle();

            availableObstacles.Enqueue(obstacleMovement);
        }
    }

    private void ReturnObstacleToPool(
        ObstacleMovement obstacle,
        int activeListIndex
    )
    {
        activeObstacles.RemoveAt(activeListIndex);

        obstacle.DeactivateObstacle();

        availableObstacles.Enqueue(obstacle);
    }

    #endregion

    #region Spawn Methods

    private void UpdateSpawnTimer()
    {
        if (availableObstacles.Count == 0 &&
            activeObstacles.Count == 0)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnPattern();

            spawnTimer -= spawnRate;
        }
    }

    private void SpawnPattern()
    {
        // Level 1 uses only single obstacles.
        if (currentDifficultyLevel <= 1)
        {
            SpawnSingleObstacle();
            return;
        }

        float currentDoubleChance =
            startingDoubleObstacleChance +
            ((currentDifficultyLevel - 2) *
             doubleObstacleChanceIncrease);

        currentDoubleChance = Mathf.Min(
            currentDoubleChance,
            maximumDoubleObstacleChance
        );

        // Attempt a double pattern when enough pooled obstacles are free.
        if (Random.value < currentDoubleChance &&
            availableObstacles.Count >= 2)
        {
            SpawnDoubleObstacle();
        }
        else
        {
            SpawnSingleObstacle();
        }
    }

    private void SpawnSingleObstacle()
    {
        if (availableObstacles.Count == 0)
        {
            return;
        }

        int selectedLane = ChooseSingleSpawnLane();

        SpawnObstacleInLane(selectedLane);
    }

    private void SpawnDoubleObstacle()
    {
        if (availableObstacles.Count < 2)
        {
            SpawnSingleObstacle();
            return;
        }

        /*
         * Pick ONE safe lane.
         * Obstacles spawn in the other two lanes.
         */
        int safeLane =
            Random.Range(0, lanePositions.Length);

        for (int lane = 0;
             lane < lanePositions.Length;
             lane++)
        {
            if (lane == safeLane)
            {
                continue;
            }

            SpawnObstacleInLane(lane);
        }

        /*
         * Reset single-lane repeat tracking because
         * a multi-lane pattern just occurred.
         */
        lastSpawnedLane = -1;
        sameLaneSpawnCount = 0;
    }

    private int ChooseSingleSpawnLane()
    {
        int selectedLane =
            Random.Range(0, lanePositions.Length);

        // Force a different lane after too many repeats.
        if (selectedLane == lastSpawnedLane &&
            sameLaneSpawnCount >= maximumSameLaneSpawns)
        {
            do
            {
                selectedLane =
                    Random.Range(0, lanePositions.Length);
            }
            while (selectedLane == lastSpawnedLane);
        }

        if (selectedLane == lastSpawnedLane)
        {
            sameLaneSpawnCount++;
        }
        else
        {
            lastSpawnedLane = selectedLane;
            sameLaneSpawnCount = 1;
        }

        return selectedLane;
    }

    private void SpawnObstacleInLane(int laneIndex)
    {
        if (availableObstacles.Count == 0)
        {
            return;
        }

        Vector3 spawnPosition = new Vector3(
            lanePositions[laneIndex],
            spawnY,
            0f
        );

        ObstacleMovement obstacle =
            availableObstacles.Dequeue();

        obstacle.ActivateObstacle(spawnPosition);

        activeObstacles.Add(obstacle);
    }

    #endregion

    #region Movement Methods

    private void MoveActiveObstacles()
    {
        for (int i = activeObstacles.Count - 1;
             i >= 0;
             i--)
        {
            ObstacleMovement obstacle =
                activeObstacles[i];

            obstacle.MoveObstacle(
                obstacleMoveSpeed,
                Time.deltaTime
            );

            if (obstacle.HasPassedDestroyPoint(
                returnToPoolY))
            {
                ReturnObstacleToPool(obstacle, i);
            }
        }
    }

    #endregion

    #region Difficulty Methods

    private void UpdateDifficulty()
    {
        if (currentDifficultyLevel >=
            maximumDifficultyLevel)
        {
            return;
        }

        difficultyTimer += Time.deltaTime;

        if (difficultyTimer <
            difficultyIncreaseInterval)
        {
            return;
        }

        difficultyTimer -=
            difficultyIncreaseInterval;

        IncreaseDifficulty();
    }

    private void IncreaseDifficulty()
    {
        currentDifficultyLevel++;

        obstacleMoveSpeed = Mathf.Min(
            obstacleMoveSpeed + speedIncreasePerLevel,
            maximumObstacleSpeed
        );

        spawnRate = Mathf.Max(
            spawnRate - spawnRateDecreasePerLevel,
            minimumSpawnRate
        );
    }

    #endregion
}