using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

/*
 * Script Name: PlayerController
 * Purpose: Controls the player for the mobile runner prototype.
 * Features: Swipe lane movement, swipe jump, accelerometer tilt feedback, and visual feedback.
 */

public class PlayerController : MonoBehaviour
{
    #region Inspector Settings

    [Header("Lane Movement")]
    [SerializeField] private float laneMoveSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float jumpSpeed = 8f;

    [Header("Swipe Settings")]
    [SerializeField] private float minimumSwipeDistance = 80f;

    [Header("Sensor Settings")]
    [SerializeField] private float tiltSmoothing = 8f;
    [SerializeField] private float tiltVisualAmount = 20f;

    [Header("UI Feedback")]
    [SerializeField] private GameUIManager gameUIManager;

    [Header("Visual Feedback")]
    [SerializeField] private Color feedbackColor = Color.yellow;
    [SerializeField] private float feedbackTime = 0.15f;

    #endregion

    #region Private Variables

    private float[] lanePositions = new float[] { -2f, 0f, 2f };
    private int currentLane = 1;

    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;

    private bool isJumping = false;
    private float baseY;
    private float jumpTimer = 0f;

    private float smoothedTiltX;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        // Enable mobile touch support for the New Input System.
        EnhancedTouchSupport.Enable();

        // Enable the accelerometer if the device has one.
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }
    }

    private void OnDisable()
    {
        // Disable enhanced touch support when this script is turned off.
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        // Save the player's starting Y position for the jump movement.
        baseY = transform.position.y;

        // Get the SpriteRenderer so the player can flash when input happens.
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        ShowStatus("Ready");
    }

    private void Update()
    {
        HandleSwipeInput();
        MoveToLane();
        HandleJump();
        HandleTiltFeedback();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player touches an obstacle.
        if (other.CompareTag("Obstacle"))
        {
            if (isJumping)
            {
                ShowStatus("Jumped Over Obstacle");
            }
            else
            {
                ShowStatus("Hit Obstacle");
                FlashPlayer();
            }
        }
    }

    #endregion

    #region Input Methods

    private void HandleSwipeInput()
    {
        // Real mobile touch input.
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                swipeStartPosition = touch.screenPosition;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                swipeEndPosition = touch.screenPosition;
                CheckSwipe();
            }
        }

        // Mouse input for Unity Editor testing.
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                swipeStartPosition = Mouse.current.position.ReadValue();
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                swipeEndPosition = Mouse.current.position.ReadValue();
                CheckSwipe();
            }
        }
    }

    private void CheckSwipe()
    {
        // Find the direction and distance of the swipe.
        Vector2 swipeDirection = swipeEndPosition - swipeStartPosition;

        // If the swipe was too short, count it as a tap.
        if (swipeDirection.magnitude < minimumSwipeDistance)
        {
            ShowStatus("Tap");
            return;
        }

        // Check if the swipe was mostly horizontal or vertical.
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            if (swipeDirection.x > 0)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
        else
        {
            if (swipeDirection.y > 0)
            {
                Jump();
            }
            else
            {
                ShowStatus("Swipe Down");
            }
        }
    }

    #endregion

    #region Movement Methods

    private void MoveLeft()
    {
        // Move one lane left if the player is not already at the left edge.
        if (currentLane > 0)
        {
            currentLane--;
            ShowStatus("Swipe Left");
            FlashPlayer();
        }
        else
        {
            ShowStatus("Left Edge");
        }
    }

    private void MoveRight()
    {
        // Move one lane right if the player is not already at the right edge.
        if (currentLane < lanePositions.Length - 1)
        {
            currentLane++;
            ShowStatus("Swipe Right");
            FlashPlayer();
        }
        else
        {
            ShowStatus("Right Edge");
        }
    }

    private void MoveToLane()
    {
        // Move the player smoothly toward the selected lane.
        Vector3 targetPosition = transform.position;
        targetPosition.x = lanePositions[currentLane];

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            laneMoveSpeed * Time.deltaTime
        );
    }

    private void Jump()
    {
        // Start a jump if the player is not already jumping.
        if (!isJumping)
        {
            isJumping = true;
            jumpTimer = 0f;
            ShowStatus("Jump");
            FlashPlayer();
        }
    }

    private void HandleJump()
    {
        // Stop this method if the player is not jumping.
        if (!isJumping)
        {
            return;
        }

        // Use a sine wave to move the player up and back down smoothly.
        jumpTimer += Time.deltaTime * jumpSpeed;

        float jumpOffset = Mathf.Sin(jumpTimer) * jumpHeight;

        Vector3 newPosition = transform.position;
        newPosition.y = baseY + jumpOffset;
        transform.position = newPosition;

        // End the jump when the sine wave finishes.
        if (jumpTimer >= Mathf.PI)
        {
            isJumping = false;

            Vector3 finalPosition = transform.position;
            finalPosition.y = baseY;
            transform.position = finalPosition;
        }
    }

    #endregion

    #region Sensor Methods

    private void HandleTiltFeedback()
    {
        float targetTiltX = 0f;

        // Read accelerometer input from the device.
        if (Accelerometer.current != null)
        {
            targetTiltX = Accelerometer.current.acceleration.ReadValue().x;
        }

        // Smooth the accelerometer value so it does not feel shaky.
        smoothedTiltX = Mathf.Lerp(smoothedTiltX, targetTiltX, tiltSmoothing * Time.deltaTime);

        // Rotate the player slightly to show sensor feedback.
        transform.rotation = Quaternion.Euler(0f, 0f, -smoothedTiltX * tiltVisualAmount);
    }

    #endregion

    #region Feedback Methods

    private void ShowStatus(string message)
    {
        // Send player feedback to the UI manager.
        if (gameUIManager != null)
        {
            gameUIManager.ShowStatusMessage(message);
        }
    }

    private void FlashPlayer()
    {
        // Flash the player color for touch feedback.
        if (spriteRenderer != null)
        {
            spriteRenderer.color = feedbackColor;
            CancelInvoke(nameof(ResetColor));
            Invoke(nameof(ResetColor), feedbackTime);
        }
    }

    private void ResetColor()
    {
        // Return the player to the original color.
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    #endregion
}