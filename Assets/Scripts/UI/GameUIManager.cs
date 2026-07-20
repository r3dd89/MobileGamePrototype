using TMPro;
using UnityEngine;

/*
 * Script Name: GameUIManager
 * Purpose: Handles the gameplay UI for the mobile runner prototype.
 * Responsibilities:
 * - Shows the starting instructions.
 * - Hides the instructions after a short delay.
 * - Displays temporary status messages for player actions.
 */

public class GameUIManager : MonoBehaviour
{
    #region Inspector Settings

    [Header("UI Text References")]

    // This text shows the controls when the game starts.
    [SerializeField] private TMP_Text instructionText;

    // This text shows short messages such as Ready, Jump, Left, or Right.
    [SerializeField] private TMP_Text statusText;

    [Header("Instruction Settings")]

    // This controls how long the instructions stay on the screen.
    [SerializeField] private float instructionHideDelay = 3f;

    [Header("Status Message Settings")]

    // This controls how long a status message stays on the screen.
    [SerializeField] private float statusClearDelay = 1.5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Show the starting UI when the gameplay scene begins.
        ShowStartingUI();
    }

    #endregion

    #region Instruction Methods

    private void ShowStartingUI()
    {
        // Show the instruction text when the scene starts.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);
        }

        // Show a short starting message to the player.
        ShowStatusMessage("Ready");

        // Hide the instructions after the player has time to read them.
        Invoke(nameof(HideInstructions), instructionHideDelay);
    }

    private void HideInstructions()
    {
        // Hide the instruction text to keep the gameplay screen clean.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Status Methods

    public void ShowStatusMessage(string message)
    {
        // Stop if the status text was not assigned in the Inspector.
        if (statusText == null)
        {
            return;
        }

        // Cancel the previous clear timer if another message appears.
        CancelInvoke(nameof(ClearStatusMessage));

        // Display the new message.
        statusText.text = message;

        // Clear the message after a short delay.
        Invoke(nameof(ClearStatusMessage), statusClearDelay);
    }

    private void ClearStatusMessage()
    {
        // Clear the status text so it does not remain on the screen.
        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    #endregion
}