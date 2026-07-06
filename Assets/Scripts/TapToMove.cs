using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TapToMove : MonoBehaviour
{
    // Controls how fast the player moves toward the tapped position.
    public float moveSpeed = 5f;

    // Stores the position the player should move toward.
    private Vector3 targetPosition;

    // Tracks whether the player should currently be moving.
    private bool isMoving = false;

    private void OnEnable()
    {
        // Enables enhanced touch support for mobile touch input.
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        // Disables enhanced touch support when this script is turned off.
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        // Starts the target position at the player's current position.
        targetPosition = transform.position;
    }

    private void Update()
    {
        // Mouse input for Unity Editor testing.
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetTargetPosition(Mouse.current.position.ReadValue());
        }

        // Real mobile touch input.
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                SetTargetPosition(touch.screenPosition);
            }
        }

        if (isMoving)
        {
            MovePlayer();
        }
    }

    private void SetTargetPosition(Vector2 screenPosition)
    {
        // Converts the mouse or touch screen position into a Unity world position.
        targetPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // Keeps the player on the 2D plane.
        targetPosition.z = 0f;

        isMoving = true;
    }

    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
        }
    }
}