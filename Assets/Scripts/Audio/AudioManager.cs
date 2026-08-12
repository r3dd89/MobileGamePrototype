using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Script Name: AudioManager
 * Purpose: Handles menu music, gameplay music,
 *          sound effects, volume settings, and
 *          audio settings for the game.
 */

public class AudioManager : MonoBehaviour
{
    #region Singleton

    // Allows other scripts to access the AudioManager.
    public static AudioManager Instance { get; private set; }

    #endregion

    #region Inspector Settings

    [Header("Audio Sources")]

    // AudioSource used for looping music.
    [SerializeField] private AudioSource musicSource;

    // AudioSource used for sound effects.
    [SerializeField] private AudioSource sfxSource;


    [Header("Music")]

    // Music played on the Main Menu scene.
    [SerializeField] private AudioClip menuMusic;

    // Music played during gameplay.
    [SerializeField] private AudioClip gameplayMusic;


    [Header("Sound Effects")]

    // Sound played when collecting a coin.
    [SerializeField] private AudioClip coinCollectSound;

    // Sound played when hitting an obstacle.
    [SerializeField] private AudioClip hitSound;

    // Sound played when the game ends.
    [SerializeField] private AudioClip gameOverSound;


    [Header("Volume")]

    // Volume level for background music.
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.4f;

    // Volume level for sound effects.
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.8f;


    [Header("Scene Settings")]

    // Exact name of the Main Menu scene.
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    // Exact name of the gameplay scene.
    [SerializeField] private string gameplaySceneName = "Gameplay";

    #endregion

    #region PlayerPrefs Keys

    // Names used to save the player's audio settings.
    private const string MusicEnabledKey = "MusicEnabled";
    private const string SFXEnabledKey = "SFXEnabled";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    #endregion

    #region Public Properties

    // Returns true when music is currently enabled.
    // SettingsUI uses this to set the Music Toggle correctly.
    public bool IsMusicEnabled
    {
        get
        {
            if (musicSource == null)
            {
                return true;
            }

            return !musicSource.mute;
        }
    }

    // Returns true when sound effects are currently enabled.
    // SettingsUI uses this to set the SFX Toggle correctly.
    public bool IsSFXEnabled
    {
        get
        {
            if (sfxSource == null)
            {
                return true;
            }

            return !sfxSource.mute;
        }
    }

    // Allows another script to read the current music volume.
    public float MusicVolume
    {
        get { return musicVolume; }
    }

    // Allows another script to read the current SFX volume.
    public float SFXVolume
    {
        get { return sfxVolume; }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Prevent duplicate AudioManagers.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep the AudioManager when changing scenes.
        DontDestroyOnLoad(gameObject);

        // Load the player's saved audio settings.
        LoadAudioSettings();

        // Apply the saved settings.
        ApplyAudioSettings();
    }

    private void OnEnable()
    {
        // Listen for scene changes.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Stop listening for scene changes.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Play the correct music for the current scene.
        PlayMusicForCurrentScene();
    }

    #endregion

    #region Scene Methods

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Switch music whenever a new scene loads.
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == mainMenuSceneName)
        {
            PlayMenuMusic();
        }
        else if (currentSceneName == gameplaySceneName)
        {
            PlayGameplayMusic();
        }
    }

    #endregion

    #region Music Methods

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        // Stop if the music source or clip is missing.
        if (musicSource == null || clip == null)
        {
            return;
        }

        // Do not restart the same song if it is already playing.
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        // Assign the music clip.
        musicSource.clip = clip;

        // Set the music volume.
        musicSource.volume = musicVolume;

        // Music should continuously loop.
        musicSource.loop = true;

        // Start playing.
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    #endregion

    #region SFX Methods

    public void PlayCoinSound()
    {
        PlaySFX(coinCollectSound);
    }

    public void PlayHitSound()
    {
        PlaySFX(hitSound);
    }

    public void PlayGameOverSound()
    {
        PlaySFX(gameOverSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        // Stop if the SFX source or clip is missing.
        if (sfxSource == null || clip == null)
        {
            return;
        }

        // Do not play sound effects when SFX are disabled.
        if (sfxSource.mute)
        {
            return;
        }

        // Play the sound effect.
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    #endregion

    #region Enable And Disable Audio

    // Turns music on or off.
    // This can be connected directly to a UI Toggle.
    public void SetMusicEnabled(bool enabled)
    {
        if (musicSource != null)
        {
            // AudioSource mute works opposite of enabled.
            musicSource.mute = !enabled;
        }

        // Save the player's choice.
        PlayerPrefs.SetInt(
            MusicEnabledKey,
            enabled ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    // Turns sound effects on or off.
    // This can be connected directly to a UI Toggle.
    public void SetSFXEnabled(bool enabled)
    {
        if (sfxSource != null)
        {
            // AudioSource mute works opposite of enabled.
            sfxSource.mute = !enabled;
        }

        // Save the player's choice.
        PlayerPrefs.SetInt(
            SFXEnabledKey,
            enabled ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    #endregion

    #region Volume Methods

    // Changes the music volume.
    // Can be connected to a UI Slider.
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        // Save the volume.
        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            musicVolume
        );

        PlayerPrefs.Save();
    }

    // Changes the SFX volume.
    // Can be connected to a UI Slider.
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

        // Save the volume.
        PlayerPrefs.SetFloat(
            SFXVolumeKey,
            sfxVolume
        );

        PlayerPrefs.Save();
    }

    #endregion

    #region Save And Load Settings

    private void LoadAudioSettings()
    {
        // Load saved music volume.
        // If there is no saved value yet,
        // use the value from the Inspector.
        musicVolume = PlayerPrefs.GetFloat(
            MusicVolumeKey,
            musicVolume
        );

        // Load saved SFX volume.
        sfxVolume = PlayerPrefs.GetFloat(
            SFXVolumeKey,
            sfxVolume
        );
    }

    private void ApplyAudioSettings()
    {
        // Apply saved volume levels.
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;

            // Default to music ON if the player
            // has never changed this setting before.
            bool musicEnabled =
                PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;

            musicSource.mute = !musicEnabled;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;

            // Default to SFX ON if the player
            // has never changed this setting before.
            bool sfxEnabled =
                PlayerPrefs.GetInt(SFXEnabledKey, 1) == 1;

            sfxSource.mute = !sfxEnabled;
        }
    }

    #endregion
}