using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;
    public float pauseAtPunctuation = 0.2f;
    
    [Header("Character Animation")]
    public Animator characterAnimator;
    public string speakingAnimationName = "Speaking";
    public string idleAnimationName = "Idle";
    
    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip typingSound;
    public float soundInterval = 0.1f;
    
    [Header("Font Settings")]
    public TMP_FontAsset timesBoldFont;
    
    [Header("Auto-Sizing")]
    public bool enableAutoSizing = false;  // Disabled to use scrolling instead
    public float fixedFontSize = 30f;  // Fixed font size for dialogue
    public float autoSizeMin = 30f;  // Increased for better readability
    public float autoSizeMax = 80f;  // Set to 80pt as requested
    
    [Header("Auto-Scroll Settings")]
    public bool enableAutoScroll = true;  // Auto-scroll to show latest text
    public UnityEngine.UI.ScrollRect scrollRect;  // Reference to ScrollRect for auto-scrolling
    public int scrollUpdateFrequency = 3;  // Update scroll every N characters (reduces lag)
    public float scrollThreshold = 0.1f;  // If scrolled up more than this (0-1), disable auto-scroll
    
    private bool userIsManuallyScrolling = false;  // Track if user is manually scrolling
    private float lastScrollPosition = 0f;  // Track last scroll position
    private bool isProgrammaticScroll = false;  // Track if scroll is from code (not user)
    private bool hasStartedScrolling = false;  // Track if we've switched to top alignment (don't switch back!)
    private bool autoScrollFinished = false;  // Flag to stop auto-scroll after typing
    private float savedScrollPosition = 0f;  // Save scroll position before layout changes
    private bool forceScrollPosition = false;  // Flag to force scroll position restoration
    
    [Header("Session Summary Settings")]
    public bool enableSessionSummaryMode = false;
    public float sessionSummaryFontSizeMin = 16f;  // Increased from 10f
    public float sessionSummaryFontSizeMax = 24f;  // Increased from 18f
    
    private string fullText;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
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
        
        // Apply font and auto-sizing
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
            
            // Apply auto-sizing for dialogue box (1133 x 673 pixels)
            if (enableAutoSizing)
            {
                textComponent.enableAutoSizing = true;
                textComponent.fontSizeMin = autoSizeMin;
                textComponent.fontSizeMax = autoSizeMax;
            }
            else
            {
                // Use fixed font size for scrolling
                textComponent.enableAutoSizing = false;
                textComponent.fontSize = fixedFontSize;
            }
            
            // Fix text alignment and overflow settings
            // Use NO vertical alignment constraints - let text flow naturally
            textComponent.alignment = TMPro.TextAlignmentOptions.Center;
            textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Top; // NO constraints!
            
            // Set overflow settings for proper text wrapping
            textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
            textComponent.enableWordWrapping = true;
            
            // Ensure text is visible and properly oriented
            textComponent.color = Color.black;
            textComponent.enabled = true;
            
            // Force text to display horizontally
            textComponent.text = "Test";
            textComponent.text = "";
            
            // Ensure horizontal text orientation
            ForceHorizontalText();
        }
        
        // Set up scroll listener if scrollRect is assigned
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
    }
    
    void Update()
    {
        // Continuously restore scroll position if user manually scrolled
        if (forceScrollPosition && scrollRect != null && userIsManuallyScrolling)
        {
            scrollRect.verticalNormalizedPosition = savedScrollPosition;
        }
    }
    
    void OnDestroy()
    {
        // Clean up scroll listener
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }
    }
    
    /// <summary>
    /// Called when the scroll position changes (user scrolling)
    /// </summary>
    private void OnScrollChanged(UnityEngine.Vector2 scrollPosition)
    {
        if (scrollRect == null) return;
        
        float currentScrollPos = scrollRect.verticalNormalizedPosition;
        
        // If this is a programmatic scroll, ignore it
        if (isProgrammaticScroll)
        {
            Debug.Log($"🤖 Programmatic scroll ignored (position: {currentScrollPos:F3})");
            isProgrammaticScroll = false;
            lastScrollPosition = currentScrollPos;
            return;
        }
        
        // STOP auto-scroll detection if typing is finished
        if (!isTyping)
        {
            Debug.Log($"⏹️ Typing finished - stopping scroll detection");
            return;
        }
        
        // Detect if user manually changed scroll
        // Check if the change is significant (more than 0.5%)
        float scrollDelta = Mathf.Abs(currentScrollPos - lastScrollPosition);
        if (scrollDelta > 0.005f)
        {
            Debug.Log($"📜 Scroll changed - delta: {scrollDelta:F3}, current: {currentScrollPos:F3}, last: {lastScrollPosition:F3}, threshold: {scrollThreshold}");
            
            // If user scrolled away from bottom (up), they're reading old text
            if (currentScrollPos > scrollThreshold)
            {
                if (!userIsManuallyScrolling)
                {
                    userIsManuallyScrolling = true;
                    forceScrollPosition = true; // Enable continuous position restoration
                    savedScrollPosition = currentScrollPos; // Save the position user scrolled to
                    Debug.Log($"🛑 MANUAL SCROLL DETECTED - PAUSED auto-scroll (position: {currentScrollPos:F3})");
                }
            }
            // If user scrolled back near bottom, resume auto-scroll
            else if (currentScrollPos <= scrollThreshold)
            {
                if (userIsManuallyScrolling)
                {
                    userIsManuallyScrolling = false;
                    forceScrollPosition = false; // Disable continuous position restoration
                    Debug.Log($"▶️ USER RETURNED TO BOTTOM - RESUMED auto-scroll (position: {currentScrollPos:F3})");
                }
            }
            
            lastScrollPosition = currentScrollPos;
        }
    }
    
    public void StartTypewriter(string text)
    {
        StartTypewriter(text, false);
    }
    
    public void StartTypewriter(string text, bool isSessionSummary)
    {
        // Safety check - ensure textComponent exists
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogError("TypewriterEffect: No TextMeshProUGUI component found!");
                return;
            }
        }
        
        if (isTyping)
        {
            StopTypewriter();
        }
        
        // Reset manual scrolling flags for new text
        userIsManuallyScrolling = false;
        hasStartedScrolling = false; // Allow new text to start centered if it's short
        autoScrollFinished = false; // Reset auto-scroll flag for new text
        
        // Configure based on mode
        if (isSessionSummary || enableSessionSummaryMode)
        {
            // Session summary mode (16-24pt auto-sizing)
            ConfigureForSessionSummary();
        }
        else
        {
            // Normal dialog mode (30-80pt auto-sizing)
            ConfigureForNormalText();
        }
        
        fullText = text;
        textComponent.text = "";
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
            
        typingCoroutine = StartCoroutine(TypeText());
    }
    
    public void StopTypewriter()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        // Complete the text immediately
        if (textComponent != null)
        {
            // Save scroll position before layout changes
            SaveScrollPosition();
            
            textComponent.text = fullText;
            
            // Restore scroll position after layout changes
            RestoreScrollPosition();
        }
        
        // Scroll to bottom when text is completed
        if (enableAutoScroll && scrollRect != null)
        {
            StartCoroutine(DelayedScrollToBottom());
        }
        
        // Stop character animation immediately when interrupted
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isSpeaking", false);
        }
        
        isTyping = false;
        autoScrollFinished = true; // Stop auto-scroll after typing finishes
        OnTypingCompleted?.Invoke();
    }
    
    /// <summary>
    /// Coroutine to scroll to bottom after a frame delay
    /// </summary>
    private IEnumerator DelayedScrollToBottom()
    {
        yield return null; // Wait one frame for layout to update
        ScrollToBottomComplete();
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
        // Ensure the text component is active before starting
        if (textComponent != null && !textComponent.gameObject.activeInHierarchy)
        {
            textComponent.gameObject.SetActive(true);
        }
        
        isTyping = true;
        OnTypingStarted?.Invoke();
        
        // Start character speaking animation immediately when typing starts
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isSpeaking", true);
        }
        
        float lastSoundTime = 0f;
        
        for (int i = 0; i < fullText.Length; i++)
        {
            // Save scroll position before layout changes (prevents Unity's auto-reset)
            SaveScrollPosition();
            
            // Add character to text
            textComponent.text += fullText[i];
            
            // Restore scroll position after layout changes
            RestoreScrollPosition();
            
            // Trigger character typed event
            OnCharacterTyped?.Invoke();
            
            // Auto-scroll to show latest text (only if user isn't manually scrolling and typing hasn't finished)
            if (enableAutoScroll && scrollRect != null && !userIsManuallyScrolling && !autoScrollFinished && i % scrollUpdateFrequency == 0)
            {
                ScrollToBottom();
            }
            
            // ⌨️ Play typing sound using GameAudioManager (for each non-space character)
            if (GameAudioManager.Instance != null && !char.IsWhiteSpace(fullText[i]))
            {
                GameAudioManager.Instance.PlayTypingSound();
            }
            
            // Play typing sound at intervals (fallback to old system)
            if (audioSource != null && typingSound != null && 
                Time.time - lastSoundTime >= soundInterval)
            {
                audioSource.PlayOneShot(typingSound);
                lastSoundTime = Time.time;
            }
            
            // Pause longer at punctuation
            float waitTime = typingSpeed;
            if (IsPunctuation(fullText[i]))
            {
                waitTime = pauseAtPunctuation;
            }
            
            yield return new WaitForSeconds(waitTime);
        }
        
        // Final scroll to ensure all text is visible (only if user didn't manually scroll up and typing hasn't finished)
        if (enableAutoScroll && scrollRect != null && !userIsManuallyScrolling && !autoScrollFinished)
        {
            yield return null; // Wait one frame for layout to update
            ScrollToBottomComplete();
        }
        
        // Stop character speaking animation immediately when typing is done
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isSpeaking", false);
        }
        
        isTyping = false;
        autoScrollFinished = true; // Stop auto-scroll after typing finishes
        
        // Reset manual scrolling flag after typing completes
        userIsManuallyScrolling = false;
        
        OnTypingCompleted?.Invoke();
    }
    
    private bool IsPunctuation(char character)
    {
        return character == '.' || character == '!' || character == '?' || 
               character == ',' || character == ';' || character == ':';
    }
    
    /// <summary>
    /// Save current scroll position before layout changes
    /// </summary>
    private void SaveScrollPosition()
    {
        if (scrollRect != null)
        {
            savedScrollPosition = scrollRect.verticalNormalizedPosition;
            Debug.Log($"💾 Saved scroll position: {savedScrollPosition:F3}");
        }
    }
    
    /// <summary>
    /// Restore scroll position after layout changes (prevents Unity's auto-reset to top)
    /// Uses delayed restoration to ensure it happens after Unity's layout rebuild
    /// </summary>
    private void RestoreScrollPosition()
    {
        if (scrollRect != null && userIsManuallyScrolling)
        {
            // Use delayed restoration to ensure it happens after Unity's layout rebuild
            StartCoroutine(DelayedRestoreScrollPosition());
        }
    }
    
    /// <summary>
    /// Delayed restoration of scroll position to override Unity's auto-reset
    /// </summary>
    private IEnumerator DelayedRestoreScrollPosition()
    {
        // Wait for Unity's layout rebuild to complete
        yield return null; // Wait one frame
        yield return null; // Wait another frame to be sure
        
        if (scrollRect != null && userIsManuallyScrolling)
        {
            scrollRect.verticalNormalizedPosition = savedScrollPosition;
            Debug.Log($"🔄 Delayed restore scroll position: {savedScrollPosition:F3}");
        }
    }
    
    /// <summary>
    /// Scrolls the ScrollRect to the bottom to show the latest text
    /// Dynamically adjusts alignment and scroll position
    /// ONLY scrolls if user is NOT manually scrolling
    /// </summary>
    private void ScrollToBottom()
    {
        if (scrollRect != null && scrollRect.content != null && textComponent != null)
        {
            // If user is manually scrolling, DON'T change anything
            if (userIsManuallyScrolling)
            {
                Debug.Log($"⏸️ Skipping auto-scroll - user is manually scrolling (position: {scrollRect.verticalNormalizedPosition:F3})");
                return; // Exit immediately, preserve user's scroll position
            }
            
            // Force text mesh to update
            textComponent.ForceMeshUpdate();
            
            // Rebuild content layout
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            
            // Force canvas update
            Canvas.ForceUpdateCanvases();
            
            // Calculate proper scroll position to show bottom content
            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport != null ? scrollRect.viewport.rect.height : 
                                  ((RectTransform)scrollRect.transform).rect.height;
            
            // Simple scroll to bottom - NO alignment changes!
            if (!userIsManuallyScrolling)
            {
                // Mark as programmatic scroll to prevent triggering manual scroll detection
                isProgrammaticScroll = true;
                lastScrollPosition = 0f;
                
                // Scroll to absolute bottom to show latest text
                scrollRect.verticalNormalizedPosition = 0f;
            }
            // If user is manually scrolling, DON'T change anything
        }
    }
    
    /// <summary>
    /// Complete scroll to bottom with layout updates - used at the end of typing
    /// Ensures latest text is visible and properly aligned
    /// </summary>
    private void ScrollToBottomComplete()
    {
        if (scrollRect != null && scrollRect.content != null && textComponent != null)
        {
            // If user is manually scrolling, DON'T override their position
            if (userIsManuallyScrolling)
            {
                Debug.Log($"⏸️ Skipping complete scroll - user is manually scrolling");
                return; // Exit immediately, preserve user's scroll position
            }
            
            // Force text mesh update first
            textComponent.ForceMeshUpdate();
            
            // Rebuild layout multiple times to ensure accuracy
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            Canvas.ForceUpdateCanvases();
            
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            Canvas.ForceUpdateCanvases();
            
            // Calculate the exact scroll position needed
            float contentHeight = scrollRect.content.rect.height;
            float viewportHeight = scrollRect.viewport != null ? scrollRect.viewport.rect.height : 
                                  ((RectTransform)scrollRect.transform).rect.height;
            
            // If content exceeds viewport OR we've already started scrolling, use bottom alignment
            // BUT ONLY if user hasn't manually scrolled
            if ((contentHeight > viewportHeight * 1.05f || hasStartedScrolling) && !userIsManuallyScrolling) // Small buffer
            {
                // Switch to bottom alignment for scrolling behavior (and STAY there)
                textComponent.alignment = TMPro.TextAlignmentOptions.BottomLeft;
                textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Bottom;
                hasStartedScrolling = true; // Lock to bottom alignment permanently
                
                // FORCE ScrollContent anchors to TOP programmatically
                if (scrollRect.content != null)
                {
                    RectTransform contentRect = scrollRect.content;
                    contentRect.anchorMin = new Vector2(0f, 1f); // Top-left
                    contentRect.anchorMax = new Vector2(1f, 1f); // Top-right
                    contentRect.pivot = new Vector2(0.5f, 1f); // Top-center pivot
                    Debug.Log($"🔧 [Complete] FORCED ScrollContent anchors to TOP");
                }
                
                // FORCE DialogText anchors to TOP programmatically
                RectTransform textRect = textComponent.GetComponent<RectTransform>();
                if (textRect != null)
                {
                    textRect.anchorMin = new Vector2(0f, 1f); // Top-left
                    textRect.anchorMax = new Vector2(1f, 1f); // Top-right
                    textRect.pivot = new Vector2(0.5f, 1f); // Top-center pivot
                    Debug.Log($"🔧 [Complete] FORCED DialogText anchors to TOP");
                }
                
                textComponent.ForceMeshUpdate();
                
                // Rebuild after alignment change
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
                Canvas.ForceUpdateCanvases();
                
                // Mark as programmatic scroll and update position BEFORE scrolling
                isProgrammaticScroll = true;
                lastScrollPosition = 0f;
                
                // Scroll to absolute bottom - shows latest text
                scrollRect.verticalNormalizedPosition = 0f;
                
                // Force immediate update
                Canvas.ForceUpdateCanvases();
                
                // Verify scroll position is correct
                float actualPosition = scrollRect.verticalNormalizedPosition;
                Debug.Log($"📜 Complete scroll: Latest text at bottom (content: {contentHeight:F1}, viewport: {viewportHeight:F1}, scroll pos: {actualPosition:F3})");
            }
            else if (!userIsManuallyScrolling)
            {
                // Content fits AND we haven't started scrolling, keep centered
                // BUT ONLY if user hasn't manually scrolled
                textComponent.alignment = TMPro.TextAlignmentOptions.Center;
                textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
                
                isProgrammaticScroll = true;
                lastScrollPosition = 1f;
                scrollRect.verticalNormalizedPosition = 1f;
                
                Debug.Log($"📜 Text fits in viewport - keeping centered (content: {contentHeight:F1}, viewport: {viewportHeight:F1})");
            }
            // If user is manually scrolling, DON'T change alignment or scroll position
        }
    }
    
    // Public methods for external control
    public bool IsTyping()
    {
        return isTyping;
    }
    
    public void SetTypingSpeed(float speed)
    {
        typingSpeed = speed;
    }
    
    public void SetCharacterAnimator(Animator animator)
    {
        characterAnimator = animator;
    }
    
    public void SetFont(TMP_FontAsset font)
    {
        timesBoldFont = font;
        if (textComponent != null)
        {
            textComponent.font = font;
            Debug.Log($"TypewriterEffect: SetFont called - Applied {font.name} to {textComponent.name}");
            Debug.Log($"TypewriterEffect: Current font after SetFont: {textComponent.font.name}");
        }
    }
    
    public void ForceHorizontalText()
    {
        if (textComponent != null)
        {
            // Ensure RectTransform is properly configured for horizontal text
            RectTransform rectTransform = textComponent.rectTransform;
            
            // Set proper pivot for horizontal text
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            // Ensure width is greater than height for horizontal text
            if (rectTransform.sizeDelta.x < rectTransform.sizeDelta.y)
            {
                float temp = rectTransform.sizeDelta.x;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y, temp);
            }
            
            // Force text alignment to horizontal
            textComponent.alignment = TMPro.TextAlignmentOptions.TopLeft;
            textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Top;
            
            // Clear any rotation that might cause vertical text
            rectTransform.rotation = Quaternion.identity;
            
            // Re-apply the font after orientation changes
            if (timesBoldFont != null)
            {
                textComponent.font = timesBoldFont;
                Debug.Log($"TypewriterEffect: Re-applied font {timesBoldFont.name} after orientation fix");
            }
            
            Debug.Log("TypewriterEffect: Forced horizontal text orientation");
        }
    }
    
    /// <summary>
    /// Refreshes the font from the universal font manager
    /// </summary>
    public void RefreshUniversalFont()
    {
        if (FilipknowFontManager.Instance != null && textComponent != null)
        {
            textComponent.font = FilipknowFontManager.Instance.GetCurrentFont();
            textComponent.fontSize = FilipknowFontManager.Instance.GetCurrentFontSize();
            textComponent.color = FilipknowFontManager.Instance.GetCurrentFontColor();
        }
    }
    
    /// <summary>
    /// Verifies and logs the current font being used
    /// </summary>
    public void VerifyCurrentFont()
    {
        if (textComponent != null)
        {
            Debug.Log($"TypewriterEffect: Current font on {textComponent.name}: {textComponent.font?.name ?? "NULL"}");
            Debug.Log($"TypewriterEffect: Assigned timesBoldFont: {timesBoldFont?.name ?? "NULL"}");
        }
    }
    
    /// <summary>
    /// Configures the text component for session summary display
    /// </summary>
    private void ConfigureForSessionSummary()
    {
        if (textComponent == null) return;
        
        // Enable auto-sizing with session summary specific settings
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = sessionSummaryFontSizeMin;
        textComponent.fontSizeMax = sessionSummaryFontSizeMax;
        
        // Adjust alignment for better readability of long text
        textComponent.alignment = TMPro.TextAlignmentOptions.TopLeft;
        textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Left;
        textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Top;
        
        // Enable word wrapping for long text
        textComponent.enableWordWrapping = true;
        textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
        
        Debug.Log("TypewriterEffect: Configured for session summary mode");
    }
    
    /// <summary>
    /// Configures the text component for normal dialog display
    /// </summary>
    private void ConfigureForNormalText()
    {
        if (textComponent == null) return;
        
        // Use fixed font size or auto-sizing based on settings
        if (enableAutoSizing)
        {
            textComponent.enableAutoSizing = true;
            textComponent.fontSizeMin = autoSizeMin;
            textComponent.fontSizeMax = autoSizeMax;
        }
        else
        {
            // Use fixed font size for scrolling
            textComponent.enableAutoSizing = false;
            textComponent.fontSize = fixedFontSize;
        }
        
        // Use NO vertical alignment constraints - let text flow naturally
        textComponent.alignment = TMPro.TextAlignmentOptions.Center;
        textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
        textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Top; // NO constraints!
        
        // Enable word wrapping
        textComponent.enableWordWrapping = true;
        textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
        
        Debug.Log("TypewriterEffect: Configured for normal dialog mode (dynamic alignment)");
    }
    
    
    /// <summary>
    /// Enables session summary mode for the next typewriter call
    /// </summary>
    public void EnableSessionSummaryMode()
    {
        enableSessionSummaryMode = true;
    }
    
    /// <summary>
    /// Disables session summary mode
    /// </summary>
    public void DisableSessionSummaryMode()
    {
        enableSessionSummaryMode = false;
    }
}
