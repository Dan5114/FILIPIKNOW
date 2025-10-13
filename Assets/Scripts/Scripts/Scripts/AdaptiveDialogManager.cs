using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AdaptiveDialogManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public Button continueButton;
    public Button backButton;
    
    [Header("Typewriter Effect")]
    public TypewriterEffect typewriterEffect;
    
    [Header("Character Animation")]
    public Animator characterAnimator;
    
    [Header("Font Settings")]
    public TMP_FontAsset timesBoldFont;
    public bool useHighQualityText = true;
    public float baseFontSize = 18f;
    public float mobileFontSizeMultiplier = 1.2f;
    
    [Header("Auto-Sizing Settings")]
    public bool enableAutoSizing = true;
    public Vector2 minDialogSize = new Vector2(400f, 200f);
    public Vector2 maxDialogSize = new Vector2(800f, 600f);
    public float textPadding = 25f;
    public float autoSizeMin = 14f;
    public float autoSizeMax = 28f;
    public float resizeAnimationDuration = 0.3f;
    
    [Header("Session Summary Settings")]
    public bool useScrollableForLongText = true;
    public float longTextThreshold = 500f; // Characters
    public ScrollRect scrollRect;
    public GameObject scrollContent;
    
    private System.Action onDialogComplete;
    private bool isDialogActive = false;
    private RectTransform dialogRectTransform;
    private RectTransform textRectTransform;
    private ContentSizeFitter contentSizeFitter;
    private LayoutElement layoutElement;
    
    void Start()
    {
        InitializeComponents();
        SetupTypewriterEffect();
        SetupButtons();
        
        // Hide dialog initially
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }
    
    void InitializeComponents()
    {
        // Get dialog panel RectTransform
        if (dialogPanel != null)
        {
            dialogRectTransform = dialogPanel.GetComponent<RectTransform>();
        }
        
        // Get text RectTransform and configure for high quality rendering
        if (dialogText != null)
        {
            textRectTransform = dialogText.GetComponent<RectTransform>();
            
            // Configure text for high quality rendering
            ConfigureTextQuality();
            
            // Add ContentSizeFitter if not present
            contentSizeFitter = dialogText.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = dialogText.gameObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Add LayoutElement for better control
            layoutElement = dialogText.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = dialogText.gameObject.AddComponent<LayoutElement>();
            }
            layoutElement.preferredWidth = -1; // Use content size
            layoutElement.preferredHeight = -1; // Use content size
        }
        
        // Setup scroll rect for long text
        if (scrollRect == null)
        {
            scrollRect = dialogPanel?.GetComponentInChildren<ScrollRect>();
        }
        
        if (scrollContent == null && scrollRect != null)
        {
            scrollContent = scrollRect.content.gameObject;
        }
    }
    
    void ConfigureTextQuality()
    {
        if (dialogText == null) return;
        
        // Set font if available
        if (timesBoldFont != null)
        {
            dialogText.font = timesBoldFont;
        }
        
        // Configure for high quality text rendering
        if (useHighQualityText)
        {
            // Enable auto-sizing
            dialogText.enableAutoSizing = true;
            dialogText.fontSizeMin = autoSizeMin;
            dialogText.fontSizeMax = autoSizeMax;
            
            // Set base font size with mobile multiplier
            float targetFontSize = baseFontSize;
            #if UNITY_ANDROID || UNITY_IOS
            targetFontSize *= mobileFontSizeMultiplier;
            #endif
            dialogText.fontSize = targetFontSize;
            
            // Improve text quality settings
            dialogText.fontStyle = FontStyles.Normal;
            dialogText.color = Color.black;
            dialogText.alignment = TextAlignmentOptions.Center;
            
            // Enable word wrapping and rich text
            dialogText.enableWordWrapping = true;
            dialogText.richText = true;
            
            // Set text overflow handling
            dialogText.overflowMode = TextOverflowModes.Overflow;
            
            // Configure line spacing for better readability
            dialogText.lineSpacing = 1.2f;
            
            // Set paragraph spacing
            dialogText.paragraphSpacing = 8f;
            
            Debug.Log($"Configured text quality: fontSize={targetFontSize}, autoSizeMin={autoSizeMin}, autoSizeMax={autoSizeMax}");
        }
    }
    
    void SetupTypewriterEffect()
    {
        // Set up typewriter effect
        if (typewriterEffect == null)
            typewriterEffect = GetComponent<TypewriterEffect>();
            
        if (typewriterEffect != null)
        {
            typewriterEffect.textComponent = dialogText;
            typewriterEffect.characterAnimator = characterAnimator;
            typewriterEffect.SetFont(timesBoldFont);
            
            // Subscribe to typewriter events
            typewriterEffect.OnTypingStarted += OnTypingStarted;
            typewriterEffect.OnTypingCompleted += OnTypingCompleted;
        }
    }
    
    void SetupButtons()
    {
        // Set up buttons
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }
    
    public void ShowDialog(string message, System.Action onComplete = null)
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }
        
        onDialogComplete = onComplete;
        isDialogActive = true;
        
        // Hide continue button while typing
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
        
        // Configure for long text if needed
        ConfigureForTextLength(message);
        
        // Apply text quality settings before showing content
        ConfigureTextQuality();
        
        // Start typewriter effect
        if (typewriterEffect != null)
        {
            typewriterEffect.StartTypewriter(message);
        }
        else
        {
            // Fallback to instant text display
            if (dialogText != null)
            {
                dialogText.text = message;
                StartCoroutine(AdjustDialogSizeAfterTextUpdate());
            }
            OnTypingCompleted();
        }
    }
    
    void ConfigureForTextLength(string message)
    {
        bool isLongText = message.Length > longTextThreshold;
        
        if (useScrollableForLongText && isLongText)
        {
            // Enable scrolling for long text
            if (scrollRect != null)
            {
                scrollRect.gameObject.SetActive(true);
                scrollRect.vertical = true;
                scrollRect.horizontal = false;
            }
            
            // Adjust text settings for scrolling
            if (dialogText != null)
            {
                dialogText.enableAutoSizing = true;
                dialogText.fontSizeMin = autoSizeMin;
                dialogText.fontSizeMax = autoSizeMax;
                
                // Ensure text doesn't exceed dialog width
                if (textRectTransform != null)
                {
                    textRectTransform.sizeDelta = new Vector2(dialogRectTransform.sizeDelta.x - textPadding * 2, -1);
                }
            }
        }
        else
        {
            // Disable scrolling for short text
            if (scrollRect != null)
            {
                scrollRect.gameObject.SetActive(false);
            }
            
            // Use auto-sizing for dialog box
            if (enableAutoSizing)
            {
                StartCoroutine(AdjustDialogSizeForText(message));
            }
        }
    }
    
    IEnumerator AdjustDialogSizeForText(string text)
    {
        // Temporarily set the text to measure its size
        string originalText = dialogText.text;
        dialogText.text = text;
        
        // Force text mesh to update
        dialogText.ForceMeshUpdate();
        
        // Wait a frame for the text to update
        yield return null;
        
        // Get the preferred size of the text
        Vector2 textSize = dialogText.GetPreferredValues();
        
        // Calculate new dialog size
        Vector2 newDialogSize = new Vector2(
            Mathf.Clamp(textSize.x + textPadding * 2, minDialogSize.x, maxDialogSize.x),
            Mathf.Clamp(textSize.y + textPadding * 2, minDialogSize.y, maxDialogSize.y)
        );
        
        // Animate dialog size change
        yield return StartCoroutine(AnimateDialogSize(dialogRectTransform.sizeDelta, newDialogSize));
        
        // Restore original text if needed
        if (!string.IsNullOrEmpty(originalText) && originalText != text)
        {
            dialogText.text = originalText;
        }
    }
    
    IEnumerator AdjustDialogSizeAfterTextUpdate()
    {
        // Wait for text to update
        yield return null;
        yield return null;
        
        if (enableAutoSizing && dialogText != null)
        {
            // Get the preferred size of the text
            Vector2 textSize = dialogText.GetPreferredValues();
            
            // Calculate new dialog size
            Vector2 newDialogSize = new Vector2(
                Mathf.Clamp(textSize.x + textPadding * 2, minDialogSize.x, maxDialogSize.x),
                Mathf.Clamp(textSize.y + textPadding * 2, minDialogSize.y, maxDialogSize.y)
            );
            
            // Animate dialog size change
            yield return StartCoroutine(AnimateDialogSize(dialogRectTransform.sizeDelta, newDialogSize));
        }
    }
    
    IEnumerator AnimateDialogSize(Vector2 fromSize, Vector2 toSize)
    {
        float elapsed = 0f;
        
        while (elapsed < resizeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / resizeAnimationDuration;
            progress = Mathf.SmoothStep(0f, 1f, progress); // Smooth animation
            
            Vector2 currentSize = Vector2.Lerp(fromSize, toSize, progress);
            dialogRectTransform.sizeDelta = currentSize;
            
            yield return null;
        }
        
        dialogRectTransform.sizeDelta = toSize;
    }
    
    public void ShowIntroductionDialog(string introText, System.Action onComplete = null)
    {
        // Introduction dialogs should be scrollable to accommodate longer text
        bool originalScrollableSetting = useScrollableForLongText;
        float originalThreshold = longTextThreshold;
        
        useScrollableForLongText = true;
        longTextThreshold = 0f; // Force scrollable for introduction
        
        ShowDialog(introText, onComplete);
        
        // Restore original settings
        useScrollableForLongText = originalScrollableSetting;
        longTextThreshold = originalThreshold;
        
        Debug.Log($"Introduction dialog displayed with {introText.Length} characters (scrollable)");
    }
    
    public void ShowSessionSummary(string summaryText, System.Action onComplete = null)
    {
        // Session summaries are typically long, so force scrollable mode
        bool originalScrollableSetting = useScrollableForLongText;
        float originalThreshold = longTextThreshold;
        
        useScrollableForLongText = true;
        longTextThreshold = 0f; // Force scrollable for session summaries
        
        ShowDialog(summaryText, onComplete);
        
        // Restore original settings
        useScrollableForLongText = originalScrollableSetting;
        longTextThreshold = originalThreshold;
        
        Debug.Log($"Session summary displayed with {summaryText.Length} characters");
    }
    
    public void HideDialog()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        
        isDialogActive = false;
        onDialogComplete = null;
        
        // Stop any ongoing typewriter effect
        if (typewriterEffect != null)
        {
            typewriterEffect.StopTypewriter();
        }
        
        // Reset dialog size to default
        if (dialogRectTransform != null)
        {
            dialogRectTransform.sizeDelta = minDialogSize;
        }
        
        // Hide scroll rect
        if (scrollRect != null)
        {
            scrollRect.gameObject.SetActive(false);
        }
    }
    
    private void OnTypingStarted()
    {
        // Hide continue button while typing
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
    }
    
    private void OnTypingCompleted()
    {
        // Show continue button when typing is complete
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
        
        // Adjust dialog size after typing is complete
        if (enableAutoSizing)
        {
            StartCoroutine(AdjustDialogSizeAfterTextUpdate());
        }
    }
    
    private void OnContinueClicked()
    {
        if (isDialogActive)
        {
            HideDialog();
            onDialogComplete?.Invoke();
        }
    }
    
    private void OnBackClicked()
    {
        // Handle back button logic
        HideDialog();
    }
    
    // Public methods for external control
    public bool IsDialogActive()
    {
        return isDialogActive;
    }
    
    public void SetAutoSizing(bool enabled)
    {
        enableAutoSizing = enabled;
    }
    
    public void SetMinDialogSize(Vector2 size)
    {
        minDialogSize = size;
    }
    
    public void SetMaxDialogSize(Vector2 size)
    {
        maxDialogSize = size;
    }
    
    public void SetTextPadding(float padding)
    {
        textPadding = padding;
    }
    
    // Debug methods
    [ContextMenu("Test Long Text")]
    public void TestLongText()
    {
        string longText = "This is a test of the adaptive dialog system with a very long text that should trigger the auto-sizing and scrolling functionality. " +
                         "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                         "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. " +
                         "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
                         "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. " +
                         "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, " +
                         "eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.";
        
        ShowDialog(longText);
    }
    
    [ContextMenu("Test Session Summary")]
    public void TestSessionSummary()
    {
        string sessionSummary = "🎉 Tapos na ang session!\n\n" +
                               "📊 Session Accuracy: 85.5%\n" +
                               "⏱️ Average Response Time: 3.2s\n" +
                               "🏆 Total Puntos: 450\n\n" +
                               "📈 Overall Accuracy: 78.3%\n" +
                               "🎯 Module Score: 25\n" +
                               "🧠 Module Mastery: 65.2%\n" +
                               "🔥 Current Streak: 8\n" +
                               "⭐ Level: 12\n" +
                               "💎 Total XP: 1,250\n\n" +
                               "🎓 Learning Style: Visual\n" +
                               "💡 Tip: Focus on accuracy over speed\n\n" +
                               "🏅 Recent Achievements:\n" +
                               "• Speed Demon\n" +
                               "• Streak Master\n" +
                               "• Accuracy Expert\n\n" +
                               "🌟 Napakagaling! Perfect performance!";
        
        ShowSessionSummary(sessionSummary);
    }
}

