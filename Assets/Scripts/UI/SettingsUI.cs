using UnityEngine;
using UnityEngine.UI;

/*
 * Script Name: SettingsUI
 * Purpose: Connects the Settings menu toggles
 *          to the AudioManager and saved preferences.
 */

public class SettingsUI : MonoBehaviour
{
    #region Inspector Settings

    [Header("Audio Toggles")]

    // Toggle used to turn music on or off.
    [SerializeField] private Toggle musicToggle;

    // Toggle used to turn sound effects on or off.
    [SerializeField] private Toggle sfxToggle;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Stop if there is no AudioManager.
        if (AudioManager.Instance == null)
        {
            return;
        }

        // Set the Music Toggle to match the current
        // saved music setting.
        if (musicToggle != null)
        {
            musicToggle.isOn =
                AudioManager.Instance.IsMusicEnabled;

            // Listen for changes to the Music Toggle.
            musicToggle.onValueChanged.AddListener(
                OnMusicToggleChanged
            );
        }

        // Set the SFX Toggle to match the current
        // saved sound effect setting.
        if (sfxToggle != null)
        {
            sfxToggle.isOn =
                AudioManager.Instance.IsSFXEnabled;

            // Listen for changes to the SFX Toggle.
            sfxToggle.onValueChanged.AddListener(
                OnSFXToggleChanged
            );
        }
    }

    #endregion

    #region Toggle Methods

    private void OnMusicToggleChanged(bool enabled)
    {
        // Send the new Music Toggle value
        // to the AudioManager.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicEnabled(enabled);
        }
    }

    private void OnSFXToggleChanged(bool enabled)
    {
        // Send the new SFX Toggle value
        // to the AudioManager.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXEnabled(enabled);
        }
    }

    #endregion
}