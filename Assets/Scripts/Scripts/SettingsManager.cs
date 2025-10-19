using UnityEngine;
using System;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    
    [Header("Audio Settings")]
    public bool musicEnabled = true;
    public bool soundEffectsEnabled = true;
    public float musicVolume = 1f;
    public float soundEffectsVolume = 1f;
    
    [Header("Language Settings")]
    public bool useFilipino = true; // true = Filipino, false = English
    
    [Header("Haptic Settings")]
    public bool hapticFeedbackEnabled = true;
    
    [Header("Audio Sources")]
    public AudioSource musicAudioSource;
    public AudioSource soundEffectsAudioSource;
    
    // Events
    public static event Action<bool> OnMusicToggled;
    public static event Action<bool> OnSoundEffectsToggled;
    public static event Action<bool> OnLanguageChanged;
    public static event Action<bool> OnHapticToggled;
    
    // Keys for PlayerPrefs
    private const string MUSIC_ENABLED_KEY = "MusicEnabled";
    private const string SOUND_EFFECTS_ENABLED_KEY = "SoundEffectsEnabled";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SOUND_EFFECTS_VOLUME_KEY = "SoundEffectsVolume";
    private const string USE_FILIPINO_KEY = "UseFilipino";
    private const string HAPTIC_ENABLED_KEY = "HapticEnabled";
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudioSources()
    {
        // Auto-find audio sources if not assigned
        if (musicAudioSource == null)
        {
            GameObject musicObject = GameObject.Find("MusicAudioSource");
            if (musicObject != null)
                musicAudioSource = musicObject.GetComponent<AudioSource>();
        }
        
        if (soundEffectsAudioSource == null)
        {
            GameObject sfxObject = GameObject.Find("SoundEffectsAudioSource");
            if (sfxObject != null)
                soundEffectsAudioSource = sfxObject.GetComponent<AudioSource>();
        }
        
        // Update audio sources with current settings
        UpdateAudioSettings();
    }
    
    public void ToggleMusic(bool enabled)
    {
        musicEnabled = enabled;
        UpdateAudioSettings();
        SaveSettings();
        OnMusicToggled?.Invoke(enabled);
        
        Debug.Log($"Music {(enabled ? "enabled" : "disabled")}");
    }
    
    public void ToggleSoundEffects(bool enabled)
    {
        soundEffectsEnabled = enabled;
        UpdateAudioSettings();
        SaveSettings();
        OnSoundEffectsToggled?.Invoke(enabled);
        
        Debug.Log($"Sound Effects {(enabled ? "enabled" : "disabled")}");
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        
        // 🎵 Sync with GameAudioManager
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.SetMusicVolume(volume);
        }
        

        UpdateAudioSettings();
        SaveSettings();
        
        Debug.Log($"Music volume set to {musicVolume:F2}");
    }
    
    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = Mathf.Clamp01(volume);
        
        // 🔊 Sync with GameAudioManager
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.SetSFXVolume(volume);
        }
        
        UpdateAudioSettings();
        SaveSettings();
        
        Debug.Log($"Sound Effects volume set to {soundEffectsVolume:F2}");
    }
    
    public void ToggleLanguage(bool useFilipinoLanguage)
    {
        useFilipino = useFilipinoLanguage;
        SaveSettings();
        OnLanguageChanged?.Invoke(useFilipino);
        
        Debug.Log($"Language set to {(useFilipino ? "Filipino" : "English")}");
    }
    
    public void ToggleHapticFeedback(bool enabled)
    {
        hapticFeedbackEnabled = enabled;
        SaveSettings();
        OnHapticToggled?.Invoke(enabled);
        
        Debug.Log($"Haptic Feedback {(enabled ? "enabled" : "disabled")}");
    }
    
    void UpdateAudioSettings()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.mute = !musicEnabled;
            musicAudioSource.volume = musicVolume;
        }
        
        if (soundEffectsAudioSource != null)
        {
            soundEffectsAudioSource.mute = !soundEffectsEnabled;
            soundEffectsAudioSource.volume = soundEffectsVolume;
        }
    }
    
    public void PlaySoundEffect(AudioClip clip)
    {
        if (soundEffectsEnabled && soundEffectsAudioSource != null && clip != null)
        {
            soundEffectsAudioSource.PlayOneShot(clip, soundEffectsVolume);
        }
    }
    
    public void TriggerHapticFeedback()
    {
        if (hapticFeedbackEnabled)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
            #elif UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
            #else
            // For testing in editor, you can add a debug log
            Debug.Log("Haptic feedback triggered (Editor mode)");
            #endif
        }
    }
    
    void SaveSettings()
    {
        PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt(SOUND_EFFECTS_ENABLED_KEY, soundEffectsEnabled ? 1 : 0);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SOUND_EFFECTS_VOLUME_KEY, soundEffectsVolume);
        PlayerPrefs.SetInt(USE_FILIPINO_KEY, useFilipino ? 1 : 0);
        PlayerPrefs.SetInt(HAPTIC_ENABLED_KEY, hapticFeedbackEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    void LoadSettings()
    {
        musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
        soundEffectsEnabled = PlayerPrefs.GetInt(SOUND_EFFECTS_ENABLED_KEY, 1) == 1;
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        soundEffectsVolume = PlayerPrefs.GetFloat(SOUND_EFFECTS_VOLUME_KEY, 1f);
        useFilipino = PlayerPrefs.GetInt(USE_FILIPINO_KEY, 1) == 1;
        hapticFeedbackEnabled = PlayerPrefs.GetInt(HAPTIC_ENABLED_KEY, 1) == 1;
    }
    
    // Public getters for other scripts
    public bool IsMusicEnabled() => musicEnabled;
    public bool AreSoundEffectsEnabled() => soundEffectsEnabled;
    public bool IsFilipinoLanguage() => useFilipino;
    public bool IsHapticEnabled() => hapticFeedbackEnabled;
    public float GetMusicVolume() => musicVolume;
    public float GetSoundEffectsVolume() => soundEffectsVolume;
}


