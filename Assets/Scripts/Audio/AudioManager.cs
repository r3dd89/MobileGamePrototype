using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Script Name: AudioManager
 * Purpose: Handles menu music, gameplay music,
 *          and sound effects for the game.
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
        string currentSceneName =
            SceneManager.GetActiveScene().name;

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
        if (musicSource.clip == clip &&
            musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
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

        sfxSource.PlayOneShot(
            clip,
            sfxVolume
        );
    }

    #endregion
}