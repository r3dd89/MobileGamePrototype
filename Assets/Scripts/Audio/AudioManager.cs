using UnityEngine;

/*
 * Script Name: AudioManager
 * Purpose: Handles background music and sound effects for the game.
 */

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager Instance { get; private set; }

    #endregion

    #region Inspector Settings

    [Header("Audio Sources")]

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Sound Effects")]

    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Volume")]

    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.4f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.8f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    #endregion

    #region Music Methods

    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
        {
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(backgroundMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

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