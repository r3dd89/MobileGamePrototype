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

    private readonly float[] lanePositions = { -2f, 0f, 2f };

    private int currentLane = 1;
    private float targetLaneX;

    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;

    private bool isMovingBetweenLanes;
    private bool isJumping;
    private bool hasAccelerometer;
    private bool isFlashing;

    private float baseY;
    private float jumpTimer;
    private float smoothedTiltX;
    private float flashTimer;

    private Transform cachedTransform;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        // Enable mobile touch support.
        EnhancedTouchSupport.Enable();

        // Check for an accelerometer once when the script is enabled.
        hasAccelerometer = Accelerometer.current != null;

        if (hasAccelerometer)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Awake()
    {
        // Cache components that are used repeatedly.
        cachedTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Obstacle"))
        {
            return;
        }

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

    #endregion

    #region Input Methods

    private void HandleSwipeInput()
    {
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];

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

        // Mouse controls remain available for Editor testing.
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
        Vector2 swipeDirection =
            swipeEndPosition - swipeStartPosition;

        // Compare squared distances to avoid an unnecessary square root.
        float minimumSwipeDistanceSquared =
            minimumSwipeDistance * minimumSwipeDistance;

        if (swipeDirection.sqrMagnitude <
            minimumSwipeDistanceSquared)
        {
            ShowStatus("Tap");
            return;
        }

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
        else if (swipeDirection.y > 0f)
        {
            Jump();
        }
        else
        {
            ShowStatus("Swipe Down");
        }
    }

    #endregion

    #region Movement Methods

    private void MoveLeft()
    {
        if (currentLane <= 0)
        {
            ShowStatus("Left Edge");
            return;
        }

        currentLane--;
        BeginLaneMovement();

        ShowStatus("Swipe Left");
        FlashPlayer();
    }

    private void MoveRight()
    {
        if (currentLane >= lanePositions.Length - 1)
        {
            ShowStatus("Right Edge");
            return;
        }

        currentLane++;
        BeginLaneMovement();

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

        if (Mathf.Approximately(currentPosition.x, targetLaneX))
        {
            isMovingBetweenLanes = false;
        }
    }

    private void Jump()
    {
        if (isJumping)
        {
            return;
        }

        isJumping = true;
        jumpTimer = 0f;

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
        float targetTiltX =
            Accelerometer.current.acceleration.ReadValue().x;

        smoothedTiltX = Mathf.Lerp(
            smoothedTiltX,
            targetTiltX,
            tiltSmoothing * Time.deltaTime
        );

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

    #endregion
}