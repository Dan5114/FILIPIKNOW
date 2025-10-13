using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Automatically sets up custom scrollbar with your sprites from Resources/DialogBox
/// Attach this to your DialogBox GameObject and it will configure everything!
/// </summary>
public class DialogScrollbarSetup : MonoBehaviour
{
    [Header("Auto-Setup")]
    [Tooltip("Click this button in Inspector to auto-setup scrollbar")]
    public bool autoSetupScrollbar = false;
    
    [Header("Scrollbar Sprites (Auto-loaded from Resources/DialogBox)")]
    private Sprite scrollBarBackground;  // ScrollBar sprite
    private Sprite scrollBarHandle;      // scrollpage-Sheet sprite
    private Sprite scrollArrow;          // scroll sprite
    
    [Header("References (Auto-assigned)")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public Scrollbar verticalScrollbar;
    
    [Header("Scrollbar Settings")]
    public float scrollbarWidth = 40f;
    public float scrollbarOffsetFromRight = 10f;
    public float handleHeight = 60f;
    
    void Start()
    {
        // Load sprites from Resources/DialogBox/
        LoadSprites();
        
        // Auto-setup if needed
        if (autoSetupScrollbar)
        {
            SetupScrollbar();
        }
    }
    
    void LoadSprites()
    {
        scrollBarBackground = Resources.Load<Sprite>("DialogBox/ScrollBar");
        scrollBarHandle = Resources.Load<Sprite>("DialogBox/scrollpage-Sheet");
        scrollArrow = Resources.Load<Sprite>("DialogBox/scroll");
        
        if (scrollBarBackground != null)
            Debug.Log("✅ Loaded ScrollBar background sprite");
        else
            Debug.LogWarning("⚠️ ScrollBar sprite not found in Resources/DialogBox/");
            
        if (scrollBarHandle != null)
            Debug.Log("✅ Loaded scrollpage-Sheet handle sprite");
        else
            Debug.LogWarning("⚠️ scrollpage-Sheet sprite not found in Resources/DialogBox/");
            
        if (scrollArrow != null)
            Debug.Log("✅ Loaded scroll arrow sprite");
        else
            Debug.LogWarning("⚠️ scroll sprite not found in Resources/DialogBox/");
    }
    
    [ContextMenu("Setup Scrollbar")]
    public void SetupScrollbar()
    {
        Debug.Log("🔧 Setting up custom scrollbar...");
        
        // Get or create ScrollRect
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            scrollRect = gameObject.AddComponent<ScrollRect>();
            Debug.Log("✅ Added ScrollRect component");
        }
        
        // Find or create Content
        Transform contentTransform = transform.Find("Content");
        if (contentTransform == null)
        {
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(transform);
            content = contentObj.AddComponent<RectTransform>();
            
            // Configure Content RectTransform
            content.anchorMin = new Vector2(0, 0);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;
            
            // Add ContentSizeFitter
            ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            Debug.Log("✅ Created Content with ContentSizeFitter");
        }
        else
        {
            content = contentTransform.GetComponent<RectTransform>();
        }
        
        // Create or find VerticalScrollbar
        Transform scrollbarTransform = transform.Find("VerticalScrollbar");
        if (scrollbarTransform == null)
        {
            verticalScrollbar = CreateCustomScrollbar();
        }
        else
        {
            verticalScrollbar = scrollbarTransform.GetComponent<Scrollbar>();
            UpdateScrollbarSprites();
        }
        
        // Configure ScrollRect
        scrollRect.content = content;
        scrollRect.viewport = GetComponent<RectTransform>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.verticalScrollbar = verticalScrollbar;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.scrollSensitivity = 30f;
        
        Debug.Log("✅ Scrollbar setup complete!");
    }
    
    Scrollbar CreateCustomScrollbar()
    {
        // Create scrollbar GameObject
        GameObject scrollbarObj = new GameObject("VerticalScrollbar");
        scrollbarObj.transform.SetParent(transform);
        
        RectTransform scrollbarRect = scrollbarObj.AddComponent<RectTransform>();
        
        // Position on right side
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.pivot = new Vector2(1, 0.5f);
        scrollbarRect.anchoredPosition = new Vector2(-scrollbarOffsetFromRight, 0);
        scrollbarRect.sizeDelta = new Vector2(scrollbarWidth, 0);
        
        // Add Image for background
        Image bgImage = scrollbarObj.AddComponent<Image>();
        if (scrollBarBackground != null)
        {
            bgImage.sprite = scrollBarBackground;
            bgImage.type = Image.Type.Sliced;
            Debug.Log("✅ Applied ScrollBar background sprite");
        }
        
        // Add Scrollbar component
        Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        
        // Create Sliding Area
        GameObject slidingArea = new GameObject("Sliding Area");
        slidingArea.transform.SetParent(scrollbarObj.transform);
        RectTransform slidingRect = slidingArea.AddComponent<RectTransform>();
        slidingRect.anchorMin = Vector2.zero;
        slidingRect.anchorMax = Vector2.one;
        slidingRect.sizeDelta = Vector2.zero;
        slidingRect.anchoredPosition = Vector2.zero;
        
        // Create Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(slidingArea.transform);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.one;
        handleRect.sizeDelta = new Vector2(0, handleHeight);
        handleRect.anchoredPosition = Vector2.zero;
        
        // Add Handle Image
        Image handleImage = handle.AddComponent<Image>();
        if (scrollBarHandle != null)
        {
            handleImage.sprite = scrollBarHandle;
            handleImage.type = Image.Type.Simple;
            Debug.Log("✅ Applied scrollpage-Sheet handle sprite");
        }
        
        // Link handle to scrollbar
        scrollbar.handleRect = handleRect;
        scrollbar.targetGraphic = handleImage;
        
        Debug.Log("✅ Created custom scrollbar with sprites");
        
        return scrollbar;
    }
    
    void UpdateScrollbarSprites()
    {
        if (verticalScrollbar == null) return;
        
        // Update background sprite
        Image bgImage = verticalScrollbar.GetComponent<Image>();
        if (bgImage != null && scrollBarBackground != null)
        {
            bgImage.sprite = scrollBarBackground;
            bgImage.type = Image.Type.Sliced;
        }
        
        // Update handle sprite
        if (verticalScrollbar.handleRect != null)
        {
            Image handleImage = verticalScrollbar.handleRect.GetComponent<Image>();
            if (handleImage != null && scrollBarHandle != null)
            {
                handleImage.sprite = scrollBarHandle;
                handleImage.type = Image.Type.Simple;
            }
        }
        
        Debug.Log("✅ Updated scrollbar sprites");
    }
    
    /// <summary>
    /// Optional: Create scroll arrow buttons (up/down)
    /// </summary>
    [ContextMenu("Add Scroll Arrows")]
    public void AddScrollArrows()
    {
        if (scrollArrow == null)
        {
            Debug.LogWarning("⚠️ scroll sprite not loaded, cannot add arrows");
            return;
        }
        
        // Create Up Arrow
        GameObject upArrow = CreateArrowButton("UpArrow", true);
        if (upArrow != null)
        {
            Button upButton = upArrow.GetComponent<Button>();
            upButton.onClick.AddListener(() => ScrollUp());
        }
        
        // Create Down Arrow
        GameObject downArrow = CreateArrowButton("DownArrow", false);
        if (downArrow != null)
        {
            Button downButton = downArrow.GetComponent<Button>();
            downButton.onClick.AddListener(() => ScrollDown());
        }
        
        Debug.Log("✅ Added scroll arrow buttons");
    }
    
    GameObject CreateArrowButton(string name, bool isUpArrow)
    {
        GameObject arrow = new GameObject(name);
        arrow.transform.SetParent(transform);
        
        RectTransform arrowRect = arrow.AddComponent<RectTransform>();
        arrowRect.sizeDelta = new Vector2(scrollbarWidth, scrollbarWidth);
        
        // Position arrows
        if (isUpArrow)
        {
            arrowRect.anchorMin = new Vector2(1, 1);
            arrowRect.anchorMax = new Vector2(1, 1);
            arrowRect.pivot = new Vector2(1, 1);
            arrowRect.anchoredPosition = new Vector2(-scrollbarOffsetFromRight, 0);
        }
        else
        {
            arrowRect.anchorMin = new Vector2(1, 0);
            arrowRect.anchorMax = new Vector2(1, 0);
            arrowRect.pivot = new Vector2(1, 0);
            arrowRect.anchoredPosition = new Vector2(-scrollbarOffsetFromRight, 0);
        }
        
        // Add Image
        Image arrowImage = arrow.AddComponent<Image>();
        arrowImage.sprite = scrollArrow;
        
        // Rotate for down arrow
        if (!isUpArrow)
        {
            arrowRect.localRotation = Quaternion.Euler(0, 0, 180);
        }
        
        // Add Button
        Button button = arrow.AddComponent<Button>();
        button.targetGraphic = arrowImage;
        
        return arrow;
    }
    
    void ScrollUp()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + 0.1f);
        }
    }
    
    void ScrollDown()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - 0.1f);
        }
    }
}

