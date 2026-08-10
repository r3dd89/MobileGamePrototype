using UnityEngine;

/*
 * Script Name: EnvironmentScroller
 * Purpose: Creates the illusion of forward movement by scrolling
 *          two environment sections downward and recycling them.
 *          The scroll speed increases with gameplay difficulty.
 */

public class EnvironmentScroller : MonoBehaviour
{
    #region Inspector Settings

    [Header("Environment Sections")]

    // First repeating environment section.
    [SerializeField] private Transform sectionOne;

    // Second repeating environment section.
    [SerializeField] private Transform sectionTwo;

    [Header("Scrolling Settings")]

    // Height of one complete environment section.
    [SerializeField] private float sectionHeight = 10f;

    // Multiplier applied to the current gameplay speed.
    [SerializeField] private float scrollSpeedMultiplier = 1f;

    #endregion

    #region Private Variables

    // Reference used so the environment can match difficulty speed.
    private ObstacleSpawner obstacleSpawner;

    // Fallback speed if the obstacle spawner cannot be found.
    private const float fallbackScrollSpeed = 3.5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Find the obstacle spawner once when gameplay begins.
        obstacleSpawner =
            FindFirstObjectByType<ObstacleSpawner>();
    }

    private void Update()
    {
        // Stop movement after Game Over.
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        // Determine the current scrolling speed.
        float currentScrollSpeed = fallbackScrollSpeed;

        if (obstacleSpawner != null)
        {
            currentScrollSpeed =
                obstacleSpawner.CurrentObstacleSpeed;
        }

        currentScrollSpeed *= scrollSpeedMultiplier;

        // Move both environment sections downward.
        MoveSection(sectionOne, currentScrollSpeed);
        MoveSection(sectionTwo, currentScrollSpeed);

        // Recycle sections after they move below the screen.
        RecycleSection(sectionOne, sectionTwo);
        RecycleSection(sectionTwo, sectionOne);
    }

    #endregion

    #region Scrolling Methods

    private void MoveSection(
        Transform section,
        float scrollSpeed
    )
    {
        if (section == null)
        {
            return;
        }

        section.Translate(
            Vector3.down * scrollSpeed * Time.deltaTime,
            Space.World
        );
    }

    private void RecycleSection(
        Transform section,
        Transform otherSection
    )
    {
        if (section == null || otherSection == null)
        {
            return;
        }

        // Move the section back above the other section
        // after it completely leaves the bottom of the screen.
        if (section.position.y <= -sectionHeight)
        {
            Vector3 newPosition = section.position;

            newPosition.y =
                otherSection.position.y + sectionHeight;

            section.position = newPosition;
        }
    }

    #endregion
}