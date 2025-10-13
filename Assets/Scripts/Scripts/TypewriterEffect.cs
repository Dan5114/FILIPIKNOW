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
    public bool enableAutoSizing = true;
    public float autoSizeMin = 30f;  // Increased for better readability
    public float autoSizeMax = 80f;  // Set to 80pt as requested
    
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
            
            // Fix text alignment and overflow settings
            textComponent.alignment = TMPro.TextAlignmentOptions.Center;
            textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
            
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
            textComponent.text = fullText;
        }
        
        // Stop character animation immediately when interrupted
        if (characterAnimator != null)
        {
            characterAnimator.SetBool("isSpeaking", false);
        }
        
        isTyping = false;
        OnTypingCompleted?.Invoke();
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
            // Add character to text
            textComponent.text += fullText[i];
            
            // Trigger character typed event
            OnCharacterTyped?.Invoke();
            
            // Play typing sound at intervals
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
        
        // Stop character speaking animation immediately when typing is done
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
            textComponent.alignment = TMPro.TextAlignmentOptions.Center;
            textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
            
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
        
        // Apply auto-sizing with normal settings
        if (enableAutoSizing)
        {
            textComponent.enableAutoSizing = true;
            textComponent.fontSizeMin = autoSizeMin;
            textComponent.fontSizeMax = autoSizeMax;
        }
        
        // Set alignment for dialog text
        textComponent.alignment = TMPro.TextAlignmentOptions.Center;
        textComponent.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
        textComponent.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
        
        // Enable word wrapping
        textComponent.enableWordWrapping = true;
        textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
        
        Debug.Log("TypewriterEffect: Configured for normal dialog mode");
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
