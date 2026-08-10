using System.Collections;
using TMPro;
using UnityEngine;

/*
 * Script Name: GameUIManager
 * Purpose: Handles gameplay instructions, the starting countdown,
 *          and temporary status messages for the mobile runner.
 */

public class GameUIManager : MonoBehaviour
{
    #region Inspector Settings

    [Header("UI Text References")]

    // Displays the controls and starting directions.
    [SerializeField] private TMP_Text instructionText;

    // Displays countdown numbers and temporary gameplay messages.
    [SerializeField] private TMP_Text statusText;

    [Header("Starting Instructions")]

    // Amount of time the player can read the controls before countdown begins.
    [SerializeField] private float instructionDisplayTime = 2f;

    [Header("Countdown Settings")]

    // Amount of time each countdown number remains visible.
    [SerializeField] private float countdownStepTime = 0.8f;

    // Amount of time GO remains visible.
    [SerializeField] private float goDisplayTime = 0.6f;

    [Header("Status Message Settings")]

    // Amount of time normal gameplay messages remain visible.
    [SerializeField] private float statusClearDelay = 1.5f;

    #endregion

    #region Private Variables

    // Tracks whether the starting sequence has finished.
    private bool startupComplete;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Begin the game paused so nothing moves before the countdown.
        Time.timeScale = 0f;

        // Start the directions and countdown sequence.
        StartCoroutine(StartGameSequence());
    }

    #endregion

    #region Starting Sequence

    private IEnumerator StartGameSequence()
    {
        startupComplete = false;

        // Show the controls first.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);

            instructionText.text =
                "Swipe Left / Right to Change Lanes\n" +
                "Swipe Up to Jump\n" +
                "Collect Coins and Avoid Obstacles!";
        }

        // Clear the status area while instructions are shown.
        if (statusText != null)
        {
            statusText.text = "";
        }

        /*
         * WaitForSecondsRealtime is used because normal game time
         * is currently paused.
         */
        yield return new WaitForSecondsRealtime(
            instructionDisplayTime
        );

        // Hide the directions before the countdown.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }

        // Show 3.
        SetStatusText("3");

        yield return new WaitForSecondsRealtime(
            countdownStepTime
        );

        // Show 2.
        SetStatusText("2");

        yield return new WaitForSecondsRealtime(
            countdownStepTime
        );

        // Show 1.
        SetStatusText("1");

        yield return new WaitForSecondsRealtime(
            countdownStepTime
        );

        // Start gameplay.
        Time.timeScale = 1f;
        startupComplete = true;

        // Show GO briefly as gameplay begins.
        SetStatusText("GO!");

        yield return new WaitForSecondsRealtime(
            goDisplayTime
        );

        ClearStatusMessage();
    }

    #endregion

    #region Status Methods

    public void ShowStatusMessage(string message)
    {
        /*
         * Ignore normal gameplay messages until the
         * starting sequence has finished.
         */
        if (!startupComplete)
        {
            return;
        }

        if (statusText == null)
        {
            return;
        }

        // Cancel the previous clear timer.
        CancelInvoke(nameof(ClearStatusMessage));

        // Display the new message.
        statusText.text = message;

        // Clear the message after a short delay.
        Invoke(
            nameof(ClearStatusMessage),
            statusClearDelay
        );
    }

    private void SetStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void ClearStatusMessage()
    {
        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    #endregion
}