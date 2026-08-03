using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

/*
 * Script Name: PlayerController
 * Purpose: Controls swipe movement, jumping, accelerometer feedback,
 *          player damage, analytics, and visual feedback.
 *
 * Optimizations:
 * 1. Caches frequently used component references.
 * 2. Only performs lane movement while movement is necessary.
 * 3. Only processes jump movement while the player is jumping.
 * 4. Uses timers instead of Invoke and CancelInvoke.
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

    // Controls how quickly the jump completes.
    [SerializeField] private float jumpSpeed = 8f;

    [Header("Swipe Settings")]

    // Minimum swipe distance required before input counts as a swipe.
    [SerializeField] private float minimumSwipeDistance = 80f;

    [Header("Sensor Settings")]

    // Controls how smoothly accelerometer input changes.
    [SerializeField] private float tiltSmoothing = 8f;

    // Controls how much the player rotates from device tilt.
    [SerializeField] private float tiltVisualAmount = 20f;

    [Header("UI Feedback")]

    // Reference used to display temporary gameplay messages.
    [SerializeField] private GameUIManager gameUIManager;

    [Header("Visual Feedback")]

    // Temporary color used after input or damage.
    [SerializeField] private Color feedbackColor = Color.yellow;

    // Length of time the temporary color remains active.
    [SerializeField] private float feedbackTime = 0.15f;

    [Header("Damage Settings")]

    // Time after damage during which another life cannot be lost.
    [SerializeField] private float invincibilityDuration = 1f;

    #endregion

    #region Private Variables

    // Horizontal positions for the left, center, and right lanes.
    private readonly float[] lanePositions = { -2f, 0f, 2f };

    // Player begins in the center lane.
    private int currentLane = 1;

    // X position of the selected lane.
    private float targetLaneX;

    // Positions used to calculate swipe direction.
    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;

    // Gameplay state values.
    private bool isMovingBetweenLanes;
    private bool isJumping;
    private bool hasAccelerometer;
    private bool isFlashing;
    private bool isInvincible;

    // Jump, sensor, feedback, and damage timers.
    private float baseY;
    private float jumpTimer;
    private float smoothedTiltX;
    private float flashTimer;
    private float invincibilityTimer;

    // Cached component references.
    private Transform cachedTransform;
    private SpriteRenderer spriteRenderer;

    // Original player sprite color.
    private Color originalColor;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache components once.
        cachedTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // Enable mobile Enhanced Touch input.
        EnhancedTouchSupport.Enable();

        // Check for an accelerometer.
        hasAccelerometer = Accelerometer.current != null;

        if (hasAccelerometer)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }
    }

    private void OnDisable()
    {
        // Disable Enhanced Touch when the script is disabled.
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        // Save starting values.
        baseY = cachedTransform.position.y;
        targetLaneX = lanePositions[currentLane];

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        ShowStatus("Ready");
    }

    private void Update()
    {
        // Stop gameplay input after the game is over.
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        HandleSwipeInput();

        if (isMovingBetweenLanes)
        {
            MoveToLane();
        }

        if (isJumping)
        {
            HandleJump();
        }

        if (hasAccelerometer)
        {
            HandleTiltFeedback();
        }

        if (isFlashing)
        {
            UpdateFlashTimer();
        }

        if (isInvincible)
        {
            UpdateInvincibilityTimer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore anything that is not an obstacle.
        if (!other.CompareTag("Obstacle"))
        {
            return;
        }

        // Ignore additional collisions during invincibility.
        if (isInvincible)
        {
            return;
        }

        // Treat obstacles as avoided while jumping.
        if (isJumping)
        {
            ShowStatus("Jumped Over Obstacle");
            return;
        }

        // Remove one life.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoseLife();
        }

        ShowStatus("Hit Obstacle");
        FlashPlayer();

        // Start temporary invincibility.
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        // Record the collision in Analytics.
        if (GameAnalyticsManager.Instance != null)
        {
            GameAnalyticsManager.Instance.TrackObstacleHit();
        }
    }

    #endregion

    #region Input Methods

    private void HandleSwipeInput()
    {
        // Read touch input on a mobile device.
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch touch =
                UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                swipeStartPosition = touch.screenPosition;
            }
            else if (touch.phase ==
                     UnityEngine.InputSystem.TouchPhase.Ended)
            {
                swipeEndPosition = touch.screenPosition;
                CheckSwipe();
            }
        }

        // Allow mouse input in the Unity Editor.
        if (Mouse.current == null)
        {
            return;
        }

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

    private void CheckSwipe()
    {
        // Calculate swipe direction and distance.
        Vector2 swipeDirection =
            swipeEndPosition - swipeStartPosition;

        float minimumSwipeDistanceSquared =
            minimumSwipeDistance * minimumSwipeDistance;

        // Treat short gestures as taps.
        if (swipeDirection.sqrMagnitude <
            minimumSwipeDistanceSquared)
        {
            ShowStatus("Tap");
            return;
        }

        // Horizontal swipe.
        if (Mathf.Abs(swipeDirection.x) >
            Mathf.Abs(swipeDirection.y))
        {
            if (swipeDirection.x > 0f)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
        // Vertical swipe.
        else
        {
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
        // Stop at the left edge.
        if (currentLane <= 0)
        {
            ShowStatus("Left Edge");
            return;
        }

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
        // Stop at the right edge.
        if (currentLane >= lanePositions.Length - 1)
        {
            ShowStatus("Right Edge");
            return;
        }

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
        targetLaneX = lanePositions[currentLane];
        isMovingBetweenLanes = true;
    }

    private void MoveToLane()
    {
        Vector3 currentPosition = cachedTransform.position;

        currentPosition.x = Mathf.MoveTowards(
            currentPosition.x,
            targetLaneX,
            laneMoveSpeed * Time.deltaTime
        );

        cachedTransform.position = currentPosition;

        if (Mathf.Approximately(
            currentPosition.x,
            targetLaneX))
        {
            isMovingBetweenLanes = false;
        }
    }

    private void Jump()
    {
        // Prevent double jumps.
        if (isJumping)
        {
            return;
        }

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
        jumpTimer += Time.deltaTime * jumpSpeed;

        float jumpOffset =
            Mathf.Sin(jumpTimer) * jumpHeight;

        Vector3 currentPosition = cachedTransform.position;
        currentPosition.y = baseY + jumpOffset;
        cachedTransform.position = currentPosition;

        if (jumpTimer < Mathf.PI)
        {
            return;
        }

        isJumping = false;

        currentPosition = cachedTransform.position;
        currentPosition.y = baseY;
        cachedTransform.position = currentPosition;
    }

    #endregion

    #region Sensor Methods

    private void HandleTiltFeedback()
    {
        // Read horizontal device tilt.
        float targetTiltX =
            Accelerometer.current.acceleration.ReadValue().x;

        // Smooth the accelerometer value.
        smoothedTiltX = Mathf.Lerp(
            smoothedTiltX,
            targetTiltX,
            tiltSmoothing * Time.deltaTime
        );

        // Rotate the player visually.
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
        if (gameUIManager != null)
        {
            gameUIManager.ShowStatusMessage(message);
        }
    }

    private void FlashPlayer()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = feedbackColor;
        flashTimer = feedbackTime;
        isFlashing = true;
    }

    private void UpdateFlashTimer()
    {
        flashTimer -= Time.deltaTime;

        if (flashTimer > 0f)
        {
            return;
        }

        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    private void UpdateInvincibilityTimer()
    {
        invincibilityTimer -= Time.deltaTime;

        if (invincibilityTimer > 0f)
        {
            return;
        }

        isInvincible = false;
    }

    #endregion
}