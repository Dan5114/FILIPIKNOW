using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple script to make the scroll arrow follow the scroll position
/// Attach this to your scroll1 arrow GameObject
/// </summary>
public class ScrollIndicator : MonoBehaviour
{
    [Header("Scroll References")]
    public ScrollRect scrollRect; // Drag your DialogBox here
    public RectTransform scrollBarBackground; // Drag your ScrollBar background here
    public RectTransform arrowTransform; // This script's RectTransform (auto-assigned)
    
    [Header("Arrow Movement")]
    public float topOffset = 10f; // Distance from top of scrollbar
    public float bottomOffset = 10f; // Distance from bottom of scrollbar
    
    void Awake()
    {
        // Get this arrow's RectTransform
        arrowTransform = GetComponent<RectTransform>();
        
        // Auto-find ScrollRect if not assigned
        if (scrollRect == null)
        {
            scrollRect = GetComponentInParent<ScrollRect>();
        }
    }
    
    void Update()
    {
        if (scrollRect != null && scrollBarBackground != null && arrowTransform != null)
        {
            UpdateArrowPosition();
        }
    }
    
    void UpdateArrowPosition()
    {
        // Get scroll position (0 = bottom, 1 = top)
        float scrollPos = scrollRect.verticalNormalizedPosition;
        
        // Get scrollbar height
        float scrollBarHeight = scrollBarBackground.rect.height;
        
        // Calculate arrow position
        // Map scroll position to arrow position within scrollbar
        float arrowY = Mathf.Lerp(-scrollBarHeight/2 + bottomOffset, 
                                  scrollBarHeight/2 - topOffset, 
                                  scrollPos);
        
        // Update arrow position (keep X position, only change Y)
        Vector2 currentPos = arrowTransform.anchoredPosition;
        arrowTransform.anchoredPosition = new Vector2(currentPos.x, arrowY);
    }
}
