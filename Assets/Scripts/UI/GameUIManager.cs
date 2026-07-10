using TMPro;
using UnityEngine;

/*
 * Script Name: GameUIManager
 * Purpose: Handles gameplay UI for the mobile runner prototype.
 * Responsibilities:
 * - Shows starting instructions.
 * - Hides instructions after a short delay.
 * - Displays temporary status messages for player actions.
 */

public class GameUIManager : MonoBehaviour
{
    #region Inspector Settings

    [Header("UI Text References")]
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text statusText;

    [Header("Instruction Settings")]
    [SerializeField] private float instructionHideDelay = 3f;

    [Header("Status Message Settings")]
    [SerializeField] private float statusClearDelay = 1.5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        ShowStartingUI();
    }

    #endregion

    #region Instruction Methods

    private void ShowStartingUI()
    {
        // Show the instructions when the scene starts.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);
        }

        // Show the starting status message.
        ShowStatusMessage("Ready");

        // Hide instructions after the player has time to read them.
        Invoke(nameof(HideInstructions), instructionHideDelay);
    }

    private void HideInstructions()
    {
        // Hide the instruction text to keep the screen clean.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Status Methods

    public void ShowStatusMessage(string message)
    {
        // Display the current player action or game message.
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = message;
        }

        // Clear the message after a short delay.
        CancelInvoke(nameof(ClearStatusMessage));
        Invoke(nameof(ClearStatusMessage), statusClearDelay);
    }

    private void ClearStatusMessage()
    {
        // Clear the status text so it does not stay on screen forever.
        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    #endregion
}