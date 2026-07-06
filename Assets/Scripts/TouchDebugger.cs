using TMPro;
using UnityEngine;

public class TouchDebugger : MonoBehaviour
{
    // TextMeshPro text used to display input information.
    public TMP_Text touchInfoText;

    private void Update()
    {
        // Stop the script if the text field was not assigned.
        if (touchInfoText == null)
        {
            return;
        }

        // Display real mobile touch information if a touch exists.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            string message = "Touch Count: " + Input.touchCount + "\n\n";
            message += "Finger ID: " + touch.fingerId + "\n";
            message += "Position: " + touch.position + "\n";
            message += "Phase: " + touch.phase + "\n";
            message += "Delta Position: " + touch.deltaPosition + "\n";
            message += "Delta Time: " + touch.deltaTime + "\n";

            touchInfoText.text = message;
        }
        else
        {
            // This lets the mouse act like simulated touch input in the Unity Editor.
            string message = "Touch Count: 0\n\n";
            message += "Editor Mouse Simulation\n";
            message += "Mouse Position: " + Input.mousePosition + "\n";

            if (Input.GetMouseButtonDown(0))
            {
                message += "Phase: Began\n";
            }
            else if (Input.GetMouseButton(0))
            {
                message += "Phase: Moved/Held\n";
            }
            else if (Input.GetMouseButtonUp(0))
            {
                message += "Phase: Ended\n";
            }
            else
            {
                message += "Phase: No Input\n";
            }

            touchInfoText.text = message;
        }
    }
}