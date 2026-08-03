using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

/*
 * Script Name: PlayerController
 * Purpose: Controls swipe movement, jumping, accelerometer feedback,
 *          and visual feedback for the mobile runner.
 *
 * Optimizations:
 * 1. Caches frequently used component references.
 * 2. Only performs lane movement while movement is necessary.
 * 3. Only processes jump movement while the player is jumping.
 * 4. Uses a timer instead of Invoke and CancelInvoke for color feedback.
 */

public class PlayerController : MonoBehaviour
{
    #region Inspector Settings

    [Header("Lane Movement")]

    // Controls how quickly the player moves between lanes.
    [SerializeField] private float laneMoveSpeed = 10f;

    [Header("Jump Settings")]

    // Controls how high the player jumps.
    [SerializeField] private float jumpHeight = 1.2f;

    // Controls how quickly the jump animation completes.
    [SerializeField] private float jumpSpeed = 8f;

    [Header("Swipe Settings")]

    // Minimum swipe distance required before input counts as a swipe.
    [SerializeField] private float minimumSwipeDistance = 80f;

    [Header("Sensor Settings")]

    // Controls how smoothly the accelerometer input changes.
    [SerializeField] private float tiltSmoothing = 8f;

    // Controls how much the player visually rotates from device tilt.
    [SerializeField] private float tiltVisualAmount = 20f;

    [Header("UI Feedback")]

    // Reference to the UI manager used to display status messages.
    [SerializeField] private GameUIManager gameUIManager;

    [Header("Visual Feedback")]

    // Color used when the player flashes after input or collision.
    [SerializeField] private Color feedbackColor = Color.yellow;

    // Length of time the player remains in the feedback color.
    [SerializeField] private float feedbackTime = 0.15f;

    #endregion

    #region Private Variables

    // Stores the horizontal positions of the three player lanes.
    private readonly float[] lanePositions = { -2f, 0f, 2f };

    // The player begins in the center lane.
    private int currentLane = 1;

    // Stores the X position of the lane the player is moving toward.
    private float targetLaneX;

    // Stores where a swipe begins and ends.
    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;

    // Tracks whether the player is currently moving between lanes.
    private bool isMovingBetweenLanes;

    // Tracks whether the player is currently jumping.
    private bool isJumping;

    // Tracks whether the current device has an accelerometer.
    private bool hasAccelerometer;

    // Tracks whether the player is currently flashing.
    private bool isFlashing;

    // Stores the player's normal Y position.
    private float baseY;

    // Tracks jump progress.
    private float jumpTimer;

    // Stores the smoothed accelerometer value.
    private float smoothedTiltX;

    // Tracks how long the feedback color should remain active.
    private float flashTimer;

    // Cached Transform reference used throughout the script.
    private Transform cachedTransform;

    // Cached SpriteRenderer reference used for visual feedback.
    private SpriteRenderer spriteRenderer;

    // Stores the player's original color.
    private Color originalColor;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache components once so they do not need to be searched for later.
        cachedTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // Enable Enhanced Touch support for mobile input.
        EnhancedTouchSupport.Enable();

        // Check whether the current device has an accelerometer.
        hasAccelerometer = Accelerometer.current != null;

        // Enable the accelerometer only when one is available.
        if (hasAccelerometer)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }
    }

    private void OnDisable()
    {
        // Disable Enhanced Touch support when this script is disabled.
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        // Save the player's starting Y position for jump movement.
        baseY = cachedTransform.position.y;

        // Set the player's starting target to the center lane.
        targetLaneX = lanePositions[currentLane];

        // Save the player's normal sprite color.
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Display the starting status message.
        ShowStatus("Ready");
    }

    private void Update()
    {
        // Check for touch or mouse swipe input.
        HandleSwipeInput();

        // Only calculate lane movement when the player is actually changing lanes.
        if (isMovingBetweenLanes)
        {
            MoveToLane();
        }

        // Only calculate jump movement while a jump is active.
        if (isJumping)
        {
            HandleJump();
        }

        // Only read accelerometer input when the device supports it.
        if (hasAccelerometer)
        {
            HandleTiltFeedback();
        }

        // Only update the flash timer while the player is flashing.
        if (isFlashing)
        {
            UpdateFlashTimer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore objects that are not tagged as obstacles.
        if (!other.CompareTag("Obstacle"))
        {
            return;
        }

        // Show a different message depending on whether the player is jumping.
        if (isJumping)
        {
            ShowStatus("Jumped Over Obstacle");
        }
        else
        {
            ShowStatus("Hit Obstacle");
            FlashPlayer();

            // Record that the player hit an obstacle.
            if (GameAnalyticsManager.Instance != null)
            {
                GameAnalyticsManager.Instance.TrackObstacleHit();
            }
        }
    }

    #endregion

    #region Input Methods

    private void HandleSwipeInput()
    {
        /*
         * Use the full namespace name here because Unity has two different
         * Touch types. This prevents the ambiguous reference compiler error.
         */
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch touch =
                UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

            // Save the position where the touch began.
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                swipeStartPosition = touch.screenPosition;
            }

            // Check the completed swipe when the finger leaves the screen.
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                swipeEndPosition = touch.screenPosition;
                CheckSwipe();
            }
        }

        // Allow mouse input while testing inside the Unity Editor.
        if (Mouse.current == null)
        {
            return;
        }

        // Save the mouse position when the left button is pressed.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            swipeStartPosition = Mouse.current.position.ReadValue();
        }

        // Check the completed swipe when the left mouse button is released.
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            swipeEndPosition = Mouse.current.position.ReadValue();
            CheckSwipe();
        }
    }

    private void CheckSwipe()
    {
        // Calculate the swipe direction and distance.
        Vector2 swipeDirection =
            swipeEndPosition - swipeStartPosition;

        /*
         * Compare squared distances instead of magnitude.
         * This avoids performing an unnecessary square-root calculation.
         */
        float minimumSwipeDistanceSquared =
            minimumSwipeDistance * minimumSwipeDistance;

        // Count short swipes as taps.
        if (swipeDirection.sqrMagnitude < minimumSwipeDistanceSquared)
        {
            ShowStatus("Tap");
            return;
        }

        // Determine whether the swipe was mostly horizontal or vertical.
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            // Move right when the horizontal swipe value is positive.
            if (swipeDirection.x > 0f)
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
            // Jump when the swipe moves upward.
            if (swipeDirection.y > 0f)
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
        // Stop if the player is already in the left lane.
        if (currentLane <= 0)
        {
            ShowStatus("Left Edge");
            return;
        }

        // Change the target lane.
        currentLane--;
        BeginLaneMovement();

        if (GameAnalyticsManager.Instance != null)
        {
            GameAnalyticsManager.Instance.TrackLaneChanged();
        }

        ShowStatus("Swipe Left");
        FlashPlayer();
    }

    private void MoveRight()
    {
        // Stop if the player is already in the right lane.
        if (currentLane >= lanePositions.Length - 1)
        {
            ShowStatus("Right Edge");
            return;
        }

        // Change the target lane.
        currentLane++;
        BeginLaneMovement();

        if (GameAnalyticsManager.Instance != null)
        {
            GameAnalyticsManager.Instance.TrackLaneChanged();
        }

        ShowStatus("Swipe Right");
        FlashPlayer();
    }

    private void BeginLaneMovement()
    {
        // Save the horizontal position of the new target lane.
        targetLaneX = lanePositions[currentLane];

        // Allow Update to begin processing lane movement.
        isMovingBetweenLanes = true;
    }

    private void MoveToLane()
    {
        // Get the player's current position.
        Vector3 currentPosition = cachedTransform.position;

        // Move only the X position toward the selected lane.
        currentPosition.x = Mathf.MoveTowards(
            currentPosition.x,
            targetLaneX,
            laneMoveSpeed * Time.deltaTime
        );

        // Apply the updated position.
        cachedTransform.position = currentPosition;

        // Stop movement calculations once the target lane is reached.
        if (Mathf.Approximately(currentPosition.x, targetLaneX))
        {
            isMovingBetweenLanes = false;
        }
    }

    private void Jump()
    {
        // Do not begin another jump while one is already active.
        if (isJumping)
        {
            return;
        }

        // Reset the jump values.
        isJumping = true;
        jumpTimer = 0f;


        if (GameAnalyticsManager.Instance != null)
        {
            GameAnalyticsManager.Instance.TrackPlayerJumped();
        }

        ShowStatus("Jump");
        FlashPlayer();
    }

    private void HandleJump()
    {
        // Increase jump progress using time and jump speed.
        jumpTimer += Time.deltaTime * jumpSpeed;

        // Use a sine wave to move the player upward and back down.
        float jumpOffset =
            Mathf.Sin(jumpTimer) * jumpHeight;

        // Apply the jump offset to the player's Y position.
        Vector3 currentPosition = cachedTransform.position;
        currentPosition.y = baseY + jumpOffset;
        cachedTransform.position = currentPosition;

        // Continue the jump until the sine wave reaches PI.
        if (jumpTimer < Mathf.PI)
        {
            return;
        }

        // End the jump and return the player to the starting Y position.
        isJumping = false;

        currentPosition = cachedTransform.position;
        currentPosition.y = baseY;
        cachedTransform.position = currentPosition;
    }

    #endregion

    #region Sensor Methods

    private void HandleTiltFeedback()
    {
        // Read the horizontal accelerometer value.
        float targetTiltX =
            Accelerometer.current.acceleration.ReadValue().x;

        // Smooth the sensor value so the player does not rotate sharply.
        smoothedTiltX = Mathf.Lerp(
            smoothedTiltX,
            targetTiltX,
            tiltSmoothing * Time.deltaTime
        );

        // Rotate the player for visual sensor feedback.
        cachedTransform.rotation = Quaternion.Euler(
            0f,
            0f,
            -smoothedTiltX * tiltVisualAmount
        );
    }

    #endregion

    #region Feedback Methods

    private void ShowStatus(string message)
    {
        // Send the message to the game UI manager.
        if (gameUIManager != null)
        {
            gameUIManager.ShowStatusMessage(message);
        }
    }

    private void FlashPlayer()
    {
        // Stop if the player does not have a SpriteRenderer.
        if (spriteRenderer == null)
        {
            return;
        }

        // Apply the temporary feedback color.
        spriteRenderer.color = feedbackColor;

        // Reset the timer each time the player flashes.
        flashTimer = feedbackTime;
        isFlashing = true;
    }

    private void UpdateFlashTimer()
    {
        // Count down the remaining feedback time.
        flashTimer -= Time.deltaTime;

        // Keep the feedback color active until the timer finishes.
        if (flashTimer > 0f)
        {
            return;
        }

        // Restore the original player color.
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    #endregion
}