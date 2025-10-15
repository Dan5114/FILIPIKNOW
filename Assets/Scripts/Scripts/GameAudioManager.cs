using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Centralized audio manager for all game sounds.
/// Handles: buttons, correct/wrong answers, victory music, typing sounds.
/// </summary>
public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    [Tooltip("Sound when any button is clicked")]
    public AudioClip buttonClickSound;
    
    [Tooltip("Sound when answer is correct")]
    public AudioClip correctAnswerSound;
    
    [Tooltip("Sound when answer is wrong")]
    public AudioClip wrongAnswerSound;
    
    [Tooltip("Keyboard typing sounds for dialog text - multiple sounds for variation!")]
    public AudioClip[] keyboardTypingSounds;
    
    [Header("Music")]
    [Tooltip("Victory music when summary/level complete shows")]
    public AudioClip victoryMusic;
    
    [Tooltip("Background music for gameplay")]
    public AudioClip backgroundMusic;
    
    [Header("Audio Sources")]
    [Tooltip("For sound effects (buttons, answers, typing)")]
    public AudioSource sfxAudioSource;
    
    [Tooltip("For music (victory, background)")]
    public AudioSource musicAudioSource;
    
    [Header("Settings")]
    [Tooltip("Volume for sound effects (0-1)")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    [Tooltip("Volume for music (0-1)")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    
    [Tooltip("Auto-add click sounds to all buttons in scene")]
    public bool autoSetupButtonSounds = true;

    private Coroutine victoryMusicCoroutine;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            
            // Subscribe to scene changes to re-setup buttons
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (autoSetupButtonSounds)
        {
            SetupAllButtonSounds();
        }
        
        PlayBackgroundMusic();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from scene events when destroyed
        if (Instance == this)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    // Called automatically whenever a new scene loads
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (autoSetupButtonSounds)
        {
            // Wait one frame for scene to fully initialize
            StartCoroutine(SetupButtonsNextFrame());
        }
        
        Debug.Log($"🎵 GameAudioManager: Scene '{scene.name}' loaded, setting up button sounds...");
    }

    void SetupAudioSources()
    {
        // Create audio sources if they don't exist
        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.volume = sfxVolume;
        }

        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource.playOnAwake = false;
            musicAudioSource.loop = true;
            musicAudioSource.volume = musicVolume;
        }
    }

    /// <summary>
    /// Automatically adds click sounds to all buttons in the scene
    /// </summary>
    public void SetupAllButtonSounds()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        
        foreach (Button button in allButtons)
        {
            // Remove any existing listeners to avoid duplicates
            button.onClick.RemoveListener(PlayButtonClick);
            
            // Add click sound at the beginning
            button.onClick.AddListener(PlayButtonClick);
        }
        
        Debug.Log($"GameAudioManager: Added click sounds to {allButtons.Length} buttons");
    }

    #region Sound Effect Methods

    /// <summary>
    /// Play button click sound
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySoundEffect(buttonClickSound);
    }

    /// <summary>
    /// Play correct answer sound with optional haptic feedback
    /// </summary>
    public void PlayCorrectAnswer()
    {
        PlaySoundEffect(correctAnswerSound);
        
        // Trigger haptic feedback
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        Debug.Log("✅ Correct answer sound played!");
    }

    /// <summary>
    /// Play wrong answer sound with optional haptic feedback
    /// </summary>
    public void PlayWrongAnswer()
    {
        PlaySoundEffect(wrongAnswerSound);
        
        // Trigger haptic feedback (different pattern if possible)
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        Debug.Log("❌ Wrong answer sound played!");
    }

    /// <summary>
    /// Play keyboard typing sound (for typewriter effect)
    /// Randomly selects from multiple sounds for natural variation!
    /// </summary>
    public void PlayTypingSound()
    {
        if (keyboardTypingSounds != null && keyboardTypingSounds.Length > 0 && sfxAudioSource != null)
        {
            // Randomly select a typing sound for natural variation
            AudioClip randomTypingSound = keyboardTypingSounds[Random.Range(0, keyboardTypingSounds.Length)];
            
            if (randomTypingSound != null)
            {
                // Play at lower volume to not be annoying
                sfxAudioSource.PlayOneShot(randomTypingSound, sfxVolume * 0.3f);
            }
        }
    }

    /// <summary>
    /// Play any sound effect
    /// </summary>
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && sfxAudioSource != null)
        {
            // Check if sound effects are enabled
            if (SettingsManager.Instance != null && !SettingsManager.Instance.AreSoundEffectsEnabled())
            {
                return;
            }

            sfxAudioSource.PlayOneShot(clip, sfxVolume);
        }
    }

    #endregion

    #region Music Methods

    /// <summary>
    /// Play background music (loops)
    /// </summary>
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicAudioSource != null)
        {
            // Check if music is enabled
            if (SettingsManager.Instance != null && !SettingsManager.Instance.IsMusicEnabled())
            {
                return;
            }

            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.loop = true;
            musicAudioSource.volume = musicVolume;
            musicAudioSource.Play();
            
            Debug.Log("🎵 Background music started");
        }
    }

    /// <summary>
    /// Play victory music when level/summary is complete
    /// </summary>
    public void PlayVictoryMusic()
    {
        if (victoryMusic != null && musicAudioSource != null)
        {
            // Check if music is enabled
            if (SettingsManager.Instance != null && !SettingsManager.Instance.IsMusicEnabled())
            {
                return;
            }

            // Stop any existing victory music
            if (victoryMusicCoroutine != null)
            {
                StopCoroutine(victoryMusicCoroutine);
            }

            victoryMusicCoroutine = StartCoroutine(PlayVictoryMusicCoroutine());
        }
    }

    IEnumerator PlayVictoryMusicCoroutine()
    {
        // Fade out background music
        float fadeTime = 0.5f;
        float startVolume = musicAudioSource.volume;
        
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }

        // Play victory music
        musicAudioSource.clip = victoryMusic;
        musicAudioSource.loop = false;
        musicAudioSource.volume = musicVolume;
        musicAudioSource.Play();
        
        Debug.Log("🎉 Victory music playing!");

        // Wait for victory music to finish
        yield return new WaitForSeconds(victoryMusic.length);

        // Resume background music
        PlayBackgroundMusic();
    }

    /// <summary>
    /// Stop all music
    /// </summary>
    public void StopMusic()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.Stop();
        }
    }

    /// <summary>
    /// Pause music
    /// </summary>
    public void PauseMusic()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.Pause();
        }
    }

    /// <summary>
    /// Resume music
    /// </summary>
    public void ResumeMusic()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.UnPause();
        }
    }

    #endregion

    #region Volume Control

    /// <summary>
    /// Set sound effects volume
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// Set music volume
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolume;
        }
    }

    #endregion

    #region Scene Integration

    IEnumerator SetupButtonsNextFrame()
    {
        yield return null; // Wait one frame for scene to fully load
        SetupAllButtonSounds();
    }

    #endregion
}

