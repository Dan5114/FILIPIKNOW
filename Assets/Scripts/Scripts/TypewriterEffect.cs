using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI textComponent;
    public AudioSource audioSource;
    
    [Header("Character Animation")]
    public Animator characterAnimator;
    
    [Header("Font Settings")]
    public TMP_FontAsset timesBoldFont;
    
    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;  // Slightly slower for better readability
    public float punctuationDelay = 0.1f;  // Longer pause at punctuation
    public float sentenceDelay = 0.3f;  // Longer pause at sentence breaks
    
    [Header("Font Size")]
    public float fixedFontSize = 80f;  // Fixed font size for dialogue
    
    [Header("Auto-Scroll Settings")]
    public bool enableAutoScroll = true;  // Auto-scroll to follow latest text
    public UnityEngine.UI.ScrollRect scrollRect;  // Reference to ScrollRect for auto-scrolling
    
    [Header("Session Summary Settings")]
    public bool enableSessionSummaryMode = false;
    public float sessionSummaryFontSizeMin = 16f;
    public float sessionSummaryFontSizeMax = 24f;
    
    [Header("Skip Settings")]
    public bool enableDoubleTapToSkip = true;  // Enable double-tap/double-click to skip
    public float doubleTapThreshold = 0.5f;  // Time window for double tap (seconds)
    
    private string fullText;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool hasSwitchedToBottomAlignment = false;  // Track if we've switched alignment
    private bool userScrolledManually = false;  // Track if user manually scrolled
    private float lastAutoScrollPosition = 0f;  // Track last auto-scroll position
    
    // Double-tap detection
    private float lastTapTime = 0f;
    private int tapCount = 0;
    
    // Events for external scripts
    public System.Action OnTypingStarted;
    public System.Action OnTypingCompleted;
    public System.Action OnCharacterTyped;
    
    void Start()
    {
        // Get components if not assigned
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
            
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // Apply font and settings
        if (textComponent != null)
        {
            // Apply font (prioritize FilipknowMainFont)
            if (FilipknowFontManager.Instance != null)
            {
                TMP_FontAsset mainFont = FilipknowFontManager.Instance.GetCurrentFont();
                if (mainFont != null)
                {
                    textComponent.font = mainFont;
                    Debug.Log($"TypewriterEffect: Applied FilipknowMainFont {mainFont.name} to {textComponent.name}");
                }
            }
            else if (timesBoldFont != null)
            {
                textComponent.font = timesBoldFont;
                Debug.Log($"TypewriterEffect: Applied font {timesBoldFont.name} to {textComponent.name}");
            }
            else
            {
                Debug.LogWarning($"TypewriterEffect: No font assigned to {textComponent.name}");
            }
            
            // Use fixed font size
            textComponent.enableAutoSizing = false;
            textComponent.fontSize = fixedFontSize;
            
            // Remove all alignment controls - let text flow naturally
            
            // Set overflow settings
            textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
            textComponent.enableWordWrapping = true;
            
            // Ensure text is visible
            textComponent.color = Color.black;
            textComponent.enabled = true;
        }
        
        // Auto-find ScrollRect if not assigned
        if (scrollRect == null)
        {
            // Look for ScrollRect in parent hierarchy
            Transform parent = transform.parent;
            while (parent != null)
            {
                scrollRect = parent.GetComponent<UnityEngine.UI.ScrollRect>();
                if (scrollRect != null)
                {
                    Debug.Log($"🔍 Auto-found ScrollRect: {scrollRect.name}");
                    break;
                }
                parent = parent.parent;
            }
        }
        
        // Set up scroll listener to detect manual scrolling
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
        else
        {
            Debug.LogError("❌ ScrollRect not found! Please assign it in Inspector or ensure it's in parent hierarchy.");
        }
    }
    
    void OnDestroy()
    {
        // Clean up listener
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }
    }
    
    void Update()
    {
        // Double-tap/double-click detection to skip typewriter
        if (enableDoubleTapToSkip && isTyping)
        {
            bool tapDetected = false;
            
            // Check for mouse click (PC/Editor)
            if (Input.GetMouseButtonDown(0))
            {
                tapDetected = true;
            }
            
            // Check for touch (Mobile)
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                tapDetected = true;
            }
            
            if (tapDetected)
            {
                float currentTime = Time.time;
                
                // Check if this is within the double-tap time window
                if (currentTime - lastTapTime < doubleTapThreshold)
                {
                    tapCount++;
                    
                    // Double tap detected!
                    if (tapCount >= 2)
                    {
                        Debug.Log("⏩ Double-tap detected - skipping typewriter!");
                        SkipTypewriter();
                        tapCount = 0;
                    }
                }
                else
                {
                    // Reset tap count if too much time has passed
                    tapCount = 1;
                }
                
                lastTapTime = currentTime;
            }
        }
    }
    
    private void OnScrollChanged(UnityEngine.Vector2 scrollPosition)
    {
        // Detect if user manually scrolled (not from auto-scroll)
        if (scrollRect != null && isTyping)
        {
            float currentPos = scrollRect.verticalNormalizedPosition;
            
            // If scroll position changed and it wasn't from our auto-scroll, user scrolled manually
            if (Mathf.Abs(currentPos - lastAutoScrollPosition) > 0.01f && Mathf.Abs(currentPos - 0f) > 0.01f)
            {
                userScrolledManually = true;
                Debug.Log($"🛑 User scrolled manually - pausing auto-scroll (pos: {currentPos:F3})");
            }
            
            // If user scrolled back to bottom (within 1%), resume auto-scroll
            if (currentPos < 0.01f && userScrolledManually)
            {
                userScrolledManually = false;
                Debug.Log($"▶️ User returned to bottom - resuming auto-scroll (pos: {currentPos:F3})");
            }
        }
    }
    
    public void StartTypewriter(string text)
    {
        StartTypewriter(text, false);
    }
    
    public void StartTypewriter(string text, bool isSessionSummary)
    {
        if (isTyping)
        {
            StopTypewriter();
        }
        
        // Add null check for text parameter
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError("❌ TypewriterEffect: Cannot start typewriter with null or empty text!");
            return;
        }
        
        // Configure based on mode
        if (isSessionSummary || enableSessionSummaryMode)
        {
            ConfigureForSessionSummary();
        }
        else
        {
            ConfigureForNormalText();
        }
        
        fullText = text;
        textComponent.text = "";
        isTyping = true;
        hasSwitchedToBottomAlignment = false;  // Reset for new text
        userScrolledManually = false;  // Reset manual scroll flag for new text
        lastAutoScrollPosition = 0f;  // Reset auto-scroll position
        
        // Remove alignment control - let text flow naturally
        
        // Simple debug check
        if (scrollRect == null)
        {
            Debug.LogError("❌ ScrollRect not found!");
        }
        else
        {
            Debug.Log($"✅ ScrollRect found: {scrollRect.name}");
        }
        
        OnTypingStarted?.Invoke();
        typingCoroutine = StartCoroutine(TypeText());
    }
    
    public void StopTypewriter()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        // Complete the text immediately
        if (textComponent != null)
        {
            textComponent.text = fullText;
        }
        
        // Scroll disabled to prevent position reset
        
        isTyping = false;
        OnTypingCompleted?.Invoke();
    }
    
    /// <summary>
    /// Skip the typewriter effect and show full text immediately (triggered by double-tap)
    /// </summary>
    public void SkipTypewriter()
    {
        if (!isTyping) return;
        
        Debug.Log("⏩ Skipping typewriter effect...");
        
        // Stop the typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        // Show complete text immediately
        if (textComponent != null)
        {
            textComponent.text = fullText;
        }
        
        // Stop character speaking animation
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isSpeaking", false);
        }
        
        // Final scroll to bottom
        if (enableAutoScroll && scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
            lastAutoScrollPosition = 0f;
        }
        
        isTyping = false;
        OnTypingCompleted?.Invoke();
    }
    
    /// <summary>
    /// Coroutine to scroll to bottom after a frame delay
    /// </summary>
    private IEnumerator DelayedScrollToBottom()
    {
        yield return null; // Wait one frame for layout to update
        ScrollToBottom();
    }
    
    // Method to handle when user presses continue (interrupts typing)
    public void OnContinuePressed()
    {
        if (isTyping)
        {
            StopTypewriter();
        }
    }
    
    private IEnumerator TypeText()
    {
        // Start character speaking animation
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isSpeaking", true);
        }
        
        for (int i = 0; i < fullText.Length; i++)
        {
            // Add character to text
            textComponent.text += fullText[i];
            
            // Trigger character typed event
            OnCharacterTyped?.Invoke();
            
            // Auto-scroll to follow latest text (only if user hasn't manually scrolled)
            if (enableAutoScroll && scrollRect != null && !userScrolledManually)
            {
                // Scroll to bottom to show latest text
                scrollRect.verticalNormalizedPosition = 0f;
                lastAutoScrollPosition = 0f;
            }
            
            // Play typing sound
            if (GameAudioManager.Instance != null && !char.IsWhiteSpace(fullText[i]))
            {
                GameAudioManager.Instance.PlayTypingSound();
            }
            
            // Calculate delay based on character type
            float waitTime = typingSpeed;
            
            if (IsPunctuation(fullText[i]))
            {
                waitTime = punctuationDelay;
            }
            else if (fullText[i] == '\n')
            {
                waitTime = sentenceDelay;
            }
            
            yield return new WaitForSeconds(waitTime);
        }
        
        // Final scroll to show complete text
        if (enableAutoScroll && scrollRect != null && !userScrolledManually)
        {
            scrollRect.verticalNormalizedPosition = 0f;
            lastAutoScrollPosition = 0f;
        }
        
        // Stop character speaking animation
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isSpeaking", false);
        }
        
        isTyping = false;
        OnTypingCompleted?.Invoke();
    }
    
    private bool IsPunctuation(char character)
    {
        return character == '.' || character == '!' || character == '?' || 
               character == ',' || character == ';' || character == ':';
    }
    
    /// <summary>
    /// Simple scroll to bottom with layout update
    /// </summary>
    private void ScrollToBottom()
    {
        if (scrollRect != null && textComponent != null)
        {
            // Force text to update its size
            textComponent.ForceMeshUpdate();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            
            // Scroll to bottom (0 = bottom, 1 = top)
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    // Public methods for external control
    public bool IsTyping()
    {
        return isTyping;
    }
    
    // Session summary mode control methods
    public void EnableSessionSummaryMode()
    {
        enableSessionSummaryMode = true;
    }
    
    public void DisableSessionSummaryMode()
    {
        enableSessionSummaryMode = false;
    }
    
    // Public methods for external scripts
    public void SetFont(TMP_FontAsset font)
    {
        timesBoldFont = font;
        if (textComponent != null)
        {
            textComponent.font = font;
            Debug.Log($"TypewriterEffect: SetFont called - Applied {font.name} to {textComponent.name}");
        }
    }
    
    public void SetCharacterAnimator(Animator animator)
    {
        characterAnimator = animator;
    }
    
    public void SetTypingSpeed(float speed)
    {
        typingSpeed = speed;
    }
    
    public void RefreshUniversalFont()
    {
        if (FilipknowFontManager.Instance != null && textComponent != null)
        {
            textComponent.font = FilipknowFontManager.Instance.GetCurrentFont();
            textComponent.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
            textComponent.color = FilipknowFontManager.Instance.GetCurrentFontColor();
        }
    }
    
    public void VerifyCurrentFont()
    {
        if (textComponent != null)
        {
            Debug.Log($"TypewriterEffect: Current font on {textComponent.name}: {textComponent.font?.name ?? "NULL"}");
            Debug.Log($"TypewriterEffect: Assigned timesBoldFont: {timesBoldFont?.name ?? "NULL"}");
        }
    }
    
    public void ForceHorizontalText()
    {
        ForceHorizontalTextInternal();
    }
    
    /// <summary>
    /// Forces text to be horizontal (prevents vertical text issues)
    /// </summary>
    private void ForceHorizontalTextInternal()
    {
        if (textComponent == null) return;
        
        RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Ensure width is greater than height
            if (rectTransform.sizeDelta.x < rectTransform.sizeDelta.y)
            {
                float temp = rectTransform.sizeDelta.x;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y, temp);
            }
            
            // Remove alignment controls
            
            // Clear any rotation that might cause vertical text
            rectTransform.rotation = Quaternion.identity;
            
            // Re-apply the font after orientation changes
            if (timesBoldFont != null)
            {
                textComponent.font = timesBoldFont;
                Debug.Log($"TypewriterEffect: Re-applied font {timesBoldFont.name} after orientation fix");
            }
        }
    }
    
    /// <summary>
    /// Configures text for session summary mode (auto-sizing)
    /// </summary>
    private void ConfigureForSessionSummary()
    {
        if (textComponent == null) return;
        
        // Enable auto-sizing for session summaries
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = sessionSummaryFontSizeMin;
        textComponent.fontSizeMax = sessionSummaryFontSizeMax;
        
        // Remove alignment controls
        
        // Enable word wrapping for long text
        textComponent.enableWordWrapping = true;
        textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
        
        Debug.Log("TypewriterEffect: Configured for session summary mode (auto-sizing)");
    }
    
    /// <summary>
    /// Configures text for normal dialogue mode (fixed size with scrolling)
    /// </summary>
    private void ConfigureForNormalText()
    {
        if (textComponent == null) return;
        
        // Use fixed font size
        textComponent.enableAutoSizing = false;
        textComponent.fontSize = fixedFontSize;
        
        // Remove alignment controls
        
        // Enable word wrapping
        textComponent.enableWordWrapping = true;
        textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
        
        Debug.Log("TypewriterEffect: Configured for normal dialog mode (fixed size)");
    }
}
