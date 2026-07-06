using UnityEngine;

public class TapToMove : MonoBehaviour
{
    // Controls how fast the player moves toward the tapped position.
    public float moveSpeed = 5f;

    // Stores the position the player should move toward.
    private Vector3 targetPosition;

    // Tracks whether the player should currently be moving.
    private bool isMoving = false;

    private void Start()
    {
        // Start the target position at the player's current position.
        targetPosition = transform.position;
    }

    private void Update()
    {
        // Check for mouse input in the Unity Editor.
        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPosition(Input.mousePosition);
        }

        // Check for touch input on a mobile device.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                SetTargetPosition(touch.position);
            }
        }

        // Move the player if a target position has been selected.
        if (isMoving)
        {
            MovePlayer();
        }
    }

    private void SetTargetPosition(Vector3 screenPosition)
    {
        // Convert the mouse or touch position from screen space to world space.
        targetPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // Keep the player on the 2D plane.
        targetPosition.z = 0f;

        // Allow the player to start moving.
        isMoving = true;
    }

    private void MovePlayer()
    {
        // Move the player toward the target position.
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Stop moving when the player reaches the target.
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
        }
    }
}