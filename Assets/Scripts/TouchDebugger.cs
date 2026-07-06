using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchDebugger : MonoBehaviour
{
    // TextMeshPro text used to display input information.
    public TMP_Text touchInfoText;

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
        if (touchInfoText == null)
        {
            return;
        }

        string message = "";

        // Real mobile touch input.
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            message += "Touch Count: " + UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count + "\n\n";

            foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                message += "Touch ID: " + touch.touchId + "\n";
                message += "Position: " + touch.screenPosition + "\n";
                message += "Phase: " + touch.phase + "\n";
                message += "Delta Position: " + touch.delta + "\n\n";
            }
        }
        else
        {
            message += "Touch Count: 0\n\n";
            message += "Editor Mouse Simulation\n";

            if (Mouse.current != null)
            {
                message += "Mouse Position: " + Mouse.current.position.ReadValue() + "\n";

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    message += "Phase: Began\n";
                }
                else if (Mouse.current.leftButton.isPressed)
                {
                    message += "Phase: Moved/Held\n";
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    message += "Phase: Ended\n";
                }
                else
                {
                    message += "Phase: No Input\n";
                }
            }
            else
            {
                message += "No mouse detected.\n";
            }
        }

        touchInfoText.text = message;
    }
}