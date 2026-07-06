using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchDetector : MonoBehaviour
{
    // TextMeshPro text used to display tap and swipe results.
    public TMP_Text touchResultText;

    // Stores where the input started.
    private Vector2 startPosition;

    // Stores where the input ended.
    private Vector2 endPosition;

    // Minimum movement distance needed to count as a swipe.
    public float swipeDistance = 100f;

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

    private void Update()
    {
        if (touchResultText == null)
        {
            return;
        }

        // Real mobile touch input.
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startPosition = touch.screenPosition;
                touchResultText.text = "Touch Started";
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                endPosition = touch.screenPosition;
                CheckTapOrSwipe();
            }
        }

        // Mouse input for Unity Editor testing.
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                startPosition = Mouse.current.position.ReadValue();
                touchResultText.text = "Mouse Touch Started";
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                endPosition = Mouse.current.position.ReadValue();
                CheckTapOrSwipe();
            }
        }
    }

    private void CheckTapOrSwipe()
    {
        float distance = Vector2.Distance(startPosition, endPosition);

        if (distance < swipeDistance)
        {
            touchResultText.text = "Tap Detected\nPosition: " + endPosition;
        }
        else
        {
            DetectSwipeDirection();
        }
    }

    private void DetectSwipeDirection()
    {
        Vector2 swipeDirection = endPosition - startPosition;

        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            if (swipeDirection.x > 0)
            {
                touchResultText.text = "Swipe Right Detected";
            }
            else
            {
                touchResultText.text = "Swipe Left Detected";
            }
        }
        else
        {
            if (swipeDirection.y > 0)
            {
                touchResultText.text = "Swipe Up Detected";
            }
            else
            {
                touchResultText.text = "Swipe Down Detected";
            }
        }
    }
}