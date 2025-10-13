using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject optionsPanel;
    public Button optionsButton;
    public Button closeOptionsButton;
    
    [Header("Music Settings")]
    public Toggle musicToggle;
    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    
    [Header("Sound Effects Settings")]
    public Toggle soundEffectsToggle;
    public Slider soundEffectsVolumeSlider;
    public TextMeshProUGUI soundEffectsVolumeText;
    
    [Header("Language Settings")]
    public Toggle languageToggle;
    public TextMeshProUGUI languageToggleText;
    
    [Header("Haptic Settings")]
    public Toggle hapticToggle;
    
    [Header("Audio Clips")]
    public AudioClip buttonClickSound;
    public AudioClip correctAnswerSound;
    public AudioClip incorrectAnswerSound;
    
    private bool isOptionsOpen = false;
    
    void Start()
    {
        InitializeUI();
        SetupEventListeners();
        LoadCurrentSettings();
        
        // Subscribe to settings changes
        SettingsManager.OnMusicToggled += OnMusicToggled;
        SettingsManager.OnSoundEffectsToggled += OnSoundEffectsToggled;
        SettingsManager.OnLanguageChanged += OnLanguageChanged;
        SettingsManager.OnHapticToggled += OnHapticToggled;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        SettingsManager.OnMusicToggled -= OnMusicToggled;
        SettingsManager.OnSoundEffectsToggled -= OnSoundEffectsToggled;
        SettingsManager.OnLanguageChanged -= OnLanguageChanged;
        SettingsManager.OnHapticToggled -= OnHapticToggled;
    }
    
    void InitializeUI()
    {
        // Hide options panel initially
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        
        // Setup language toggle text
        UpdateLanguageToggleText();
    }
    
    void SetupEventListeners()
    {
        // Options button
        if (optionsButton != null)
            optionsButton.onClick.AddListener(ToggleOptions);
        
        // Close button
        if (closeOptionsButton != null)
            closeOptionsButton.onClick.AddListener(CloseOptions);
        
        // Music settings
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        // Sound Effects settings
        if (soundEffectsToggle != null)
            soundEffectsToggle.onValueChanged.AddListener(OnSoundEffectsToggleChanged);
        
        if (soundEffectsVolumeSlider != null)
            soundEffectsVolumeSlider.onValueChanged.AddListener(OnSoundEffectsVolumeChanged);
        
        // Language settings
        if (languageToggle != null)
            languageToggle.onValueChanged.AddListener(OnLanguageToggleChanged);
        
        // Haptic settings
        if (hapticToggle != null)
            hapticToggle.onValueChanged.AddListener(OnHapticToggleChanged);
    }
    
    void LoadCurrentSettings()
    {
        if (SettingsManager.Instance == null) return;
        
        // Load current settings into UI
        if (musicToggle != null)
            musicToggle.isOn = SettingsManager.Instance.IsMusicEnabled();
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = SettingsManager.Instance.GetMusicVolume();
            UpdateMusicVolumeText(SettingsManager.Instance.GetMusicVolume());
        }
        
        if (soundEffectsToggle != null)
            soundEffectsToggle.isOn = SettingsManager.Instance.AreSoundEffectsEnabled();
        
        if (soundEffectsVolumeSlider != null)
        {
            soundEffectsVolumeSlider.value = SettingsManager.Instance.GetSoundEffectsVolume();
            UpdateSoundEffectsVolumeText(SettingsManager.Instance.GetSoundEffectsVolume());
        }
        
        if (languageToggle != null)
            languageToggle.isOn = SettingsManager.Instance.IsFilipinoLanguage();
        
        if (hapticToggle != null)
            hapticToggle.isOn = SettingsManager.Instance.IsHapticEnabled();
    }
    
    public void ToggleOptions()
    {
        PlayButtonSound();
        TriggerHapticFeedback();
        
        isOptionsOpen = !isOptionsOpen;
        if (optionsPanel != null)
            optionsPanel.SetActive(isOptionsOpen);
        
        Debug.Log($"Options panel {(isOptionsOpen ? "opened" : "closed")}");
    }
    
    public void CloseOptions()
    {
        PlayButtonSound();
        TriggerHapticFeedback();
        
        isOptionsOpen = false;
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
    
    void OnMusicToggleChanged(bool isOn)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ToggleMusic(isOn);
            PlayButtonSound();
        }
    }
    
    void OnMusicVolumeChanged(float volume)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMusicVolume(volume);
            UpdateMusicVolumeText(volume);
        }
    }
    
    void OnSoundEffectsToggleChanged(bool isOn)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ToggleSoundEffects(isOn);
            PlayButtonSound();
        }
    }
    
    void OnSoundEffectsVolumeChanged(float volume)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSoundEffectsVolume(volume);
            UpdateSoundEffectsVolumeText(volume);
        }
    }
    
    void OnLanguageToggleChanged(bool useFilipino)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ToggleLanguage(useFilipino);
            UpdateLanguageToggleText();
            PlayButtonSound();
        }
    }
    
    void OnHapticToggleChanged(bool isOn)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ToggleHapticFeedback(isOn);
            PlayButtonSound();
        }
    }
    
    void UpdateMusicVolumeText(float volume)
    {
        if (musicVolumeText != null)
            musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    
    void UpdateSoundEffectsVolumeText(float volume)
    {
        if (soundEffectsVolumeText != null)
            soundEffectsVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    
    void UpdateLanguageToggleText()
    {
        if (languageToggleText != null && SettingsManager.Instance != null)
        {
            languageToggleText.text = SettingsManager.Instance.IsFilipinoLanguage() ? "Filipino" : "English";
        }
    }
    
    void PlayButtonSound()
    {
        if (SettingsManager.Instance != null && buttonClickSound != null)
        {
            SettingsManager.Instance.PlaySoundEffect(buttonClickSound);
        }
    }
    
    void TriggerHapticFeedback()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
    }
    
    // Public methods for other scripts to play answer sounds
    public void PlayCorrectAnswerSound()
    {
        if (SettingsManager.Instance != null && correctAnswerSound != null)
        {
            SettingsManager.Instance.PlaySoundEffect(correctAnswerSound);
        }
    }
    
    public void PlayIncorrectAnswerSound()
    {
        if (SettingsManager.Instance != null && incorrectAnswerSound != null)
        {
            SettingsManager.Instance.PlaySoundEffect(incorrectAnswerSound);
        }
    }
    
    // Event handlers for settings changes
    void OnMusicToggled(bool enabled)
    {
        if (musicToggle != null)
            musicToggle.SetIsOnWithoutNotify(enabled);
    }
    
    void OnSoundEffectsToggled(bool enabled)
    {
        if (soundEffectsToggle != null)
            soundEffectsToggle.SetIsOnWithoutNotify(enabled);
    }
    
    void OnLanguageChanged(bool useFilipino)
    {
        if (languageToggle != null)
        {
            languageToggle.SetIsOnWithoutNotify(useFilipino);
            UpdateLanguageToggleText();
        }
    }
    
    void OnHapticToggled(bool enabled)
    {
        if (hapticToggle != null)
            hapticToggle.SetIsOnWithoutNotify(enabled);
    }
}


