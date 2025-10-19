using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public GameObject settingsBackgroundOverlay;
    public Button settingsButton;
    public Button closeSettingsButton;
    
    [Header("Audio Settings")]
    public Toggle musicToggle;
    public Toggle soundEffectsToggle;
    public Slider musicVolumeSlider;
    public Slider soundEffectsVolumeSlider;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI soundEffectsVolumeText;
    
    [Header("Language Settings")]
    public Toggle englishToggle;
    public Toggle filipinoToggle;
    public TextMeshProUGUI languageLabel;
    
    // Removed Experience, Font, and Curriculum settings as per requirements
    // Haptic feedback always enabled
    // Correct/incorrect sounds included in sound effects toggle
    // Filipknow font always used
    
    // Settings Manager Reference
    private SettingsManager settingsManager;
    
    void Start()
    {
        // Get SettingsManager instance (try both singleton and direct reference)
        settingsManager = SettingsManager.Instance;
        if (settingsManager == null)
        {
            // Try to find SettingsManager in scene
            settingsManager = FindObjectOfType<SettingsManager>();
            if (settingsManager == null)
            {
                Debug.LogWarning("SettingsManager not found! Settings will work with PlayerPrefs directly.");
                // Continue without SettingsManager - we'll use PlayerPrefs directly
            }
        }
        
        if (settingsManager != null)
        {
            // Debug: List all available methods in SettingsManager
            Debug.Log("SettingsManager found! Available methods:");
            var methods = settingsManager.GetType().GetMethods();
            foreach (var method in methods)
            {
                if (method.IsPublic)
                {
                    Debug.Log($"- {method.Name}");
                }
            }
        }
        
        SetupUI();
        LoadSettings();
        SetupButtonListeners();
    }
    
    void Update()
    {
        // Close settings with Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel != null && settingsPanel.activeInHierarchy)
        {
            CloseSettings();
        }
    }
    
    void SetupUI()
    {
        // Ensure settings panel and overlay are initially hidden
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        if (settingsBackgroundOverlay != null)
        {
            settingsBackgroundOverlay.SetActive(false);
        }
        
        // Setup labels
        if (languageLabel != null)
            languageLabel.text = "Language Settings";
    }
    
    void SetupButtonListeners()
    {
        // Settings button
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }
        
        // Close settings button
        if (closeSettingsButton != null)
        {
            closeSettingsButton.onClick.AddListener(CloseSettings);
        }
        
        // Audio settings
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }
        
        if (soundEffectsToggle != null)
        {
            soundEffectsToggle.onValueChanged.AddListener(OnSoundEffectsToggleChanged);
        }
        
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        if (soundEffectsVolumeSlider != null)
        {
            soundEffectsVolumeSlider.onValueChanged.AddListener(OnSoundEffectsVolumeChanged);
        }
        
        // Language settings
        if (englishToggle != null)
        {
            englishToggle.onValueChanged.AddListener(OnEnglishToggleChanged);
        }
        
        if (filipinoToggle != null)
        {
            filipinoToggle.onValueChanged.AddListener(OnFilipinoToggleChanged);
        }
        
        // Removed experience, font, and curriculum settings as per requirements
    }
    
    void LoadSettings()
    {
        // Load audio settings with flexible method names or PlayerPrefs fallback
        if (musicToggle != null)
        {
            if (settingsManager != null)
            {
                try
                {
                    musicToggle.isOn = CallMethod<bool>("IsMusicEnabled", true);
                }
                catch
                {
                    Debug.LogWarning("IsMusicEnabled method not found, using PlayerPrefs");
                    musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
                }
            }
            else
            {
                musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            }
        }
        
        if (soundEffectsToggle != null)
        {
            if (settingsManager != null)
            {
                try
                {
                    soundEffectsToggle.isOn = CallMethod<bool>("IsSoundEffectsEnabled", true);
                }
                catch
                {
                    Debug.LogWarning("IsSoundEffectsEnabled method not found, using PlayerPrefs");
                    soundEffectsToggle.isOn = PlayerPrefs.GetInt("SoundEffectsEnabled", 1) == 1;
                }
            }
            else
            {
                soundEffectsToggle.isOn = PlayerPrefs.GetInt("SoundEffectsEnabled", 1) == 1;
            }
        }
        
        if (musicVolumeSlider != null)
        {
            if (settingsManager != null)
            {
                try
                {
                    musicVolumeSlider.value = CallMethod<float>("GetMusicVolume", 1.0f);
                }
                catch
                {
                    Debug.LogWarning("GetMusicVolume method not found, using PlayerPrefs");
                    musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
                }
            }
            else
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            }
        }
        
        // Load language settings with flexible method names or PlayerPrefs fallback
        if (englishToggle != null)
        {
            if (settingsManager != null)
            {
                try
                {
                    bool isFilipino = CallMethod<bool>("IsFilipinoLanguage", false);
                    englishToggle.isOn = !isFilipino;
                }
                catch
                {
                    Debug.LogWarning("IsFilipinoLanguage method not found, using PlayerPrefs");
                    bool isFilipino = PlayerPrefs.GetInt("IsFilipinoLanguage", 0) == 1;
                    englishToggle.isOn = !isFilipino;
                }
            }
            else
            {
                bool isFilipino = PlayerPrefs.GetInt("IsFilipinoLanguage", 0) == 1;
                englishToggle.isOn = !isFilipino;
            }
        }
        
        if (filipinoToggle != null)
        {
            if (settingsManager != null)
            {
                try
                {
                    filipinoToggle.isOn = CallMethod<bool>("IsFilipinoLanguage", false);
                }
                catch
                {
                    Debug.LogWarning("IsFilipinoLanguage method not found, using PlayerPrefs");
                    filipinoToggle.isOn = PlayerPrefs.GetInt("IsFilipinoLanguage", 0) == 1;
                }
            }
            else
            {
                filipinoToggle.isOn = PlayerPrefs.GetInt("IsFilipinoLanguage", 0) == 1;
            }
        }
        
        UpdateVolumeTexts();
    }
    
    // Helper method to call SettingsManager methods safely
    T CallMethod<T>(string methodName, T defaultValue)
    {
        try
        {
            var method = settingsManager.GetType().GetMethod(methodName);
            if (method != null)
            {
                return (T)method.Invoke(settingsManager, null);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error calling {methodName}: {e.Message}");
        }
        return defaultValue;
    }
    
    // Helper method to call SettingsManager void methods safely
    void CallVoidMethod(string methodName, params object[] parameters)
    {
        try
        {
            var method = settingsManager.GetType().GetMethod(methodName);
            if (method != null)
            {
                method.Invoke(settingsManager, parameters);
            }
            else
            {
                Debug.LogWarning($"Method {methodName} not found in SettingsManager");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error calling {methodName}: {e.Message}");
        }
    }
    
    // Button Actions
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        
        if (settingsBackgroundOverlay != null)
        {
            settingsBackgroundOverlay.SetActive(true);
        }
        
        Debug.Log("Settings panel opened");
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        
        if (settingsBackgroundOverlay != null)
        {
            settingsBackgroundOverlay.SetActive(false);
        }
        
        Debug.Log("Settings panel closed");
    }
    
    // Close settings when clicking background overlay
    public void OnBackgroundOverlayClicked()
    {
        CloseSettings();
    }
    
    // Audio Settings
    void OnMusicToggleChanged(bool isOn)
    {
        if (settingsManager != null)
        {
            CallVoidMethod("SetMusicEnabled", isOn);
        }
        else
        {
            PlayerPrefs.SetInt("MusicEnabled", isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        Debug.Log($"Music {(isOn ? "enabled" : "disabled")}");
    }
    
    void OnSoundEffectsToggleChanged(bool isOn)
    {
        if (settingsManager != null)
        {
            CallVoidMethod("SetSoundEffectsEnabled", isOn);
        }
        else
        {
            PlayerPrefs.SetInt("SoundEffectsEnabled", isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        Debug.Log($"Sound effects {(isOn ? "enabled" : "disabled")}");
    }
    
    void OnMusicVolumeChanged(float volume)
    {
        if (settingsManager != null)
        {
            CallVoidMethod("SetMusicVolume", volume);
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
        UpdateVolumeTexts();
        Debug.Log($"Music volume set to {volume:F1}");
    }
    
    void OnSoundEffectsVolumeChanged(float volume)
    {
        if (settingsManager != null)
        {
            CallVoidMethod("SetSoundEffectsVolume", volume);
        }
        else
        {
            PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
            PlayerPrefs.Save();
        }
        UpdateVolumeTexts();
        Debug.Log($"Sound effects volume set to {volume:F1}");
    }
    
    void UpdateVolumeTexts()
    {
        if (musicVolumeText != null && musicVolumeSlider != null)
        {
            musicVolumeText.text = $"Music Volume: {musicVolumeSlider.value:F0}%";
        }
        
        if (soundEffectsVolumeText != null && soundEffectsVolumeSlider != null)
        {
            soundEffectsVolumeText.text = $"Sound Effects Volume: {soundEffectsVolumeSlider.value:F0}%";
        }
    }
    
    // Language Settings
    void OnEnglishToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (settingsManager != null)
            {
                CallVoidMethod("SetFilipinoLanguage", false);
            }
            else
            {
                PlayerPrefs.SetInt("IsFilipinoLanguage", 0);
                PlayerPrefs.Save();
            }
            
            if (filipinoToggle != null)
                filipinoToggle.isOn = false;
            Debug.Log("Language set to English");
        }
    }
    
    void OnFilipinoToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (settingsManager != null)
            {
                CallVoidMethod("SetFilipinoLanguage", true);
            }
            else
            {
                PlayerPrefs.SetInt("IsFilipinoLanguage", 1);
                PlayerPrefs.Save();
            }
            
            if (englishToggle != null)
                englishToggle.isOn = false;
            Debug.Log("Language set to Filipino");
        }
    }
    
    // Removed experience, font, and curriculum settings methods as per requirements
    // Haptic feedback always enabled
    // Correct/incorrect sounds included in sound effects toggle
    // Filipknow font always used
    
    // Public methods for external access
    public void ResetToDefaults()
    {
        if (settingsManager != null)
        {
            CallVoidMethod("ResetToDefaults");
        }
        else
        {
            // Reset PlayerPrefs to defaults
            PlayerPrefs.SetInt("MusicEnabled", 1);
            PlayerPrefs.SetInt("SoundEffectsEnabled", 1);
            PlayerPrefs.SetFloat("MusicVolume", 1.0f);
            PlayerPrefs.SetFloat("SoundEffectsVolume", 1.0f);
            PlayerPrefs.SetInt("IsFilipinoLanguage", 0);
            PlayerPrefs.Save();
        }
        LoadSettings();
        Debug.Log("Settings reset to defaults");
    }
    
    public void SaveSettings()
    {
        if (settingsManager != null)
        {
            CallVoidMethod("SaveSettings");
        }
        else
        {
            PlayerPrefs.Save();
        }
        Debug.Log("Settings saved");
    }
}
