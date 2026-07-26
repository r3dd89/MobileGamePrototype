using UnityEngine;

/*
 * Script Name: ObstacleMovement
 * Purpose: Stores and controls one pooled obstacle.
 * Optimization: This script no longer uses its own Update method.
 */

public class ObstacleMovement : MonoBehaviour
{
    #region Private Variables

    // Cached Transform reference so Unity does not repeatedly access transform.
    private Transform cachedTransform;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Save the Transform reference when the obstacle is first created.
        cachedTransform = transform;
    }

    #endregion

    #region Pool Methods

    public void ActivateObstacle(Vector3 spawnPosition)
    {
        // Place the obstacle in its selected lane.
        cachedTransform.position = spawnPosition;

        // Turn the pooled obstacle back on.
        gameObject.SetActive(true);
    }

    public void MoveObstacle(float moveSpeed, float deltaTime)
    {
        // Move the obstacle downward.
        cachedTransform.Translate(
            Vector3.down * moveSpeed * deltaTime,
            Space.World
        );
    }

    public bool HasPassedDestroyPoint(float destroyY)
    {
        // Return true after the obstacle moves below the screen.
        return cachedTransform.position.y <= destroyY;
    }

    public void DeactivateObstacle()
    {
        // Turn the obstacle off instead of destroying it.
        gameObject.SetActive(false);
    }

    #endregion
}