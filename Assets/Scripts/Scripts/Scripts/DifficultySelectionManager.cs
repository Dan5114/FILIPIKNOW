using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DifficultySelectionManager : MonoBehaviour
{
    [Header("UI References")]
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;
    public Button backButton;
    
    [Header("Topic Information")]
    public TextMeshProUGUI topicTitle;
    public TextMeshProUGUI topicDescription;
    public Image topicIcon;
    
    [Header("Difficulty Buttons")]
    public GameObject easyButtonGO;
    public GameObject mediumButtonGO;
    public GameObject hardButtonGO;
    
    [Header("Button Images")]
    [Space(10)]
    [Tooltip("Normal state images for difficulty buttons")]
    public Sprite easyNormalSprite;
    public Sprite mediumNormalSprite;
    public Sprite hardNormalSprite;
    
    [Tooltip("Highlighted/Clicked state images for difficulty buttons")]
    public Sprite easyHighlightedSprite;
    public Sprite mediumHighlightedSprite;
    public Sprite hardHighlightedSprite;
    
    [Header("Progress Indicators")]
    public GameObject easyCompletedIcon;
    public GameObject mediumCompletedIcon;
    public GameObject hardCompletedIcon;
    
    [Header("Level Status Icons")]
    [Space(10)]
    [Tooltip("Locked level icons - normal and highlighted states")]
    public Sprite lockedLevelNormalIcon;
    public Sprite lockedLevelHighlightedIcon;
    
    [Tooltip("Unlocked level icons - normal and highlighted states")]
    public Sprite unlockedLevelNormalIcon;
    public Sprite unlockedLevelHighlightedIcon;
    
    [Tooltip("Icon UI components for each difficulty")]
    public Image easyLevelIcon;
    public Image mediumLevelIcon;
    public Image hardLevelIcon;
    
    [Header("Neon Green Icon Settings")]
    [Space(10)]
    [Tooltip("Enable neon green pixelated style for icons")]
    public bool useNeonGreenStyle = true;
    [Tooltip("Neon green color for unlocked icons")]
    public Color neonGreenColor = new Color(0.0f, 1.0f, 0.2f, 1.0f); // Bright neon green
    [Tooltip("Neon red color for locked icons")]
    public Color neonRedColor = new Color(1.0f, 0.0f, 0.2f, 1.0f); // Bright neon red
    [Tooltip("Icon size for neon green blocks")]
    public Vector2 iconSize = new Vector2(32f, 32f);
    
    [Header("Configuration")]
    public string topicName;
    public string gameSceneName; // Single scene for all difficulties
    
    [Header("Universal Font")]
    public bool useUniversalFont = true;
    
    [Header("Typewriter Effect")]
    public TypewriterEffect titleTypewriterEffect;
    public bool useTypewriterForTitle = true;
    public float typingSpeed = 0.05f;
    
    private TopicProgress topicProgress;
    
    void Start()
    {
        Debug.Log($"🎯 DifficultySelectionManager starting for topic: {topicName}");
        Debug.Log($"🎯 Game scene name: {gameSceneName}");
        
        SetupUniversalFont();
        SetupUI();
        // SetupButtonImages();
        LoadTopicProgress();
        // UpdateButtonStates();
    }
    
    void SetupUniversalFont()
    {
        if (useUniversalFont && FilipknowFontManager.Instance != null)
        {
            Debug.Log("🎨 Applying universal Minecraft font to DifficultySelectionManager");
            
            // Apply universal font to topic title
            if (topicTitle != null)
            {
                topicTitle.font = FilipknowFontManager.Instance.GetCurrentFont();
                topicTitle.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                topicTitle.color = FilipknowFontManager.Instance.GetCurrentFontColor();
                
                // Force refresh the text component
                topicTitle.SetAllDirty();
                
                // Also apply font to typewriter effect if available
                if (titleTypewriterEffect != null)
                {
                    titleTypewriterEffect.textComponent = topicTitle;
                    Debug.Log("🎬 Typewriter effect configured for title");
                }
                
                Debug.Log($"✅ Applied universal font to topic title: {topicTitle.name}");
                Debug.Log($"   Font: {topicTitle.font?.name ?? "NULL"}");
                Debug.Log($"   Font Size: {topicTitle.fontSize}");
                Debug.Log($"   Color: {topicTitle.color}");
            }
            else
            {
                Debug.LogWarning("⚠️ TopicTitle is null - cannot apply universal font");
            }
            
            // Apply universal font to topic description
            if (topicDescription != null)
            {
                topicDescription.font = FilipknowFontManager.Instance.GetCurrentFont();
                topicDescription.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                topicDescription.color = FilipknowFontManager.Instance.GetCurrentFontColor();
                topicDescription.SetAllDirty();
                Debug.Log($"✅ Applied universal font to topic description: {topicDescription.name}");
            }
            
            // Apply universal font to difficulty button texts
            ApplyFontToButton(easyButton, "Easy");
            ApplyFontToButton(mediumButton, "Medium");
            ApplyFontToButton(hardButton, "Hard");
            ApplyFontToButton(backButton, "Back");
            
            Debug.Log("🎨 Universal Minecraft font applied to all UI elements");
        }
        else if (!useUniversalFont)
        {
            Debug.Log("ℹ️ Universal font disabled for DifficultySelectionManager");
        }
        else
        {
            Debug.LogWarning("⚠️ FilipknowFontManager.Instance is null - cannot apply universal font");
        }
    }
    
    void ApplyFontToButton(Button button, string buttonName)
    {
        if (button != null)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.font = FilipknowFontManager.Instance.GetCurrentFont();
                buttonText.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                buttonText.color = FilipknowFontManager.Instance.GetCurrentFontColor();
                Debug.Log($"✅ Applied universal font to {buttonName} button text");
            }
            else
            {
                Debug.LogWarning($"⚠️ No TextMeshProUGUI found in {buttonName} button");
            }
        }
    }
    
    void SetupUI()
    {
        // Set topic title with typewriter effect
        if (topicTitle != null)
        {
            if (useTypewriterForTitle && titleTypewriterEffect != null)
            {
                // Use typewriter effect for title
                titleTypewriterEffect.textComponent = topicTitle;
                titleTypewriterEffect.typingSpeed = typingSpeed;
                
                // Ensure font is applied before starting typewriter
                if (useUniversalFont && FilipknowFontManager.Instance != null)
                {
                    titleTypewriterEffect.textComponent.font = FilipknowFontManager.Instance.GetCurrentFont();
                    titleTypewriterEffect.textComponent.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                    titleTypewriterEffect.textComponent.color = FilipknowFontManager.Instance.GetCurrentFontColor();
                }
                
                titleTypewriterEffect.StartTypewriter(topicName);
                Debug.Log("🎬 Started typewriter effect for title");
            }
            else
            {
                // Set title text directly
                topicTitle.text = topicName;
                Debug.Log("📝 Set title text directly");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ TopicTitle is null - cannot set title text");
        }
        
        // Set topic description
        if (topicDescription != null)
        {
            topicDescription.text = GetTopicDescription();
        }
        
        // Setup button listeners
        if (easyButton != null)
            easyButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Easy));
        
        if (mediumButton != null)
            mediumButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Medium));
        
        if (hardButton != null)
            hardButton.onClick.AddListener(() => SelectDifficulty(DifficultyLevel.Hard));
        
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
    }
    
    // void SetupButtonImages()
    // {
    //     // Load images from Resources if not assigned
    //     LoadImagesFromResources();
        
    //     // Setup Easy button images
    //     SetupButtonImageStates(easyButton, easyNormalSprite, easyHighlightedSprite);
        
    //     // Setup Medium button images
    //     SetupButtonImageStates(mediumButton, mediumNormalSprite, mediumHighlightedSprite);
        
    //     // Setup Hard button images
    //     SetupButtonImageStates(hardButton, hardNormalSprite, hardHighlightedSprite);
        
    //     Debug.Log("🎨 Button images configured for all difficulty levels");
    // }
    
    // void LoadImagesFromResources()
    // {
    //     // Load Easy button images
    //     if (easyNormalSprite == null)
    //     {
    //         easyNormalSprite = Resources.Load<Sprite>("DifficultyButtons/Easy_Normal");
    //         if (easyNormalSprite != null) Debug.Log("✅ Loaded Easy Normal sprite from Resources");
    //     }
        
    //     if (easyHighlightedSprite == null)
    //     {
    //         easyHighlightedSprite = Resources.Load<Sprite>("DifficultyButtons/Easy_Highlighted");
    //         if (easyHighlightedSprite != null) Debug.Log("✅ Loaded Easy Highlighted sprite from Resources");
    //     }
        
    //     // Load Medium button images
    //     if (mediumNormalSprite == null)
    //     {
    //         mediumNormalSprite = Resources.Load<Sprite>("DifficultyButtons/Medium_Normal");
    //         if (mediumNormalSprite != null) Debug.Log("✅ Loaded Medium Normal sprite from Resources");
    //     }
        
    //     if (mediumHighlightedSprite == null)
    //     {
    //         mediumHighlightedSprite = Resources.Load<Sprite>("DifficultyButtons/Medium_Highlighted");
    //         if (mediumHighlightedSprite != null) Debug.Log("✅ Loaded Medium Highlighted sprite from Resources");
    //     }
        
    //     // Load Hard button images
    //     if (hardNormalSprite == null)
    //     {
    //         hardNormalSprite = Resources.Load<Sprite>("DifficultyButtons/Hard_Normal");
    //         if (hardNormalSprite != null) Debug.Log("✅ Loaded Hard Normal sprite from Resources");
    //     }
        
    //     if (hardHighlightedSprite == null)
    //     {
    //         hardHighlightedSprite = Resources.Load<Sprite>("DifficultyButtons/Hard_Highlighted");
    //         if (hardHighlightedSprite != null) Debug.Log("✅ Loaded Hard Highlighted sprite from Resources");
    //     }
        
    //     // Load level status icons from Resources
    //     LoadLevelIconsFromResources();
    // }
    
    // void LoadLevelIconsFromResources()
    // {
    //     // Load locked level icons (padlock icons)
    //     if (lockedLevelNormalIcon == null)
    //     {
    //         lockedLevelNormalIcon = Resources.Load<Sprite>("LevelIcons/Locked_Normal");
    //         if (lockedLevelNormalIcon != null) Debug.Log("🔒 Loaded Locked Normal (padlock) icon from Resources");
    //     }
        
    //     if (lockedLevelHighlightedIcon == null)
    //     {
    //         lockedLevelHighlightedIcon = Resources.Load<Sprite>("LevelIcons/Locked_Highlighted");
    //         if (lockedLevelHighlightedIcon != null) Debug.Log("🔒 Loaded Locked Highlighted (padlock) icon from Resources");
    //     }
        
    //     // Load unlocked level icons (exclamation icons)
    //     if (unlockedLevelNormalIcon == null)
    //     {
    //         unlockedLevelNormalIcon = Resources.Load<Sprite>("LevelIcons/Unlocked_Normal");
    //         if (unlockedLevelNormalIcon != null) Debug.Log("🔓 Loaded Unlocked Normal (exclamation) icon from Resources");
    //     }
        
    //     if (unlockedLevelHighlightedIcon == null)
    //     {
    //         unlockedLevelHighlightedIcon = Resources.Load<Sprite>("LevelIcons/Unlocked_Highlighted");
    //         if (unlockedLevelHighlightedIcon != null) Debug.Log("🔓 Loaded Unlocked Highlighted (exclamation) icon from Resources");
    //     }
    // }
    
    void SetupButtonImageStates(Button button, Sprite normalSprite, Sprite highlightedSprite)
    {
        if (button == null) return;
        
        // Set the normal sprite
        if (normalSprite != null)
        {
            button.image.sprite = normalSprite;
        }
        
        // Configure button transitions for highlight effect
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        button.colors = colors;
        
        // Add pointer events for highlight effect
        if (highlightedSprite != null)
        {
            var trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            // Clear existing triggers
            trigger.triggers.Clear();
            
            // Pointer Enter - Show highlighted sprite
            var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((eventData) => {
                if (button.interactable)
                {
                    button.image.sprite = highlightedSprite;
                }
            });
            trigger.triggers.Add(pointerEnter);
            
            // Pointer Exit - Show normal sprite
            var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((eventData) => {
                if (button.interactable)
                {
                    button.image.sprite = normalSprite;
                }
            });
            trigger.triggers.Add(pointerExit);
            
            // Pointer Down - Keep highlighted sprite
            var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((eventData) => {
                if (button.interactable)
                {
                    button.image.sprite = highlightedSprite;
                }
            });
            trigger.triggers.Add(pointerDown);
            
            // Pointer Up - Return to normal sprite
            var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((eventData) => {
                if (button.interactable)
                {
                    button.image.sprite = normalSprite;
                }
            });
            trigger.triggers.Add(pointerUp);
        }
    }
    
    void LoadTopicProgress()
    {
        if (LearningProgressionManager.Instance != null)
        {
            topicProgress = LearningProgressionManager.Instance.GetTopicProgress(topicName);
        }
        else
        {
            // Fallback if LearningProgressionManager is not available
            topicProgress = new TopicProgress(topicName);
        }
    }
    
    void UpdateButtonStates()
    {
        // If no progress data available, make all buttons accessible for testing
        bool isEasyCompleted = topicProgress?.isEasyCompleted ?? false;
        bool isMediumCompleted = topicProgress?.isMediumCompleted ?? false;
        bool isHardCompleted = topicProgress?.isHardCompleted ?? false;
        
        // Easy button - always accessible
        UpdateButtonState(easyButtonGO, easyCompletedIcon, true, isEasyCompleted);
        
        // Medium button - always accessible for development (unlock logic disabled)
        bool canAccessMedium = true; // Always true for development
        UpdateButtonState(mediumButtonGO, mediumCompletedIcon, canAccessMedium, isMediumCompleted);
        
        // Hard button - always accessible for development (unlock logic disabled)
        bool canAccessHard = true; // Always true for development
        UpdateButtonState(hardButtonGO, hardCompletedIcon, canAccessHard, isHardCompleted);
    }
    
    void UpdateButtonState(GameObject buttonGO, GameObject completedIcon, bool isAccessible, bool isCompleted)
    {
        if (buttonGO == null) return;
        
        Button button = buttonGO.GetComponent<Button>();
        if (button == null) return;
        
        // Set interactability
        button.interactable = isAccessible;
        
        // Update visual state with custom images
        if (isAccessible)
        {
            // Available state - normal color and normal sprite
            button.image.color = Color.white;
            
            // Set appropriate normal sprite based on button
            Sprite normalSprite = GetNormalSpriteForButton(buttonGO);
            if (normalSprite != null)
            {
                button.image.sprite = normalSprite;
            }
            
            if (isCompleted)
            {
                // Completed state - slight green tint
                button.image.color = new Color(0.9f, 1f, 0.9f, 1f);
            }
        }
        else
        {
            // Locked state - gray tint and normal sprite
            button.image.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            
            // Keep normal sprite for locked state
            Sprite normalSprite = GetNormalSpriteForButton(buttonGO);
            if (normalSprite != null)
            {
                button.image.sprite = normalSprite;
            }
        }
        
        // Show/hide completed icon
        if (completedIcon != null)
        {
            completedIcon.SetActive(isCompleted);
        }
        
        // Update level status icon
        UpdateLevelStatusIcon(buttonGO, isAccessible, isCompleted);
        
        // Update button text
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            string baseText = GetDifficultyButtonText(buttonGO);
            string statusText = GetStatusText(isAccessible, isCompleted);
            buttonText.text = $"{baseText}\n{statusText}";
            
            // Ensure universal font is applied to button text
            if (useUniversalFont && FilipknowFontManager.Instance != null)
            {
                buttonText.font = FilipknowFontManager.Instance.GetCurrentFont();
                buttonText.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
                buttonText.color = FilipknowFontManager.Instance.GetCurrentFontColor();
            }
        }
    }
    
    Sprite GetNormalSpriteForButton(GameObject buttonGO)
    {
        if (buttonGO == easyButtonGO) return easyNormalSprite;
        if (buttonGO == mediumButtonGO) return mediumNormalSprite;
        if (buttonGO == hardButtonGO) return hardNormalSprite;
        return null;
    }
    
    void UpdateLevelStatusIcon(GameObject buttonGO, bool isAccessible, bool isCompleted)
    {
        Image levelIcon = GetLevelIconForButton(buttonGO);
        if (levelIcon == null) 
        {
            Debug.LogWarning($"⚠️ No level icon found for {buttonGO.name}. Make sure to assign easyLevelIcon, mediumLevelIcon, or hardLevelIcon in the Inspector.");
            return;
        }
        
        // Determine which icon to show based on the logic:
        // - Locked: Show padlock icon
        // - Unlocked (available): Show exclamation icon  
        // - Completed: Hide icon (show normal button)
        
        if (isCompleted)
        {
            // Completed levels: Hide the icon completely
            levelIcon.gameObject.SetActive(false);
            Debug.Log($"✅ Completed level - hiding icon for {buttonGO.name}");
        }
        else if (isAccessible)
        {
            // Available but not completed: Show exclamation icon
            levelIcon.sprite = unlockedLevelNormalIcon;
            levelIcon.gameObject.SetActive(true);
            
            // Setup highlight effect for the icon
            SetupIconHighlightEffect(levelIcon, unlockedLevelNormalIcon, unlockedLevelHighlightedIcon);
            
            Debug.Log($"🔓 Showing exclamation icon for available level: {buttonGO.name}");
        }
        else
        {
            // Locked: Show padlock icon
            levelIcon.sprite = lockedLevelNormalIcon;
            levelIcon.gameObject.SetActive(true);
            
            // Setup highlight effect for the icon
            SetupIconHighlightEffect(levelIcon, lockedLevelNormalIcon, lockedLevelHighlightedIcon);
            
            Debug.Log($"🔒 Showing padlock icon for locked level: {buttonGO.name}");
        }
        
        Debug.Log($"🔒 Updated level status icon for {buttonGO.name}: {(isCompleted ? "Completed (no icon)" : (isAccessible ? "Available (exclamation)" : "Locked (padlock)"))}");
    }
    
    Image GetLevelIconForButton(GameObject buttonGO)
    {
        if (buttonGO == easyButtonGO) return easyLevelIcon;
        if (buttonGO == mediumButtonGO) return mediumLevelIcon;
        if (buttonGO == hardButtonGO) return hardLevelIcon;
        return null;
    }
    
    void SetupIconHighlightEffect(Image iconImage, Sprite normalIcon, Sprite highlightedIcon)
    {
        if (iconImage == null || highlightedIcon == null) return;
        
        // Add EventTrigger for highlight effect
        var trigger = iconImage.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = iconImage.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Clear existing triggers
        trigger.triggers.Clear();
        
        // Pointer Enter - Show highlighted icon
        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => {
            iconImage.sprite = highlightedIcon;
        });
        trigger.triggers.Add(pointerEnter);
        
        // Pointer Exit - Show normal icon
        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => {
            iconImage.sprite = normalIcon;
        });
        trigger.triggers.Add(pointerExit);
    }
    
    
    string GetDifficultyButtonText(GameObject buttonGO)
    {
        if (buttonGO == easyButtonGO) return "🟢 EASY";
        if (buttonGO == mediumButtonGO) return "🟡 MEDIUM";
        if (buttonGO == hardButtonGO) return "🔴 HARD";
        return "";
    }
    
    string GetStatusText(bool isAccessible, bool isCompleted)
    {
        if (isCompleted) return "✅ COMPLETED";
        if (isAccessible) return "▶️ START";
        return "🔒 LOCKED";
    }
    
    void SelectDifficulty(DifficultyLevel difficulty)
    {
        // Check if difficulty is accessible
        if (LearningProgressionManager.Instance != null && !LearningProgressionManager.Instance.CanAccessLevel(topicName, difficulty))
        {
            ShowAccessDeniedMessage(difficulty);
            return;
        }
        
        // Store selected difficulty
        if (SceneController.Instance != null)
        {
            SceneController.Instance.SetSelectedDifficulty(difficulty);
            SceneController.Instance.SetSelectedTopic(topicName);
        }
        
        // Route to Introduction scene first, then to game
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"🎯 Loading {topicName}Introduction with {difficulty} difficulty");
            Debug.Log($"🎯 Topic: {topicName}, Selected Difficulty: {difficulty}");
            Debug.Log($"🔍 DEBUG: topicName == 'Pangngalan': {topicName == "Pangngalan"}");
            Debug.Log($"🔍 DEBUG: difficulty == Medium: {difficulty == DifficultyLevel.Medium}");
            Debug.Log($"🔍 DEBUG: isNounsTopic: {topicName == "Pangngalan" || topicName == "Nouns" || topicName == "nouns"}");
            
            // Special routing for Nouns difficulties
            // Check for various possible topic names for Nouns
            bool isNounsTopic = topicName == "Pangngalan" || topicName == "Nouns" || topicName == "nouns";
            if (isNounsTopic && difficulty == DifficultyLevel.Medium)
            {
                Debug.Log("🎯 Special routing: Nouns Medium → NounsIntroductionMedium");
                SceneManager.LoadScene("NounsIntroductionMedium");
            }
            else if (isNounsTopic && difficulty == DifficultyLevel.Hard)
            {
                Debug.Log("🎯 Special routing: Nouns Hard → NounsIntroductionHard");
                SceneManager.LoadScene("NounsIntroductionHard");
            }
            else
            {
                Debug.Log($"🎯 Standard routing: {topicName}Introduction");
                SceneManager.LoadScene($"{topicName}Introduction");
            }
        }
        else
        {
            Debug.LogError($"❌ gameSceneName is not set for {topicName} difficulty selection!");
            Debug.LogError($"❌ Please set gameSceneName in the DifficultySelectionManager Inspector!");
        }
    }
    
    void ShowAccessDeniedMessage(DifficultyLevel difficulty)
    {
        string requiredLevel = difficulty switch
        {
            DifficultyLevel.Medium => "Easy",
            DifficultyLevel.Hard => "Medium",
            _ => "Unknown"
        };
        
        string message = $"🔒 You can't access this level yet.\n\n" +
                        $"You must first complete the {requiredLevel} level.";
        
        // Show message using adaptive dialog if available
        AdaptiveDialogManager dialogManager = FindObjectOfType<AdaptiveDialogManager>();
        if (dialogManager != null)
        {
            dialogManager.ShowDialog(message, null);
        }
        else
        {
            Debug.Log(message);
        }
    }
    
    string GetTopicDescription()
    {
        return topicName switch
        {
            "Pangngalan" => "Mga salitang tumutukoy sa tao, lugar, bagay, o hayop.\nHalimbawa: aso, paaralan, Maria, tubig",
            "Pand'iwa" => "Mga salitang nagsasaad ng kilos o galaw.\nHalimbawa: tumakbo, kumain, naglaro, umiyak",
            "Pang-uri" => "Mga salitang naglalarawan sa pangngalan o panghalip.\nHalimbawa: maganda, malaki, mabait, mabilis",
            "Panghalip" => "Mga salitang ginagamit na pamalit sa pangngalan.\nHalimbawa: ako, ikaw, siya, kami, kayo, sila",
            "Pang-abay" => "Mga salitang naglalarawan sa pandiwa, pang-uri, o kapwa pang-abay.\nHalimbawa: mabilis, maingat, dahan-dahan",
            "Pang-ukol" => "Mga salitang nag-uugnay sa pangngalan sa iba pang salita sa pangungusap.\nHalimbawa: sa, ng, para sa, ayon sa",
            "Pangatnig" => "Mga salitang ginagamit na pang-ugnay sa mga salita, parirala, o sugnay.\nHalimbawa: at, ngunit, dahil, kung",
            "Pang-angkop" => "Mga salitang nag-uugnay sa mga salita na hindi magkatugma.\nHalimbawa: na, -ng",
            _ => "Paksa sa Filipino"
        };
    }
    
    public void GoBack()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.GoBack();
        }
        else
        {
            SceneManager.LoadScene("Modules Available");
        }
    }
    
    // Refresh progress when returning to this scene
    // void OnEnable()
    // {
    //     if (Application.isPlaying)
    //     {
    //         LoadTopicProgress();
    //         UpdateButtonStates();
    //     }
    // }
}
