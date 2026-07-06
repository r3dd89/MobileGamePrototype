using TMPro;
using UnityEngine;

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

    private void Update()
    {
        // Stop the script if the text field was not assigned.
        if (touchResultText == null)
        {
            return;
        }

        // Use real mobile touch input if available.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
                touchResultText.text = "Touch Started";
            }

            if (touch.phase == TouchPhase.Ended)
            {
                endPosition = touch.position;
                CheckTapOrSwipe();
            }
        }

        // Use mouse input for testing inside the Unity Editor.
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            touchResultText.text = "Mouse Touch Started";
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPosition = Input.mousePosition;
            CheckTapOrSwipe();
        }
    }

    private void CheckTapOrSwipe()
    {
        // Measure how far the input moved.
        float distance = Vector2.Distance(startPosition, endPosition);

        // If the input did not move far enough, it is a tap.
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
        // Get the direction of the swipe.
        Vector2 swipeDirection = endPosition - startPosition;

        // Check whether the swipe was mostly horizontal or vertical.
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