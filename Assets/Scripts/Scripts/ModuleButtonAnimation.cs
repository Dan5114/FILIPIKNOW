using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ModuleButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Animation Settings")]
    public float hoverScale = 1.08f;
    public float clickScale = 0.95f;
    public float animationSpeed = 10f;
    public float clickAnimationSpeed = 15f;
    
    [Header("Image References")]
    public Image buttonImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;
    
    [Header("Color & Effects")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 1f, 0.9f, 1f);
    public Color clickColor = new Color(0.9f, 0.9f, 0.8f, 1f);
    
    [Header("Glow Effect")]
    public bool useGlowEffect = true;
    public float glowIntensity = 1.3f;
    public Color glowColor = new Color(1f, 1f, 0.7f, 0.5f);
    
    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    
    private Vector3 originalScale;
    private Color originalColor;
    private bool isAnimating = false;
    private Coroutine currentAnimation;
    
    void Start()
    {
        // Store original values
        originalScale = transform.localScale;
        
        // Get components if not assigned
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
            
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
            
            // Set normal sprite if assigned
            if (normalSprite != null)
                buttonImage.sprite = normalSprite;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            PlayHoverAnimation();
            PlaySound(hoverSound);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            PlayNormalAnimation();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isAnimating)
        {
            PlayClickAnimation();
            PlaySound(clickSound);
        }
    }
    
    private void PlayHoverAnimation()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
            
        currentAnimation = StartCoroutine(AnimateToState(
            originalScale * hoverScale, 
            hoverColor, 
            hoverSprite, 
            animationSpeed
        ));
    }
    
    private void PlayNormalAnimation()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
            
        currentAnimation = StartCoroutine(AnimateToState(
            originalScale, 
            originalColor, 
            normalSprite, 
            animationSpeed
        ));
    }
    
    private void PlayClickAnimation()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        
        // Quick click animation
        currentAnimation = StartCoroutine(AnimateClick());
    }
    
    private IEnumerator AnimateClick()
    {
        isAnimating = true;
        
        // Quick scale down
        Vector3 clickScale = originalScale * this.clickScale;
        yield return StartCoroutine(AnimateToState(clickScale, clickColor, hoverSprite, clickAnimationSpeed));
        
        // Quick scale back to hover
        yield return StartCoroutine(AnimateToState(originalScale * hoverScale, hoverColor, hoverSprite, clickAnimationSpeed));
        
        isAnimating = false;
    }
    
    private IEnumerator AnimateToState(Vector3 targetScale, Color targetColor, Sprite targetSprite, float speed)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = buttonImage.color;
        
        float elapsedTime = 0f;
        float duration = 1f / speed;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // Smooth interpolation using ease-out
            float smoothProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            // Animate scale
            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothProgress);
            
            // Animate color
            if (buttonImage != null)
            {
                buttonImage.color = Color.Lerp(startColor, targetColor, smoothProgress);
                
                // Change sprite if available
                if (targetSprite != null)
                    buttonImage.sprite = targetSprite;
            }
            
            yield return null;
        }
        
        // Ensure final values are set
        transform.localScale = targetScale;
        if (buttonImage != null)
        {
            buttonImage.color = targetColor;
            if (targetSprite != null)
                buttonImage.sprite = targetSprite;
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Public methods for external control
    public void SetNormalState()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        transform.localScale = originalScale;
        if (buttonImage != null)
        {
            buttonImage.color = originalColor;
            if (normalSprite != null)
                buttonImage.sprite = normalSprite;
        }
        isAnimating = false;
    }
    
    public void SetHoverState()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        transform.localScale = originalScale * hoverScale;
        if (buttonImage != null)
        {
            buttonImage.color = hoverColor;
            if (hoverSprite != null)
                buttonImage.sprite = hoverSprite;
        }
        isAnimating = false;
    }
}