using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mobile-friendly scroll indicator for DialogBox
/// - User swipes to scroll (touch/mouse drag)
/// - Arrow indicator shows scroll direction availability
/// - Scrollbar shows current scroll position visually
/// </summary>
public class MobileScrollIndicator : MonoBehaviour
{
    [Header("Scroll Indicators (Auto-loaded)")]
    private Sprite scrollArrowSprite;      // scroll sprite
    private Sprite scrollBarSprite;        // ScrollBar sprite
    private Sprite scrollPageSprite;       // scrollpage-Sheet sprite
    
    [Header("Auto-Setup")]
    [Tooltip("Check this to auto-setup scroll indicators on Start")]
    public bool autoSetup = true;
    
    [Header("References (Auto-assigned)")]
    public ScrollRect scrollRect;
    public RectTransform content;
    
    [Header("Indicator GameObjects")]
    public GameObject topArrowIndicator;
    public GameObject bottomArrowIndicator;
    public Image scrollPositionBar;
    public Image scrollPositionFill;
    
    [Header("Indicator Settings")]
    public float arrowSize = 40f;
    public float arrowOffsetFromEdge = 10f;
    public float scrollBarWidth = 30f;
    public float scrollBarOffsetFromRight = 10f;
    
    [Header("Visual Feedback")]
    public Color canScrollColor = Color.white;
    public Color cannotScrollColor = new Color(1f, 1f, 1f, 0.3f); // Faded white
    public float fadeSpeed = 5f;
    
    private bool canScrollUp = false;
    private bool canScrollDown = false;
    
    void Start()
    {
        if (autoSetup)
        {
            LoadSprites();
            SetupScrollIndicators();
        }
    }
    
    void Update()
    {
        UpdateScrollIndicators();
    }
    
    void LoadSprites()
    {
        scrollArrowSprite = Resources.Load<Sprite>("DialogBox/scroll");
        scrollBarSprite = Resources.Load<Sprite>("DialogBox/ScrollBar");
        scrollPageSprite = Resources.Load<Sprite>("DialogBox/scrollpage-Sheet");
        
        if (scrollArrowSprite != null)
            Debug.Log("✅ Loaded scroll arrow sprite for indicator");
        else
            Debug.LogWarning("⚠️ scroll sprite not found in Resources/DialogBox/");
            
        if (scrollBarSprite != null)
            Debug.Log("✅ Loaded ScrollBar sprite for position indicator");
        else
            Debug.LogWarning("⚠️ ScrollBar sprite not found in Resources/DialogBox/");
            
        if (scrollPageSprite != null)
            Debug.Log("✅ Loaded scrollpage sprite for position fill");
        else
            Debug.LogWarning("⚠️ scrollpage-Sheet sprite not found in Resources/DialogBox/");
    }
    
    [ContextMenu("Setup Scroll Indicators")]
    public void SetupScrollIndicators()
    {
        Debug.Log("🔧 Setting up mobile scroll indicators...");
        
        // Get or create ScrollRect
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            scrollRect = gameObject.AddComponent<ScrollRect>();
            Debug.Log("✅ Added ScrollRect component");
        }
        
        // Configure ScrollRect for mobile touch
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.1f;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
        scrollRect.scrollSensitivity = 1f;
        scrollRect.verticalScrollbar = null; // No scrollbar needed for mobile!
        
        // Find or create Content
        Transform contentTransform = transform.Find("Content");
        if (contentTransform == null)
        {
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(transform);
            content = contentObj.AddComponent<RectTransform>();
            
            // Configure Content
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.anchoredPosition = Vector2.zero;
            
            // Add ContentSizeFitter for dynamic sizing
            ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            Debug.Log("✅ Created Content with ContentSizeFitter");
        }
        else
        {
            content = contentTransform.GetComponent<RectTransform>();
        }
        
        scrollRect.content = content;
        scrollRect.viewport = GetComponent<RectTransform>();
        
        // Create visual indicators
        CreateTopArrowIndicator();
        CreateBottomArrowIndicator();
        CreateScrollPositionBar();
        
        Debug.Log("✅ Mobile scroll indicators setup complete!");
    }
    
    void CreateTopArrowIndicator()
    {
        // Check if already exists
        Transform existing = transform.Find("TopScrollIndicator");
        if (existing != null)
        {
            topArrowIndicator = existing.gameObject;
            return;
        }
        
        topArrowIndicator = new GameObject("TopScrollIndicator");
        topArrowIndicator.transform.SetParent(transform);
        
        RectTransform rect = topArrowIndicator.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -arrowOffsetFromEdge);
        rect.sizeDelta = new Vector2(arrowSize, arrowSize);
        
        Image image = topArrowIndicator.AddComponent<Image>();
        if (scrollArrowSprite != null)
        {
            image.sprite = scrollArrowSprite;
            image.color = cannotScrollColor; // Start faded
        }
        
        // Arrow points up
        rect.localRotation = Quaternion.Euler(0, 0, 0);
        
        Debug.Log("✅ Created top scroll indicator (shows when can scroll up)");
    }
    
    void CreateBottomArrowIndicator()
    {
        // Check if already exists
        Transform existing = transform.Find("BottomScrollIndicator");
        if (existing != null)
        {
            bottomArrowIndicator = existing.gameObject;
            return;
        }
        
        bottomArrowIndicator = new GameObject("BottomScrollIndicator");
        bottomArrowIndicator.transform.SetParent(transform);
        
        RectTransform rect = bottomArrowIndicator.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = new Vector2(0, arrowOffsetFromEdge);
        rect.sizeDelta = new Vector2(arrowSize, arrowSize);
        
        Image image = bottomArrowIndicator.AddComponent<Image>();
        if (scrollArrowSprite != null)
        {
            image.sprite = scrollArrowSprite;
            image.color = canScrollColor; // Start visible (can scroll down initially)
        }
        
        // Arrow points down
        rect.localRotation = Quaternion.Euler(0, 0, 180);
        
        Debug.Log("✅ Created bottom scroll indicator (shows when can scroll down)");
    }
    
    void CreateScrollPositionBar()
    {
        // Check if already exists
        Transform existing = transform.Find("ScrollPositionBar");
        if (existing != null)
        {
            scrollPositionBar = existing.GetComponent<Image>();
            Transform fill = existing.Find("ScrollPositionFill");
            if (fill != null)
                scrollPositionFill = fill.GetComponent<Image>();
            return;
        }
        
        // Create background bar
        GameObject barObj = new GameObject("ScrollPositionBar");
        barObj.transform.SetParent(transform);
        
        RectTransform barRect = barObj.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(1, 0);
        barRect.anchorMax = new Vector2(1, 1);
        barRect.pivot = new Vector2(1, 0.5f);
        barRect.anchoredPosition = new Vector2(-scrollBarOffsetFromRight, 0);
        barRect.sizeDelta = new Vector2(scrollBarWidth, 0);
        
        scrollPositionBar = barObj.AddComponent<Image>();
        if (scrollBarSprite != null)
        {
            scrollPositionBar.sprite = scrollBarSprite;
            scrollPositionBar.type = Image.Type.Sliced;
            scrollPositionBar.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent
        }
        
        // Create fill (shows scroll position)
        GameObject fillObj = new GameObject("ScrollPositionFill");
        fillObj.transform.SetParent(barObj.transform);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 0);
        fillRect.pivot = new Vector2(0.5f, 0);
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.sizeDelta = new Vector2(0, 0);
        
        scrollPositionFill = fillObj.AddComponent<Image>();
        if (scrollPageSprite != null)
        {
            scrollPositionFill.sprite = scrollPageSprite;
            scrollPositionFill.type = Image.Type.Sliced;
            scrollPositionFill.color = canScrollColor;
        }
        
        Debug.Log("✅ Created scroll position bar (shows current scroll position)");
    }
    
    void UpdateScrollIndicators()
    {
        if (scrollRect == null || content == null) return;
        
        // Calculate if can scroll
        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        
        bool hasContent = contentHeight > viewportHeight;
        
        if (hasContent)
        {
            float scrollPos = scrollRect.verticalNormalizedPosition;
            
            // Can scroll up if not at top
            canScrollUp = scrollPos < 0.99f;
            
            // Can scroll down if not at bottom
            canScrollDown = scrollPos > 0.01f;
            
            // Update scroll position fill height
            if (scrollPositionFill != null)
            {
                RectTransform fillRect = scrollPositionFill.rectTransform;
                float fillHeight = scrollRect.viewport.rect.height * (viewportHeight / contentHeight);
                fillRect.anchoredPosition = new Vector2(0, scrollPos * (scrollRect.viewport.rect.height - fillHeight));
                fillRect.sizeDelta = new Vector2(0, fillHeight);
            }
        }
        else
        {
            canScrollUp = false;
            canScrollDown = false;
        }
        
        // Update arrow indicators
        UpdateArrowIndicator(topArrowIndicator, canScrollUp);
        UpdateArrowIndicator(bottomArrowIndicator, canScrollDown);
        
        // Show/hide position bar
        if (scrollPositionBar != null)
        {
            scrollPositionBar.gameObject.SetActive(hasContent);
        }
    }
    
    void UpdateArrowIndicator(GameObject indicator, bool canScroll)
    {
        if (indicator == null) return;
        
        Image image = indicator.GetComponent<Image>();
        if (image == null) return;
        
        Color targetColor = canScroll ? canScrollColor : cannotScrollColor;
        image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * fadeSpeed);
    }
    
    /// <summary>
    /// Call this to force show/hide indicators (useful for debugging)
    /// </summary>
    public void SetIndicatorsActive(bool active)
    {
        if (topArrowIndicator != null)
            topArrowIndicator.SetActive(active);
        if (bottomArrowIndicator != null)
            bottomArrowIndicator.SetActive(active);
        if (scrollPositionBar != null)
            scrollPositionBar.gameObject.SetActive(active);
    }
}

