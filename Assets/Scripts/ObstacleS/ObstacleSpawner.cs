using UnityEngine;

/*
 * Script Name: ObstacleSpawner
 * Purpose: Spawns obstacles in one of three lanes.
 */

public class ObstacleSpawner : MonoBehaviour
{
    #region Inspector Settings

    [Header("Obstacle Settings")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRate = 1.5f;
    [SerializeField] private float spawnY = 5.5f;

    #endregion

    #region Private Variables

    private float[] lanePositions = new float[] { -2f, 0f, 2f };
    private float spawnTimer;

    #endregion

    #region Unity Methods

    private void Update()
    {
        // Count time until the next obstacle should spawn.
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }
    }

    #endregion

    #region Spawn Methods

    private void SpawnObstacle()
    {
        // Stop if there is no prefab assigned.
        if (obstaclePrefab == null)
        {
            return;
        }

        // Pick one of the three lanes randomly.
        int randomLane = Random.Range(0, lanePositions.Length);

        Vector3 spawnPosition = new Vector3(
            lanePositions[randomLane],
            spawnY,
            0f
        );

        // Create the obstacle in the selected lane.
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }

    #endregion
}