using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuOptions : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button startGameButton;
    public Button optionsButton;
    public Button quitButton;
    
    [Header("Options Panel")]
    public GameObject optionsPanel;
    public OptionsMenu optionsMenu;
    
    void Start()
    {
        SetupButtons();
        
        // Hide options panel initially
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
    
    void SetupButtons()
    {
        // Setup start game button
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        
        // Setup options button
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OpenOptions);
        }
        
        // Setup quit button
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }
    
    public void StartGame()
    {
        // Play button sound
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        // Load the main game scene or module selection
        SceneManager.LoadScene("Module 1"); // Adjust scene name as needed
    }
    
    public void OpenOptions()
    {
        // Play button sound
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        // Show options panel
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
        
        // If using OptionsMenu component, trigger it
        if (optionsMenu != null)
        {
            optionsMenu.ToggleOptions();
        }
    }
    
    public void QuitGame()
    {
        // Play button sound
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerHapticFeedback();
        }
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}


