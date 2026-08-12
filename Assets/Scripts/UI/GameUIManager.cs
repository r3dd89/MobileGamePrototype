using System.Collections;
using TMPro;
using UnityEngine;

/*
 * Script Name: GameUIManager
 * Purpose: Handles starting instructions, countdown,
 *          and temporary gameplay status messages.
 */

public class GameUIManager : MonoBehaviour
{
    #region Inspector Settings

    [Header("UI Text References")]

    // Shows the controls when the game begins.
    [SerializeField] private TMP_Text instructionText;

    // Shows countdown numbers and temporary gameplay messages.
    [SerializeField] private TMP_Text statusText;

    [Header("Starting Sequence")]

    // How long instructions remain visible.
    [SerializeField] private float instructionDisplayTime = 2.5f;

    // How long each countdown number remains visible.
    [SerializeField] private float countdownStepTime = 0.8f;

    // How long GO remains visible.
    [SerializeField] private float goDisplayTime = 0.6f;

    [Header("Status Message Settings")]

    // How long normal gameplay messages remain visible.
    [SerializeField] private float statusClearDelay = 1.5f;

    #endregion

    #region Private Variables

    // Prevents normal status messages during the intro sequence.
    private bool startupComplete;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Pause gameplay while instructions and countdown are shown.
        Time.timeScale = 0f;

        StartCoroutine(StartGameSequence());
    }

    #endregion

    #region Starting Sequence

    private IEnumerator StartGameSequence()
    {
        startupComplete = false;

        // Show only the instructions first.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(true);

            instructionText.text =
                "Swipe Left / Right to Change Lanes\n" +
                "Swipe Up to Jump\n" +
                "Collect Coins and Avoid Obstacles";
        }

        // Hide countdown text while instructions are visible.
        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(
            instructionDisplayTime
        );

        // Hide instructions completely.
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }

        // Show countdown text.
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
        }

        SetStatusText("3");

        yield return new WaitForSecondsRealtime(
            countdownStepTime
        );

        SetStatusText("2");

        yield return new WaitForSecondsRealtime(
            countdownStepTime
        );

        SetStatusText("1");

        yield return new WaitForSecondsRealtime(
            countdownStepTime
        );

        SetStatusText("GO!");

        // Start gameplay.
        Time.timeScale = 1f;
        startupComplete = true;

        yield return new WaitForSecondsRealtime(
            goDisplayTime
        );

        ClearStatusMessage();
    }

    #endregion

    #region Status Methods

    public void ShowStatusMessage(string message)
    {
        // Ignore gameplay messages during the intro.
        if (!startupComplete)
        {
            return;
        }

        if (statusText == null)
        {
            return;
        }

        // Make sure status text is visible.
        statusText.gameObject.SetActive(true);

        CancelInvoke(nameof(ClearStatusMessage));

        statusText.text = message;

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