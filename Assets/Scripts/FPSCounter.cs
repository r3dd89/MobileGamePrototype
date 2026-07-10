using TMPro;
using UnityEngine;

/*
 * Script Name: FPSCounter
 * Purpose: Displays the current frames per second on the UI.
 */

public class FPSCounter : MonoBehaviour
{
    #region Inspector Settings

    [Header("UI Settings")]
    [SerializeField] private TMP_Text fpsText;

    #endregion

    #region Private Variables

    private float deltaTime;

    #endregion

    #region Unity Methods

    private void Update()
    {
        // Stop if the FPS text has not been assigned.
        if (fpsText == null)
        {
            return;
        }

        // Smooth the FPS value so it does not jump around too much.
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float fps = 1f / deltaTime;

        fpsText.text = "FPS: " + Mathf.RoundToInt(fps);
    }

    #endregion
}