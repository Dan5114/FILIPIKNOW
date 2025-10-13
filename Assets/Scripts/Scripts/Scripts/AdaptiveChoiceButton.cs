using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(ContentSizeFitter))]
[RequireComponent(typeof(LayoutElement))]
public class AdaptiveChoiceButton : MonoBehaviour
{
    [Header("UI References")]
    public Button button;
    public TextMeshProUGUI choiceText;
    public Image buttonImage;
    
    [Header("Auto-Sizing Settings")]
    public bool enableAutoSizing = true;
    public float minButtonWidth = 120f;
    public float maxButtonWidth = 300f;
    public float minButtonHeight = 40f;
    public float maxButtonHeight = 80f;
    public float textPadding = 20f;
    
    [Header("Font Settings")]
    public float textMinSize = 10f;
    public float textMaxSize = 18f;
    public bool enableTextAutoSizing = true;
    
    [Header("Animation Settings")]
    public float resizeAnimationDuration = 0.2f;
    public AnimationCurve resizeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Visual Settings")]
    public Color normalColor = new Color(1f, 1f, 1f, 1f);
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    private ContentSizeFitter contentSizeFitter;
    private LayoutElement layoutElement;
    private RectTransform rectTransform;
    private Vector2 originalSize;
    private bool isAnimating = false;
    
    // Events
    public System.Action<string> OnChoiceSelected;
    public System.Action<AdaptiveChoiceButton> OnButtonConfigured;
    
    void Awake()
    {
        // Get required components
        button = GetComponent<Button>();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        layoutElement = GetComponent<LayoutElement>();
        rectTransform = GetComponent<RectTransform>();
        
        // Get text component if not assigned
        if (choiceText == null)
            choiceText = GetComponentInChildren<TextMeshProUGUI>();
            
        // Get image component if not assigned
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
            
        // Store original size
        originalSize = rectTransform.sizeDelta;
    }
    
    void Start()
    {
        ConfigureComponents();
        SetupButtonListeners();
        SetupVisualAppearance();
    }
    
    void ConfigureComponents()
    {
        // Configure ContentSizeFitter
        if (contentSizeFitter != null)
        {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        // Configure LayoutElement
        if (layoutElement != null)
        {
            layoutElement.preferredWidth = minButtonWidth;
            layoutElement.preferredHeight = minButtonHeight;
            layoutElement.minWidth = minButtonWidth;
            layoutElement.minHeight = minButtonHeight;
        }
        
        // Configure TextMeshPro
        if (choiceText != null)
        {
            choiceText.enableAutoSizing = enableTextAutoSizing;
            choiceText.fontSizeMin = textMinSize;
            choiceText.fontSizeMax = textMaxSize;
            choiceText.alignment = TextAlignmentOptions.Center;
            choiceText.enableWordWrapping = true;
            choiceText.overflowMode = TextOverflowModes.Overflow;
        }
    }
    
    void SetupButtonListeners()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }
    
    void SetupVisualAppearance()
    {
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        
        // Set up button color transitions
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = normalColor;
            colors.pressedColor = pressedColor;
            colors.disabledColor = disabledColor;
            button.colors = colors;
        }
    }
    
    public void ConfigureChoice(string choiceText, System.Action<string> onSelected = null)
    {
        if (this.choiceText != null)
        {
            this.choiceText.text = choiceText;
        }
        
        if (onSelected != null)
        {
            OnChoiceSelected = onSelected;
        }
        
        // Adjust button size to fit content
        if (enableAutoSizing)
        {
            StartCoroutine(AdjustButtonSize());
        }
        
        OnButtonConfigured?.Invoke(this);
        
        Debug.Log($"AdaptiveChoiceButton: Configured choice '{choiceText}'");
    }
    
    public void ConfigureChoiceWithCallback(string choiceText, System.Action<string> onSelected)
    {
        ConfigureChoice(choiceText, onSelected);
    }
    
    IEnumerator AdjustButtonSize()
    {
        if (isAnimating) yield break;
        
        isAnimating = true;
        
        // Wait for text to update
        yield return null;
        yield return null;
        
        if (choiceText == null) 
        {
            isAnimating = false;
            yield break;
        }
        
        // Force text mesh to update
        choiceText.ForceMeshUpdate();
        
        // Get preferred size of the text
        Vector2 textSize = choiceText.GetPreferredValues();
        
        // Calculate new button size with padding
        Vector2 targetSize = new Vector2(
            Mathf.Clamp(textSize.x + textPadding, minButtonWidth, maxButtonWidth),
            Mathf.Clamp(textSize.y + textPadding, minButtonHeight, maxButtonHeight)
        );
        
        // Update layout element preferred size
        if (layoutElement != null)
        {
            layoutElement.preferredWidth = targetSize.x;
            layoutElement.preferredHeight = targetSize.y;
        }
        
        // Animate size change if significantly different
        Vector2 currentSize = rectTransform.sizeDelta;
        if (Vector2.Distance(currentSize, targetSize) > 5f)
        {
            yield return StartCoroutine(AnimateSizeChange(currentSize, targetSize));
        }
        
        isAnimating = false;
    }
    
    IEnumerator AnimateSizeChange(Vector2 fromSize, Vector2 toSize)
    {
        float elapsed = 0f;
        
        while (elapsed < resizeAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / resizeAnimationDuration;
            progress = resizeCurve.Evaluate(progress);
            
            Vector2 currentSize = Vector2.Lerp(fromSize, toSize, progress);
            rectTransform.sizeDelta = currentSize;
            
            yield return null;
        }
        
        rectTransform.sizeDelta = toSize;
    }
    
    void OnButtonClicked()
    {
        if (choiceText != null)
        {
            string selectedChoice = choiceText.text;
            OnChoiceSelected?.Invoke(selectedChoice);
            Debug.Log($"AdaptiveChoiceButton: Choice selected - '{selectedChoice}'");
        }
    }
    
    // Public methods for external control
    public void SetChoiceText(string text)
    {
        ConfigureChoice(text);
    }
    
    public void SetEnabled(bool enabled)
    {
        if (button != null)
        {
            button.interactable = enabled;
        }
    }
    
    public void SetAutoSizing(bool enabled)
    {
        enableAutoSizing = enabled;
        if (contentSizeFitter != null)
        {
            contentSizeFitter.enabled = enabled;
        }
    }
    
    public void SetMinMaxSizes(float minWidth, float maxWidth, float minHeight, float maxHeight)
    {
        minButtonWidth = minWidth;
        maxButtonWidth = maxWidth;
        minButtonHeight = minHeight;
        maxButtonHeight = maxHeight;
        
        if (layoutElement != null)
        {
            layoutElement.minWidth = minWidth;
            layoutElement.minHeight = minHeight;
        }
    }
    
    public void SetTextSizes(float minSize, float maxSize)
    {
        textMinSize = minSize;
        textMaxSize = maxSize;
        
        if (choiceText != null)
        {
            choiceText.fontSizeMin = minSize;
            choiceText.fontSizeMax = maxSize;
        }
    }
    
    // Visual feedback methods
    public void HighlightButton(bool highlight)
    {
        if (buttonImage != null)
        {
            buttonImage.color = highlight ? pressedColor : normalColor;
        }
    }
    
    public void PulseButton()
    {
        StartCoroutine(PulseAnimation());
    }
    
    IEnumerator PulseAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 pulseScale = originalScale * 1.1f;
        
        float duration = 0.2f;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, pulseScale, progress);
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale down
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            transform.localScale = Vector3.Lerp(pulseScale, originalScale, progress);
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    // Debug methods
    [ContextMenu("Test Long Text")]
    public void TestLongText()
    {
        ConfigureChoice("This is a very long choice text that should test the auto-sizing functionality of the adaptive choice button system.");
    }
    
    [ContextMenu("Test Short Text")]
    public void TestShortText()
    {
        ConfigureChoice("Yes");
    }
    
    [ContextMenu("Test Medium Text")]
    public void TestMediumText()
    {
        ConfigureChoice("This is a medium length choice that tests normal functionality.");
    }
}
