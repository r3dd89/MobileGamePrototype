using UnityEngine;

/*
 * Script Name: ObstacleMovement
 * Purpose: Moves obstacles downward and destroys them after they leave the screen.
 */

public class ObstacleMovement : MonoBehaviour
{
    #region Inspector Settings

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Destroy Settings")]
    [SerializeField] private float destroyY = -6f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        // Move the obstacle downward.
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // Destroy the obstacle after it leaves the screen.
        if (transform.position.y <= destroyY)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}